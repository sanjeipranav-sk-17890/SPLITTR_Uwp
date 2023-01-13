using System;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;
using Windows.UI.Xaml.Media.Animation;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.DataRepository;

namespace SPLITTR_Uwp.ViewModel
{
    public class LoginPageViewModel :ObservableObject
    {
        private readonly IUserDataHandler _userDataHandler;
        private  int _selectedItem = 0;
        private  bool _loginInformationTextBox=false;
        private  bool _wrongUserCredentialTextBlockVisibility=false;

        public int SelectedItem
        {
            get => _selectedItem;

            set => SetProperty(ref  _selectedItem,value);
        }


        public bool LoginInformationTextBox
        {
            get => _loginInformationTextBox;
            set => SetProperty(ref _loginInformationTextBox, value);
        }

        public bool WrongUserCreDentialTextBlockVisibility
        {
            get=> _wrongUserCredentialTextBlockVisibility;
            set => SetProperty(ref _wrongUserCredentialTextBlockVisibility, value);
        }

        public string UserEmailIdTextBox { get; set; }

        private DispatcherTimer _timer;

        public LoginPageViewModel(IUserDataHandler userDataHandler)
        {
            _userDataHandler = userDataHandler;
            

            //Firing a event for every 0.5 seconds
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();
            
        }

        private int _change = 1;

        private void _timer_Tick(object sender, object e)
        {
            SelectedItem += _change;
            if (SelectedItem < 1 || SelectedItem > 2)
            {
                _change *= -1;
            }
        }

        public async void LoginButtonPressed()
        {
            //===============================================/
                        //By PAss To Be Deleted

                       // UserEmailIdTextBox = "sanjei.pranav@gmail.com";
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
                LoginInformationTextBox=false;
                WrongUserCreDentialTextBlockVisibility=true;
            }
            else
            {
                var isOldUser = await _userDataHandler.IsUserAlreadyExist(UserEmailIdTextBox.Trim().ToLower());
                if (isOldUser is not true)
                {
                    WrongUserCreDentialTextBlockVisibility = true;
                    return;
                }
                WrongUserCreDentialTextBlockVisibility = false;
                //To be deleted
                Store.CurreUserBobj = await _userDataHandler.FetchCurrentUserDetails(UserEmailIdTextBox.Trim().ToLower());
                 

                 NavigationService.Navigate<MainPage>();
            }


        }
        public void SignUpButtonOnClick()
        {
            NavigationService.Frame.Navigate(typeof(SignUpPage),null, infoOverride:new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }


        public void PageUnloaded()
        {
            //stoping main page animation if the the page is unloaded
            _timer.Stop();
        }
    }
}
