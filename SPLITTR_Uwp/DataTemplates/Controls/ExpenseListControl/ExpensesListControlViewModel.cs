using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserExpenses;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.VmLogic;
using SPLITTR_Uwp.ViewModel.Vobj;
using SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject;

namespace SPLITTR_Uwp.DataTemplates.Controls.ExpenseListControl;

public class ExpensesListControlViewModel :ObservableObject
{

    #region BindableProperties

    private readonly IExpenseListControl _view;
    private  IExpenseGrouper _expenseGrouper;
    private string _titleText;

    public ObservableCollection<ExpenseVobj> DateSortedExpenseList { get; } = new ObservableCollection<ExpenseVobj>();

    public ObservableCollection<ExpenseGroupingList> GroupedExpenses { get; } = new ObservableCollection<ExpenseGroupingList>();

    private List<ExpenseVobj> _userExpensesCache;

    public string TitleText
    {
        get => _titleText;
        set => SetProperty(ref _titleText, value);
    }
    public ExpenseVobj SelectedExpenseObj { get; set; }

    /// <exception cref="ArgumentNullException" accessor="set">Thrown when the arguments are <see langword="null"/></exception>
    public IExpenseGrouper ExpenseGrouper
    {
        get => _expenseGrouper;
        set
        {
            _expenseGrouper = value ?? throw new ArgumentNullException();
            FilterExpensesToBeShown(_preferedFilter);
        }
    }
    
    #endregion

    public ExpensesListControlViewModel(IExpenseListControl view, IExpenseGrouper expenseGrouper)
    {
        _view = view;
        _expenseGrouper = expenseGrouper;

        SplittrNotification.ExpensesSplitted += SplittrNotification_ExpensesSplitted;
        SplittrNotification.ExpenseStatusChanged += SplittrNotification_ExpenseStatusChanged;
    }


    #region SortingAndGroupingRequest

    public void SortExpenseBasedOnDate(ObservableCollection<ExpenseGroupingList> groupedExpenses)
    {
        DateSortedExpenseList.Clear();
        var expensesToBeSorted = new List<ExpenseVobj>();
        foreach (var expenses in groupedExpenses)
        {
            expensesToBeSorted.AddRange(expenses);
        }
        var expenseArray = expensesToBeSorted.ToArray();
        Array.Sort(expenseArray, new ExpenseDateSorter());

        foreach (var expense in expenseArray)
        {
            DateSortedExpenseList.Add(expense);
        }
    }

    private ExpenseListFilterObj _preferedFilter;

    public void FilterExpensesToBeShown(ExpenseListFilterObj filterObj)
    {
        if (filterObj == null)
        {
            CallUseCaseToFetchCurrentUserExpense();
            return;
        }
        _preferedFilter = filterObj;
        switch (filterObj.FilterType)
        {
            case ExpenseListFilterObj.ExpenseFilter.RequestByMe:
                PopulateUserRaisedExpenses();
                break;
            case ExpenseListFilterObj.ExpenseFilter.RequestToMe:
                PopulateUserReceivedExpenses();
                break;
            case ExpenseListFilterObj.ExpenseFilter.AllExpenses:
                CallUseCaseToFetchCurrentUserExpense();
                break;
            case ExpenseListFilterObj.ExpenseFilter.GroupExpense:
                PopulateSpecificGroupExpenses(filterObj.Group);
                break;
            case ExpenseListFilterObj.ExpenseFilter.UserExpense:
                PopulateUserRelatedExpenses(filterObj.User);
                break;
            
        }

        //After Grouping Expenses Based on Request Also Populating Date Sorted ListView Based On Newly Grouped Expenses
        SortExpenseBasedOnDate(GroupedExpenses);
        //Assign Icon Source For ExpenseVobjs
        AssignIconSourceToExpenseVobjs();
    }
        
    #endregion

    #region NotificationListering

    private async void SplittrNotification_ExpensesSplitted(ExpenseSplittedEventArgs obj)
    {
        if (obj?.NewExpenses is null)
        {
            return;
        }
        //Populating Existing Cache
        _userExpensesCache?.AddRange(obj.NewExpenses.Select(ex => new ExpenseVobj(ex)));

        await UiService.RunOnUiThread(() =>
        {
            //grouping and Populating Newly added expenses Groups
            FilterExpensesToBeShown(_preferedFilter);

        }).ConfigureAwait(false);
    }

