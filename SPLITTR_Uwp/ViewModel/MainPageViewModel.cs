using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.VmLogic;
using SPLITTR_Uwp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using SPLITTR_Uwp.Core.UseCase.GetUserExpenses;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel
{
    internal class MainPageViewModel : ObservableObject, IMainPageViewModel, IViewModel
    {


        private readonly IMainView _mainPage;
        private readonly IExpenseGrouper _expenseGrouper;
        private readonly IStateService _stateService;
        private string _userInitial;
        private bool _isUpdateWalletBalanceTeachingTipOpen;


        public MainPageViewModel(IMainView mainPage, IExpenseGrouper expenseGrouper, IStateService stateService)
        {

            _mainPage = mainPage;
            _expenseGrouper = expenseGrouper;
            _stateService = stateService;
            UserVobj = new UserVobj(Store.CurreUserBobj);

        }

        public string UserInitial
        {
            get => UserVobj.UserName.GetUserInitial();
            set => SetProperty(ref _userInitial, value);
        }

        public UserVobj UserVobj { get; }

        public bool IsUpdateWalletBalanceTeachingTipOpen
        {
            get => _isUpdateWalletBalanceTeachingTipOpen;
            set => SetProperty(ref _isUpdateWalletBalanceTeachingTipOpen, value);
        }

        public ObservableCollection<GroupBobj> UserGroups { get; } = new ObservableCollection<GroupBobj>();

        public ObservableCollection<User> RelatedUsers { get; } = new ObservableCollection<User>();

        public void ViewLoaded()
        {

            if (Store.CurreUserBobj == null)
            {
                //Setting frame reference to Default windows Frame
                NavigationService.Frame = null;
                NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());
                return;
            }

            //Calling UseCase For Fetching Current USer Groups
            FetchUserGroups();

            CallUseCaseToFetchCurrentUserExpenses();

            //Listening For Group Creation Notification
            SplittrNotification.GroupCreated += SplittrNotification_GroupCreated;

            //Listening For New ExpensesSplit
            SplittrNotification.ExpensesSplited += SplittrNotification_ExpensesSplited; ;

        }

        private void SplittrNotification_ExpensesSplited(ExpenseSplittedEventArgs obj)
        {
           CallUseCaseToFetchCurrentUserExpenses();
        }

        private void CallUseCaseToFetchCurrentUserExpenses()
        {
            var getExpensesRequestObj = new GetExpensesByIdRequest(CancellationToken.None, new MainViewVmPresenterCb(this), Store.CurreUserBobj);

            var getExpenseUseCase = InstanceBuilder.CreateInstance<GetExpensesByUserId>(getExpensesRequestObj);

            getExpenseUseCase.Execute();
        }

        private async void SplittrNotification_GroupCreated(GroupCreatedEventArgs obj)
        { 
            //Adding New Ly Created Group To Navigation View
           await RunOnUiThread((() =>
           {
               if (obj?.CreatedGroup is not null)
               {
                   UserGroups.Add(obj.CreatedGroup);
               }
           })).ConfigureAwait(false);
           
        }
        public void ViewDisposed()
        {
            //Unsubscribing For Group Creation Notification
            SplittrNotification.GroupCreated -= SplittrNotification_GroupCreated;

            SplittrNotification.ExpensesSplited -= SplittrNotification_ExpensesSplited;

        }


        private void FetchUserGroups()
        {
            var getUSerGroupReqObj = new GetUserGroupReq(CancellationToken.None, new MainViewVmPresenterCb(this), Store.CurreUserBobj);

            var getGroupsUseCase = InstanceBuilder.CreateInstance<GetUserGroups>(getUSerGroupReqObj);

            getGroupsUseCase.Execute();
        }


        private void PopulateIndividualSplitUsers(IEnumerable<ExpenseBobj> currentUserExpenses)
        {
            RelatedUsers.Clear();
            foreach (var expense in currentUserExpenses)
            {
                // Adds user object to Individual Split users  list Excluding Current User 
                if (expense.GroupUniqueId is not null)
                {
                    continue;
                }
                if (!expense.SplitRaisedOwner.Equals(Store.CurreUserBobj) && !RelatedUsers.Contains(expense.SplitRaisedOwner))
                {
                    RelatedUsers.Add(expense.SplitRaisedOwner);
                }
                if (!expense.CorrespondingUserObj.Equals(Store.CurreUserBobj) && !RelatedUsers.Contains(expense.CorrespondingUserObj))
                {
                    RelatedUsers.Add(expense.CorrespondingUserObj);
                }
            }

        }


        #region NavigationLogicRegion


        private bool _paneVisibility = true;

        public bool PaneVisibility
        {
            get => _paneVisibility;
            set => SetProperty(ref _paneVisibility, value);
        }

        public void LogOutButtonClicked()
        {
            _stateService.RequestUserLogout();

        }

        public void PersonProfileClicked()
        {
            PaneVisibility = false;
            NavigationService.Frame = _mainPage.ChildFrame;
            NavigationService.Navigate<UserProfilePage>();
        }

        public void AddButtonItemSelected(object sender, RoutedEventArgs e)
        {

            //closing main Page pane
            PaneVisibility = false;

            var selectedItem = sender as MenuFlyoutItem;
            var title = selectedItem.Text;
            NavigationService.Frame = _mainPage.ChildFrame;
            NavigationService.Navigate(string.Compare(title, "Add Exepense", StringComparison.InvariantCultureIgnoreCase) == 0 ?
                typeof(AddExpenseTestPage) :
                typeof(GroupCreationPage), new DrillInNavigationTransitionInfo());
        }

        #endregion



        public void AddWalletBalanceButtonClicked()
        {
            IsUpdateWalletBalanceTeachingTipOpen = !IsUpdateWalletBalanceTeachingTipOpen;
        }

        public event Action BindingUpdateInvoked;



        class MainViewVmPresenterCb : IPresenterCallBack<GetUserGroupResponse>,IPresenterCallBack<GetExpensesByIdResponse>
        {
            private readonly MainPageViewModel _viewModel;

            public MainViewVmPresenterCb(MainPageViewModel viewModel)
            {
                this._viewModel = viewModel;

            }
            public async void OnSuccess(GetUserGroupResponse result)
            {
                await RunOnUiThread((() =>
                {
                    _viewModel.UserGroups.ClearAndAdd(result.UserParticipatingGroups);

                })).ConfigureAwait(false);
            }
            public async void OnSuccess(GetExpensesByIdResponse result)
            {
                if (result == null)
                {
                    return;
                }
                await RunOnUiThread((() =>
                {
                 _viewModel.PopulateIndividualSplitUsers(result.CurrentUserExpenses);
                })).ConfigureAwait(false);
            }
            public void OnError(SplittrException ex)
            {
                ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}
