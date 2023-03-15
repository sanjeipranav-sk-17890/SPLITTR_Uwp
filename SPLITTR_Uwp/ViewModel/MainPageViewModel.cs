using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.UI.Xaml.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserExpenses;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Vobj;
using SPLITTR_Uwp.Views;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel;

internal class MainPageViewModel : ObservableObject, IMainPageViewModel
{

    private readonly IStateService _stateService;
    private string _userInitial;


    public MainPageViewModel(IStateService stateService)
    {
        _stateService = stateService;
        UserVobj = new UserVobj(Store.CurrentUserBobj);
    }

    public string UserInitial
    {
        get => UserVobj.UserName.GetUserInitial();
        set => SetProperty(ref _userInitial, value);
    }

    public UserVobj UserVobj { get; }

    public ObservableCollection<GroupBobj> UserGroups { get; } = new ObservableCollection<GroupBobj>();

    public ObservableCollection<User> RelatedUsers { get; } = new ObservableCollection<User>();

    public void ViewLoaded()
    {

        if (Store.CurrentUserBobj == null)
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
        SplittrNotification.ExpensesSplitted += SplittrNotificationExpensesSplitted; ;

    }

    private void SplittrNotificationExpensesSplitted(ExpenseSplittedEventArgs obj)
    {
        CallUseCaseToFetchCurrentUserExpenses();
    }

    private void CallUseCaseToFetchCurrentUserExpenses()
    {
        var getExpensesRequestObj = new GetExpensesByIdRequest(CancellationToken.None, new MainViewVmPresenterCb(this), Store.CurrentUserBobj);

        var getExpenseUseCase = InstanceBuilder.CreateInstance<GetExpensesByUserId>(getExpensesRequestObj);

        getExpenseUseCase.Execute();
    }

    private void SplittrNotification_GroupCreated(GroupCreatedEventArgs obj)
    {
        //Adding New Ly Created Group To Navigation View
        _ = RunOnUiThread(() =>
        {
            if (obj?.CreatedGroup is not null)
            {
                UserGroups.Add(obj.CreatedGroup);
            }
        }).ConfigureAwait(false);
           
    }
    public void ViewDisposed()
    {
        //Unsubscribing For Group Creation Notification
        SplittrNotification.GroupCreated -= SplittrNotification_GroupCreated;

        SplittrNotification.ExpensesSplitted -= SplittrNotificationExpensesSplitted;

    }


    private void FetchUserGroups()
    {
        var getUSerGroupReqObj = new GetUserGroupReq(CancellationToken.None, new MainViewVmPresenterCb(this), Store.CurrentUserBobj);

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
            if (!expense.SplitRaisedOwner.Equals(Store.CurrentUserBobj) && !RelatedUsers.Contains(expense.SplitRaisedOwner))
            {
                RelatedUsers.Add(expense.SplitRaisedOwner);
            }
            if (!expense.CorrespondingUserObj.Equals(Store.CurrentUserBobj) && !RelatedUsers.Contains(expense.CorrespondingUserObj))
            {
                RelatedUsers.Add(expense.CorrespondingUserObj);
            }
        }

    }

    public void LogOutRequested()
    {
        _stateService.RequestUserLogout();

    }



    private class MainViewVmPresenterCb : IPresenterCallBack<GetUserGroupResponse>,IPresenterCallBack<GetExpensesByIdResponse>
    {
        private readonly MainPageViewModel _viewModel;

        public MainViewVmPresenterCb(MainPageViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public void OnSuccess(GetUserGroupResponse result)
        {
            _ = RunOnUiThread(() =>
            {
                _viewModel.UserGroups.ClearAndAdd(result.UserParticipatingGroups);

            }).ConfigureAwait(false);
        }
        public void OnSuccess(GetExpensesByIdResponse result)
        {
            if (result == null)
            {
                return;
            }
            _ = RunOnUiThread(() =>
            {
                _viewModel.PopulateIndividualSplitUsers(result.CurrentUserExpenses);
            }).ConfigureAwait(false);
        }
        public void OnError(SplittrException ex)
        {
            ExceptionHandlerService.HandleException(ex);
        }
    }
}