    private async void SplittrNotification_ExpenseStatusChanged(ExpenseStatusChangedEventArgs obj)
    {
        await UiService.RunOnUiThread(() =>
        {
            //grouping and Populating Newly added expenses Groups
            FilterExpensesToBeShown(_preferedFilter);

        }).ConfigureAwait(false);
    }

    #endregion


    public void ViewDisposed()
    {
        SplittrNotification.ExpensesSplitted -= SplittrNotification_ExpensesSplitted;
        SplittrNotification.ExpenseStatusChanged -= SplittrNotification_ExpenseStatusChanged;
        Store.CategoriesLoaded -= OnAssignCategoryLoaded;
    }

    public void OnExpenseListControlLoaded()
    {
        FilterExpensesToBeShown(_preferedFilter);
    }
    private void CallUseCaseToFetchCurrentUserExpense()
    {
        var getUserExpenses = new GetExpensesByIdRequest(CancellationToken.None, new ExpenseListVmPresenterCb(this), Store.CurrentUserBobj);

        var getUserExpensesUseCase = InstanceBuilder.CreateInstance<GetExpensesByUserId>(getUserExpenses);

        getUserExpensesUseCase.Execute();
    }
    private void PopuLateExpenses(IEnumerable<ExpenseVobj> currentUserExpenses)
    {
        GroupedExpenses.Clear();

        TitleText = "All Expenses";

        GroupingAndPopulateExpensesList(FilterCurrentUserExpense(currentUserExpenses));

        _view.SetCollectionListSource(GroupedExpenses);

        IEnumerable<ExpenseVobj> FilterCurrentUserExpense(IEnumerable<ExpenseVobj> expenses)
        {
            return expenses.Where(IsNotOwnerExpense);
        }
    }

    #region GroupingLogicRegion

    private ObservableCollection<ExpenseGroupingList> _previousGroupedCollection;

