using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;

namespace SPLITTR_Uwp.ViewModel
{
   

    public class UserProfilePageViewModel : ObservableObject,IViewModel
    {


        public UserVobj User { get; }


        public UserProfilePageViewModel()
        {
            User = new UserVobj(Store.CurreUserBobj);
            SplittrNotification.UserObjUpdated += SplittrNotification_UserObjUpdated;
        }

        private async void SplittrNotification_UserObjUpdated(UserBobjUpdatedEventArgs obj)
        {
            await UiService.RunOnUiThread((() =>
            {
                BindingUpdateInvoked?.Invoke();

            })).ConfigureAwait(false);
        }

        public void ViewDisposed()
        {
            SplittrNotification.UserObjUpdated -= SplittrNotification_UserObjUpdated;
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

        public List<string> _currencyList = new List<string>()
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
            get => User.CurrencyIndex;
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
        public  void SaveButtonClicked()
        {
            if (string.IsNullOrEmpty(_currentUserName.Trim()))
            {
                IsUserNameEmptyIndicatorVisible = true;
                return;
            }
            IsUserNameEmptyIndicatorVisible = false;
            var cts = new CancellationTokenSource().Token;
            //useCase classes is updating the UserObj and its related data's
            var updateUserRequestOBj = new UpdateUserRequestObj(cts,new UserProfileVmPresenterCallBack(this),Store.CurreUserBobj,_currentUserName,(Currency)_preferedCurrencyIndex);

            var updateUserUseCaseObj = InstanceBuilder.CreateInstance<UpdateUser>(updateUserRequestOBj);

            updateUserUseCaseObj.Execute();


        }
        
        public event Action BindingUpdateInvoked;

        public async void InvokeOnUserObjectUpdated(UpdateUserResponseObj result)
        { 

           await UiService.ShowContentAsync("Account Updated SuccessFully", "SuccessFull!!").ConfigureAwait(false);
           await UiService.RunOnUiThread(() =>
           {
               //showing Update successFull messageBox
               IsEditUserProfileVisible = false;
           }).ConfigureAwait(false);
        }
        public class UserProfileVmPresenterCallBack : IPresenterCallBack<UpdateUserResponseObj>
        {
            private readonly UserProfilePageViewModel _viewModel;
            public UserProfileVmPresenterCallBack(UserProfilePageViewModel viewModel)
            {
                _viewModel = viewModel;

            }
            public void OnSuccess(UpdateUserResponseObj result)
            {
                _viewModel.InvokeOnUserObjectUpdated(result);
            }
            public void OnError(SplittrException ex)
            {
                if (ex.InnerException is SqlException)
                {
                    ExceptionHandlerService.HandleException(ex);
                }
            }
        }
    }
}
