using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.SignUpUser;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;

namespace SPLITTR_Uwp.ViewModel
{
   
    public class SignPageViewModel : ObservableObject
    {
        
        private bool _emailPassInputPanelVisibility;
        private string _userName;
        private bool _isValidEmailIdTextBlockVisibility;
        private bool _userAlreadyExistTextBlockVisibility;

        public List<string> _currencyList = new List<string>
        {
            "Rupees ₹",
            "Dollar $",
            "Euro   €",
            "Yen    ¥"
        };

        public int SelectedIndex { get; set; }


        public bool EmailPassInputPanelVisibility
        {
            get => _emailPassInputPanelVisibility;
            set
            {
                SetProperty(ref _emailPassInputPanelVisibility, value);
            }
        }

        public string EmailIdText { get; set; }

        public string UserName
        {
            get => _userName;
            set
            {
              _userName = value;
              UserNameValueChanged();
            }
        }
        
        public bool IsValidEmailIdTextBlockVisibility
        {
            get => _isValidEmailIdTextBlockVisibility;
            set => SetProperty(ref _isValidEmailIdTextBlockVisibility, value);
        }

        public bool UserAlreadyExistTextBlockVisibility
        {
            get => _userAlreadyExistTextBlockVisibility;
            set => SetProperty(ref _userAlreadyExistTextBlockVisibility, value);
        }

        private void UserNameValueChanged()
        {
            EmailPassInputPanelVisibility = _userName.Length > 3;
        }


        public void LoginButtonOnClicked()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate<LoginPage>(infoOverride: new SlideNavigationTransitionInfo
                    { Effect = SlideNavigationTransitionEffect.FromLeft });
            }
        }


        public  void OnSignUpButtonClicked()
        {
            if (string.IsNullOrEmpty(EmailIdText) ||
                !EmailIdText.ContainsString(new[]
                {
                    "@gmail", "@yahoo", "@zoho", "@bitsathy"
                }))
            {
                IsValidEmailIdTextBlockVisibility = true;
                UserAlreadyExistTextBlockVisibility = false;
                return;
            }
            IsValidEmailIdTextBlockVisibility=false;
            
            var cts = new CancellationTokenSource();

            var signUpReqObj = new SignUpUserReqObj(SelectedIndex, EmailIdText.Trim().ToLower(), UserName.Trim(), new SignUpVmPresenterCallBack(this), cts.Token);

            var signUpUseCaseOBj = InstanceBuilder.CreateInstance<SignUpUser>(signUpReqObj);

            signUpUseCaseOBj.Execute();

        }
        private async void InvokeOnSignUpSuccessFull(SignUpUserResponseObj result)
        {
           await UiService.RunOnUiThread(() =>
           {
               UserAlreadyExistTextBlockVisibility = false;
               IsValidEmailIdTextBlockVisibility = false;

           });
           await ShowSignUpSuccessFullMessageBoxAsync().ConfigureAwait(false);

        }
        private async void InvokeOnSignUpFailed(SplittrException ex)
        {
            if (ex.InnerException is UserAlreadyExistException)
            {
              await  UiService.RunOnUiThread(() =>
              {
                  UserAlreadyExistTextBlockVisibility = true;
              });

            }
        }
        private Task ShowSignUpSuccessFullMessageBoxAsync()
        {
            IUICommand msgBoxResult;

            return UiService.RunOnUiThread(
                async () =>
                {
                    MessageDialog msg = new MessageDialog("Account Created SuccessFully", "Signed Up SuccessFully!!");

                    msg.Commands.Add(new UICommand("LogIn Page"));
                    msg.Commands.Add(new UICommand("close"));
                    msgBoxResult = await msg.ShowAsync();
                    if (msgBoxResult.Label.Equals("LogIn Page"))
                    {
                        LoginButtonOnClicked();
                    }
                });
        }
        private class SignUpVmPresenterCallBack : IPresenterCallBack<SignUpUserResponseObj>
        {
            private readonly SignPageViewModel _viewModel;
            public SignUpVmPresenterCallBack(SignPageViewModel viewModel)
            {
                _viewModel = viewModel;

            }
            public void OnSuccess(SignUpUserResponseObj result)
            {
                _viewModel.InvokeOnSignUpSuccessFull(result);
            }
            public void OnError(SplittrException ex)
            {
                _viewModel.InvokeOnSignUpFailed(ex);
            }
        }
    }
}