    private void GroupingAndPopulateExpensesList(IEnumerable<ExpenseVobj> filteredExpenses)
    {
        if (filteredExpenses == null)
        {
            return;
        }
        TryClearingPreviousSubscription();
        var groups = _expenseGrouper.CreateExpenseGroupList(filteredExpenses);

        if (groups is ObservableCollection<ExpenseGroupingList> observableGroups)
        {
            observableGroups.CollectionChanged += GroupedCollectionOnChange;
            _previousGroupedCollection = observableGroups;
        }

        GroupedExpenses.Clear();
        foreach (var group in groups)
        {
            GroupedExpenses.Add(group);
        }


        void TryClearingPreviousSubscription()
        {
            if (_previousGroupedCollection != null)
            {
                _previousGroupedCollection.CollectionChanged -= GroupedCollectionOnChange;
            }
        }

    }
    private void GroupedCollectionOnChange(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var newGroup in e.NewItems)
            {
                if (newGroup is ExpenseGroupingList newAddedGroup)
                {
                    GroupedExpenses.Add(newAddedGroup);
                }
            }

        }
        if (e.OldItems is not null)
        {
            foreach (var oldGroups in e.OldItems)
            {
                if (oldGroups is ExpenseGroupingList oldRemovedGroup)
                {
                    GroupedExpenses.Remove(oldRemovedGroup);
                }
            }
        }
       
    }

    public void PopulateSpecificGroupExpenses(Group selectedGroup)
    {
        if (selectedGroup is null)
        {
            return;
        }
        GroupedExpenses.Clear();
        //filter expenses based on particular group 
        var groupSpecificExpenses = _userExpensesCache?.Where(ex => IsSelectedGroupExpense(ex,selectedGroup) && IsNotOwnerExpense(ex));

        GroupingAndPopulateExpensesList(groupSpecificExpenses);

        //Setting ExpenseControl Title
        TitleText = selectedGroup?.GroupName + " Group Expenses";


        //Checks Whether Expense belongs to that particular group
        bool IsSelectedGroupExpense(ExpenseBobj expense, Group selectedGroup)
        {
            return expense.GroupUniqueId is not null && expense.GroupUniqueId.Equals(selectedGroup.GroupUniqueId);
        }


    }

    /// <summary>
    /// returns True if corresponding expense obj points to present user 
    /// </summary>
    /// <param name="expense"></param>
    /// <returns></returns>
    private bool IsNotOwnerExpense(ExpenseBobj expense)
    {
        return !expense.CorrespondingUserObj.Equals(expense.SplitRaisedOwner);
    }

    public void PopulateUserRelatedExpenses(User selectedUser)
    {
        if (selectedUser is null)
        {
            return;
        }
        GroupedExpenses.Clear();
        var userSpecificExpenses = _userExpensesCache.Where(CheckExpenseMatchesUser);

        GroupingAndPopulateExpensesList(userSpecificExpenses);


        //filter expenses based on Related User
        bool CheckExpenseMatchesUser(ExpenseBobj e)
        {
            if (e.SplitRaisedOwner.Equals(selectedUser) || e.CorrespondingUserObj.Equals(selectedUser) && e.GroupUniqueId is null)
                return true;
            return false;
        }

        //Setting ExpenseControl Title
        TitleText = selectedUser.UserName + " Expenses";

    }
    public void PopulateUserReceivedExpenses()
    {
        GroupedExpenses.Clear();

        var userReceivedExpenses = _userExpensesCache?.Where(ex => IsUserRecievedExpense(ex) && IsNotOwnerExpense(ex));

        GroupingAndPopulateExpensesList(userReceivedExpenses);

        //Setting ExpenseControl Title
        TitleText = "Request To Me";


        bool IsUserRecievedExpense(ExpenseBobj expense)
        {
            return !expense.SplitRaisedOwner.Equals(Store.CurrentUserBobj);
        }
    }


    public void PopulateUserRaisedExpenses()
    {
        GroupedExpenses.Clear();

        var userRaisedExpenses = _userExpensesCache?.Where(ex => IsUserRaisedExpense(ex) && IsNotOwnerExpense(ex));

        GroupingAndPopulateExpensesList(userRaisedExpenses);
        //Setting ExpenseControl Title
        TitleText = "Request By Me";

        bool IsUserRaisedExpense(ExpenseBobj expense)
        {
            return expense.SplitRaisedOwner.Equals(Store.CurrentUserBobj);
        }
    }
    #endregion

    #region IconLogicApplyRegion
    private void AssignIconSourceToExpenseVobjs()
    {
        if (!Store.Categories.Any())
        {
            Store.CategoriesLoaded += OnAssignCategoryLoaded;
            return;
        }
        AssignRespectiveIcons(Store.CategoryOnlyDictionary);
    }
    private void OnAssignCategoryLoaded(CategoryLoadedEventArgs obj)
    {
        AssignRespectiveIcons(obj.ExpenseCategoriesDict);
    }
    private void AssignRespectiveIcons(IReadOnlyDictionary<int,ExpenseCategory> categoryDictionary)
    {
        //Assigning RespectiveIcon For grouped Expenses and DateSortedExpenseList
        foreach (var group in GroupedExpenses)
        {
            group.Select(ex => AssignRespectiveCategoryIcon(ex,categoryDictionary));
        }
        //Assigning Respective Icon To Date Sorted Expense List
        foreach (var expense in DateSortedExpenseList)
        {
            AssignRespectiveCategoryIcon(expense,categoryDictionary);
        }
    }
    private string AssignRespectiveCategoryIcon(ExpenseVobj ex, IReadOnlyDictionary<int, ExpenseCategory> categoryDictionary)
    {
        var respectedCategory = categoryDictionary[ex.CategoryId];
        ex.CategoryName = respectedCategory?.Name;
        return ex.IconSource = respectedCategory?.Icon;
    }

    #endregion

    #region PresenterCallBack

    private class ExpenseListVmPresenterCb : IPresenterCallBack<GetExpensesByIdResponse>
    {
        private readonly ExpensesListControlViewModel _viewModel;

        public ExpenseListVmPresenterCb(ExpensesListControlViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        public async void OnSuccess(GetExpensesByIdResponse result)
        {
            if (result == null)
            {
                return;
            }
            _viewModel._userExpensesCache = result.CurrentUserExpenses.Select(ex => new ExpenseVobj(ex)).ToList();
            await UiService.RunOnUiThread(() =>
            {
                _viewModel.PopuLateExpenses(_viewModel._userExpensesCache);
                _viewModel.SortExpenseBasedOnDate(_viewModel.GroupedExpenses);
                _viewModel.AssignIconSourceToExpenseVobjs();
            }).ConfigureAwait(false);
        }
        public void OnError(SplittrException ex)
        {
            ExceptionHandlerService.HandleException(ex);
        }
    }


    #endregion


}
