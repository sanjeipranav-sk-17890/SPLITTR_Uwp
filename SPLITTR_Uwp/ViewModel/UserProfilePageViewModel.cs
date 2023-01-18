using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;

namespace SPLITTR_Uwp.ViewModel
{
    public class UserProfilePageViewModel : ObservableObject,IViewModel
    {

        
        IUserUtility _userUtility;
        public UserViewModel User { get; }


        public UserProfilePageViewModel(IUserUtility userUtility)
        {
            _userUtility = userUtility;
            User = new UserViewModel(Store.CurreUserBobj);
            Store.CurreUserBobj.ValueChanged += UserBobj_ValueChanged;

        }

        private async void UserBobj_ValueChanged(string property)
        {
            await UiService.RunOnUiThread((() =>
            {
                BindingUpdateInvoked?.Invoke();

            }));
        }



        public string UserInitial
        {
            get
            {
                return User.UserName.GetUserInitial();
            }
        }

        public string CurrencyPreference
        {
            get
            {
                return Store.CurreUserBobj?.CurrencyPreference.ToString();
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

        private bool _isEditUserProfileVisible = false;

        public void EditUserProfileClicked()
        {
            IsEditUserProfileVisible = true;
        }

        private string _currentUserName = String.Empty;
        public string CurrentUserName
        {
            get
            {
                _currentUserName = User.UserName;
                return _currentUserName;
            }
            set => _currentUserName = value;
        }

        private int _preferedCurrencyIndex;
        private bool _isUserNameEmptyIndicatorVisible = false;

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
        public  void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_currentUserName.Trim()))
            {
                IsUserNameEmptyIndicatorVisible = true;
                return;
            }
            IsUserNameEmptyIndicatorVisible = false;
            //utility classes is updating the UserObj and its related data's
             _userUtility.UpdateUserObjAsync(Store.CurreUserBobj, _currentUserName, (Currency)_preferedCurrencyIndex,(async () =>
            {
                await ShowSignUpSuccessFullMessageBoxAsync();

            }));

            //showing Update successfull messagebox

            IsEditUserProfileVisible = false;

        }
        private Task ShowSignUpSuccessFullMessageBoxAsync()
        {

            return UiService.ShowContentAsync("Account Updated SuccessFully", "SuccessFull!!");

        }

        public event Action BindingUpdateInvoked;
    }
}
