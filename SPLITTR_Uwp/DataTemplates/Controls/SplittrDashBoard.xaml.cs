using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;
using SPLITTR_Uwp.Services;
using static SPLITTR_Uwp.ViewModel.UserProfilePageViewModel;
using System.Threading;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.VmLogic;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class SplittrDashBoard : UserControl
    {
        private SplittrDashBoardVm _viewModel;
        public SplittrDashBoard()
        {
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<SplittrDashBoardVm>(App.Container);
            _viewModel.BindingUpdateInvoked += ViewModel_BindingUpdateInvoked;

            this.InitializeComponent();
            Loaded += SplittrDashBoard_Loaded;
            Unloaded += SplittrDashBoard_Unloaded;

        }

        private void ViewModel_BindingUpdateInvoked()
        {
           Bindings.Update();
        }

        private void SplittrDashBoard_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer is not null && _timer.IsEnabled)
            {
                _timer.Stop(); 
            }
            
        }

        private DispatcherTimer _timer;

        private void SplittrDashBoard_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.OnViewLoaded();
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (o, o1) => TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            _timer.Start();

        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrencyPreferrenceSplitButton.Flyout.IsOpen)
            {
                CurrencyPreferrenceSplitButton.Flyout.Hide();
                _viewModel.SaveUserCredentials();
            }
        }

        private void UserNameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!UserNameTextBox.Text.Equals(Store.CurreUserBobj.UserName))
            {
                _viewModel.SaveUserCredentials();
            }
        }
        private void DashBoardLogOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.OnLogoutRequested();
        }
    }

    public class SplittrDashBoardVm : ViewModelBase
    {
        private readonly IStateService _stateService;
        private string _userCurrencyPreference;
        private string _currentUserName;

        public UserViewModel UserVm { get; }

        public string UserCurrencyPreference
        {
            get => _userCurrencyPreference;
            set => SetProperty(ref _userCurrencyPreference, value);
        }

        public string CurrentUserName
        {
            get => _currentUserName;
            set => SetProperty(ref _currentUserName, value);
        }

        public int PreferredCurrencyIndex { get; set; }

        public List<string> CurrencyList { get; } = new List<string>()
        {
            "Rupees ₹", "Dollar $", "Euro   €", "Yen    ¥"
        };

        public SplittrDashBoardVm(IStateService stateService)
        {
            _stateService = stateService;
            UserVm = new UserViewModel(Store.CurreUserBobj);
            UserVm.PropertyChanged += UserVm_PropertyChanged;
        }


        private void UserVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Store.CurreUserBobj.CurrencyPreference)))
            {
                InvokeBindingsUpdate();
            }
        }

        public void OnViewLoaded()
        {
            UserCurrencyPreference = Store.CurreUserBobj.CurrencyPreference.ToString();
            _isVmChanged = true;
            CurrentUserName = Store.CurreUserBobj.UserName;
            PreferredCurrencyIndex = UserVm.CurrencyPreference;
            _isVmChanged = false;
        }

        private bool _isVmChanged;
        public void SaveUserCredentials()
        {
            if (_isVmChanged) // No Call to USe Case if The Property value is Set during Loading or Some Vm Logic
            {
                return;
            }

            var cts = new CancellationTokenSource().Token;
            //useCase classes is updating the UserObj and its related data's
            var updateUserRequestOBj = new UpdateUserRequestObj(cts, new SplittrDashBoardPresenterCallBack(this), Store.CurreUserBobj, _currentUserName.Trim(), (Currency)PreferredCurrencyIndex);

            var updateUserUseCaseObj = InstanceBuilder.CreateInstance<UpdateUser>(updateUserRequestOBj);

            updateUserUseCaseObj.Execute();

        }
        public void OnLogoutRequested()
        {
            _stateService.RequestUserLogout();
        }

        private class  SplittrDashBoardPresenterCallBack : IPresenterCallBack<UpdateUserResponseObj>
        {
            private readonly SplittrDashBoardVm _viewModel;
            public SplittrDashBoardPresenterCallBack(SplittrDashBoardVm viewModel)
            {
                _viewModel = viewModel;
            }
            public async void OnSuccess(UpdateUserResponseObj result)
            {
               await UiService.RunOnUiThread((() =>
               {
                   _viewModel.CurrentUserName = result.UpdatedUserBobj.UserName;
                   _viewModel.UserCurrencyPreference = result.UpdatedUserBobj.CurrencyPreference.ToString();
               })).ConfigureAwait(false);
            }
            public async void OnError(SplittrException ex)
            {
                switch (ex.InnerException)
                {
                    case UserNameInvalidException:
                        await UiService.RunOnUiThread(() =>
                        {
                            _viewModel.PreferredCurrencyIndex = Store.CurreUserBobj.CurrencyIndex;
                            _viewModel.CurrentUserName = Store.CurreUserBobj.UserName;
                            _viewModel.PreferredCurrencyIndex = Store.CurreUserBobj.CurrencyIndex;
                        }).ConfigureAwait(false);
                            ExceptionHandlerService.HandleException(ex.InnerException);
                        break;
                    case SqlException:
                        //Sql lite Error Handling Code
                        break;
                }

            }
        }

    }
}
