using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.LoginUser;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;
using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace SPLITTR_Uwp.ViewModel
{

    public class LoginPageViewModel : ObservableObject
    {
        private readonly IStateService _stateManager;
        private int _selectedItem = 0;
        private bool _loginInformationTextBox = false;
        private bool _wrongUserCredentialTextBlockVisibility = false;

        public int SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }


        public bool LoginInformationTextBox
        {
            get => _loginInformationTextBox;
            set => SetProperty(ref _loginInformationTextBox, value);
        }

        public bool WrongUserCreDentialTextBlockVisibility
        {
            get => _wrongUserCredentialTextBlockVisibility;
            set => SetProperty(ref _wrongUserCredentialTextBlockVisibility, value);
        }

        public string UserEmailIdTextBox { get; set; }

        private DispatcherTimer _timer;

        public LoginPageViewModel(IStateService stateManager)
        {
            _stateManager = stateManager;
            //Firing a event for every 0.5 seconds
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

        }

        private int _change = 1;

        private void Timer_Tick(object sender, object e)
        {
            SelectedItem += _change;
            if (SelectedItem < 1 || SelectedItem > 2)
            {
                _change *= -1;
            }
        }

        public  void LoginButtonPressed()
        {
            //===============================================/
            //By PAss To Be Deleted

            // UserEmailIdTextBox = "saran@gmail.com";
            //===============================================//

            if (string.IsNullOrWhiteSpace(UserEmailIdTextBox))
            {
                LoginInformationTextBox = true;
                WrongUserCreDentialTextBlockVisibility = false;
                return;
            }
            if (!UserEmailIdTextBox.ContainsString(new string[]
                {
                    "@gmail", "@yahoo", "@zoho", "@bitsathy"
                }))
            {
                LoginInformationTextBox = false;
                WrongUserCreDentialTextBlockVisibility = true;
            }
            else
            {
                var cts = new CancellationTokenSource();

                var loginReqObj = new LoginRequestObj(UserEmailIdTextBox.Trim().ToLower(), new LoginVmPresenterCalBack(this), cts.Token);

                var loginUseCaseObj = InstanceBuilder.CreateInstance<UserLogin>(loginReqObj);

                loginUseCaseObj.Execute();
            }


        }
        public void SignUpButtonOnClick()
        {
            NavigationService.Frame.Navigate(typeof(SignUpPage), null, infoOverride: new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        public void PageUnloaded()
        {
            //stopping main page animation if the the page is unloaded
            _timer.Stop();
        }
        private async void OnLoginCompleted(LoginResponseObj result)
        {
            await UiService.RunOnUiThread((() =>
            {
                if (!result.IsUserAlreadyExist)
                {
                    WrongUserCreDentialTextBlockVisibility = true;
                    return;
                }
                WrongUserCreDentialTextBlockVisibility = false;
                Store.CurreUserBobj = result.LoginUserCred;
                NavigationService.Navigate<MainPage>();
            }));
            //Storing Current User Session 
            _stateManager?.RegisterUserLoginSession(Store.CurreUserBobj);
        }


        private class LoginVmPresenterCalBack : IPresenterCallBack<LoginResponseObj>
        {
            private readonly LoginPageViewModel _viewModel;
            public LoginVmPresenterCalBack(LoginPageViewModel viewModel)
            {
                _viewModel = viewModel;
            }
            public void OnSuccess(LoginResponseObj result)
            {
                _viewModel.OnLoginCompleted(result);
            }
            public async void OnError(SplittrException ex)
            {
                await UiService.RunOnUiThread((() =>
                {
                    if (ex.InnerException is ArgumentNullException)
                    {
                        _viewModel.WrongUserCreDentialTextBlockVisibility = true;
                    }
                }));

            }
        }
    }
}
