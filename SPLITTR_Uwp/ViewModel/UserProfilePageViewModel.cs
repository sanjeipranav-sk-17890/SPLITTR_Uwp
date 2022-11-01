using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.Core.Utility.Blogic;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace SPLITTR_Uwp.ViewModel
{
    public class UserProfilePageViewModel : ObservableObject
    {
        
        private readonly DataStore _dataStore;
        private readonly IStringManipulator _manipulator;
        IUserUtility _userUtility;
        public UserViewModel User { get; }


        public UserProfilePageViewModel(DataStore dataStore,IStringManipulator manipulator, IUserUtility userUtility)
        {
            _dataStore = dataStore;
            _manipulator = manipulator;
            _userUtility = userUtility;
            User = new UserViewModel(_dataStore.UserBobj);
            _dataStore.UserBobj.ValueChanged += UserBobj_ValueChanged;

        }

        private async void UserBobj_ValueChanged()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    OnPropertyChanged(nameof(UserInitial));
                    OnPropertyChanged(nameof(CurrencyPreference));
                    OnPropertyChanged(nameof(CurrentUserName));

                });
        }



        public string UserInitial
        {
            get
            {
                return _manipulator.GetUserInitial(User.UserName);
            }
        }

        public string CurrencyPreference {
            get
            {
                return _dataStore.UserBobj?.CurrencyPreference.ToString();
            }

         }

        public bool IsEditUserProfileVisible
        {
            get => _isEditUserProfileVisible;
            set => SetProperty(ref _isEditUserProfileVisible, value);
        }

        public List<string> CurrencyList = new List<string>()
        {
            "Rupees ₹",
            "Dollar $",
            "Euro   €",
            "Yen    ¥"
        };

        private bool _isEditUserProfileVisible =false;

        public void EditUserProfileClicked()
        {
            IsEditUserProfileVisible= true;
        }

        private string _currentUserName=String.Empty;
        public string CurrentUserName
        {
            get
            {
                _currentUserName = User.UserName;
              return _currentUserName;
            } 
            set => _currentUserName= value;
        }

        private int _preferedCurrencyIndex;
        private bool _isUserNameEmptyIndicatorVisible=false;

        public int PreferendCurrencyIndex
        {
            get => User.CurrencyPreference;
            set => _preferedCurrencyIndex = value;
        }

        public bool IsUserNameEmptyIndicatorVisible
        {
            get => _isUserNameEmptyIndicatorVisible;
            set => SetProperty(ref _isUserNameEmptyIndicatorVisible, value);
        }

        public void CancelButtonClicked()
        {
            CurrentUserName = "";
            _preferedCurrencyIndex = -1;
            IsEditUserProfileVisible = false;
            IsUserNameEmptyIndicatorVisible = false;
        }
        public async void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_currentUserName.Trim()))
            {
                IsUserNameEmptyIndicatorVisible = true;
                return;
            }
            IsUserNameEmptyIndicatorVisible = false;
            //utility classes is updating the UserObj and its related data's
            await _userUtility.UpdateUserObjAsync(_dataStore.UserBobj, _currentUserName, (Currency)_preferedCurrencyIndex);
           
            //showing Update successfull messagebox
               await ShowSignUpSuccessFullMessageBoxAsync();

               IsEditUserProfileVisible = false;

        }
        private async Task ShowSignUpSuccessFullMessageBoxAsync()
        {

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog msg = new MessageDialog("Account Updated SuccessFully", "SuccessFull!!");

                    msg.Commands.Add(new UICommand("close"));
                    await msg.ShowAsync();
                    
                });

        }
    }
}
