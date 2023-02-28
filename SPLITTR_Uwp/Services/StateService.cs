using System;
using System.Linq;
using System.Threading;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml.Media.Animation;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.LoginUser;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Views;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.Services
{
    public class StateService : IStateService
    {

        #region EventsAndInvokator
        private static Action<UserBobj> _onLogoutAction;

        public static event Action<UserBobj> OnUserLoggedOut
        {
            add => _onLogoutAction += value;
            remove => _onLogoutAction -= value;
        }

        public static event Action<UserBobj> OnUserLoggin;

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        private void InvokeCurrentUserLoggedOut(UserBobj user)
        {
            _onLogoutAction?.Invoke(user);
        }
        private void InvokeUserLoggingIn(UserBobj obj)
        {
            OnUserLoggin?.Invoke(obj);
        }

        #endregion


        public bool RequestUserLogout()
        {
            InvokeCurrentUserLoggedOut(Store.CurrentUserBobj);

            //Clearing Reference to Current USer Cache
            Store.CurrentUserBobj = null;
            //Clearing User Session Details in 
            RevokeSessionLogIn();

            return true;
        }

        public void VerifySessionIfAvailable()
        {
            try
            {
                var email = ApplicationData.Current.LocalSettings.Values["UserEmail"] as string;
                if (string.IsNullOrEmpty(email))
                {
                    RevokeSessionLogIn();
                    return;
                }
                CurrentUserCredential = new PasswordVault().FindAllByResource(email).First();
                if (CurrentUserCredential == null)
                {
                    RevokeSessionLogIn();
                    return;
                }
                //if Previous Session Exist Calling Login User useCase 
                var userLoginReq = new LoginRequestObj(CurrentUserCredential.UserName, new LoginStateServiceCallBack(this), CancellationToken.None);
                var loginUseCase = InstanceBuilder.CreateInstance<UserLogin>(userLoginReq);
                loginUseCase.Execute();

            }
            catch (Exception)
            {
                ApplicationData.Current.LocalSettings.Values["UserEmail"] = "";
                RevokeSessionLogIn();
            }

        }
        private void RevokeSessionLogIn()
        {
            ClearSessionDetails();
            //Navigating Back To Login Page
            NavigationService.Frame = null;
            NavigationService.Navigate(typeof(LoginPage),null,new SuppressNavigationTransitionInfo());


            void ClearSessionDetails()
            {
                //if Session Verification failed no Credential Will be set 
                if (CurrentUserCredential is not null)
                {
                    new PasswordVault().Remove(CurrentUserCredential);
                }
                //Clearing Current User Marker in localSettings
                ApplicationData.Current.LocalSettings.Values["UserEmail"] = "";
            }
        }
        private PasswordCredential CurrentUserCredential { get; set; }

        public void RegisterUserLoginSession(UserBobj currentUser)
        {
            if (currentUser == null)
            {
                return;
            }
            ApplicationData.Current.LocalSettings.Values["UserEmail"] = currentUser.EmailId;
            //String Current User Session in Password Vault
            CurrentUserCredential = new PasswordCredential(currentUser.EmailId, currentUser.EmailId, currentUser.UserName);
            AddToAppVault(CurrentUserCredential);

            void AddToAppVault(PasswordCredential credential)
            {
                new PasswordVault().Add(credential);
            }
        }


        private class LoginStateServiceCallBack :IPresenterCallBack<LoginResponseObj>
        {
            private readonly StateService _stateManager;
            public LoginStateServiceCallBack(StateService stateManager)
            {
                _stateManager = stateManager;

            }

            public async void OnSuccess(LoginResponseObj result)
            {
                await RunOnUiThread(() =>
                {
                    Store.CurrentUserBobj = result.LoginUserCred;
                    NavigationService.Navigate(typeof(MainPage));
                }).ConfigureAwait(false);
                _stateManager.InvokeUserLoggingIn(Store.CurrentUserBobj);

            }
            public async void OnError(SplittrException ex)
            {
                await RunOnUiThread(() =>
                {
                    _stateManager.RevokeSessionLogIn();

                }).ConfigureAwait(false);
            }
        }

     
    }
public interface IStateService
    {
        bool RequestUserLogout();
        void RegisterUserLoginSession(UserBobj currentUser);
        void VerifySessionIfAvailable();
    }
}
