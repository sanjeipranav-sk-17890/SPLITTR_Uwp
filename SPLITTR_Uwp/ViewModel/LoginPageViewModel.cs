using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Controls;
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
        private readonly DataStore _store;
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

        public LoginPageViewModel(IUserDataHandler userDataHandler,DataStore store)
        {
            _userDataHandler = userDataHandler;
            _store = store;


            //Firing a event for every 0.5 seconds
            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += _timer_Tick;
            timer.Start();
            
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
                _store.UserBobj = await _userDataHandler.FetchCurrentUserDetails(UserEmailIdTextBox.Trim().ToLower());
                 NavigationService.Navigate<MainPage>();
            }


        }
        public void SignUpButtonOnClick()
        {
            NavigationService.Frame.Navigate(typeof(SignUpPage),null, infoOverride:new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        
    }
}
