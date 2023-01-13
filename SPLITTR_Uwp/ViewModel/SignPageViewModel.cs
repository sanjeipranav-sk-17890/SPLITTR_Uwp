using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;

namespace SPLITTR_Uwp.ViewModel
{
    public class SignPageViewModel : ObservableObject
    {
        private readonly IUserDataHandler _dataHandler;
        private bool _emailPassInputPanelVisibility=false;
        private string _userName;
        private bool _isValidEmailIdTextBlockVisibility=false;
        private bool _userAlreadyExistTextBlockVisibility=false;

        public List<string> CurrencyList = new List<string>()
        {
            "Rupees ₹",
            "Dollar $",
            "Euro   €",
            "Yen    ¥"
        };

        public int SelectedIndex { get; set; }

        public SignPageViewModel(IUserDataHandler dataHandler)
        {
            _dataHandler = dataHandler;

        }

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
            if (_userName.Length > 3)
            {
                EmailPassInputPanelVisibility = true;

            }
            else
            {
                 EmailPassInputPanelVisibility = false;
            }
        }


        public void LoginButtonOnClicked()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate<LoginPage>(infoOverride: new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
            }
        }


        public async void OnSignUpButtonClicked()
        {
            if (string.IsNullOrEmpty(EmailIdText) ||
                !EmailIdText.ContainsString(new string[]
                {
                    "@gmail", "@yahoo", "@zoho", "@bitsathy"
                }))
            {
                IsValidEmailIdTextBlockVisibility = true;
                UserAlreadyExistTextBlockVisibility = false;
                return;
            }
            if (await _dataHandler.IsUserAlreadyExist(EmailIdText.Trim().ToLower()))
            {
                UserAlreadyExistTextBlockVisibility = true;
                IsValidEmailIdTextBlockVisibility=false;
                return;
            }
            UserAlreadyExistTextBlockVisibility = false;
            IsValidEmailIdTextBlockVisibility = false;
            await _dataHandler.CreateNewUser(UserName.Trim(), EmailIdText.Trim().ToLower(),SelectedIndex).ConfigureAwait(false);
            await  ShowSignUpSuccessFullMessageBoxAsync();
             
        }
        private async  Task ShowSignUpSuccessFullMessageBoxAsync()
        {
            IUICommand msgBoxResult;



            await UiService.RunOnUiThread(
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
    }
}
