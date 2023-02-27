using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserExpenses;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;
using SPLITTR_Uwp.ViewModel.VmLogic;
using static SPLITTR_Uwp.Services.UiService;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class UserExpensesListControl : UserControl,IExpenseListControl
    {
        private ExpensesListControlViewModel _viewModel;

        private static UserExpensesListControl ListControl { get; set; }
        public UserExpensesListControl()
        {
            InitializeComponent();
            _viewModel = ActivatorUtilities.CreateInstance<ExpensesListControlViewModel>(App.Container, this);
            Loaded += UserExpensesListControl_Loaded;
            ListControl = this;
            Unloaded += ((sender, args) => _viewModel.ViewDisposed());
        }

        private void UserExpensesListControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.OnExpenseListControlLoaded();
        }

      
        public static void FilterExpense(ExpenseListFilterObj requestObj)
        {
            if (ListControl is null)
            {
                return;
            }
            switch (requestObj.FilterType)
            {
                case ExpenseListFilterObj.ExpenseFilter.RequestByMe:
                    ListControl._viewModel.PopulateUserRaisedExpenses();
                    break;
                case ExpenseListFilterObj.ExpenseFilter.RequestToMe:
                    ListControl._viewModel.PopulateUserReceivedExpenses();
                    break;
                case ExpenseListFilterObj.ExpenseFilter.AllExpenses:
                    ListControl._viewModel.OnExpenseListControlLoaded();
                    break;
                case ExpenseListFilterObj.ExpenseFilter.GroupExpense:
                    ListControl._viewModel.PopulateSpecificGroupExpenses(requestObj.Group);
                    break;
                case ExpenseListFilterObj.ExpenseFilter.UserExpense:
                    ListControl._viewModel.PopulateUserRelatedExpenses(requestObj.User);
                    break;
            }
        }

        
        public event Action PaneButtonOnClick;
        private void NavigationPaneName_OnClick(object sender, RoutedEventArgs e)
        {
            //Changing Appearance of pane button and invoking click event 
            if (PaneButtonStateGroup.CurrentState is not null && PaneButtonStateGroup.CurrentState.Name == nameof(OpenPaneState))
            {
                VisualStateManager.GoToState(this, nameof(ClosePaneState), true);
                PaneButtonOnClick?.Invoke();
                return;
            }
            VisualStateManager.GoToState(this, nameof(OpenPaneState), true);
            PaneButtonOnClick?.Invoke();
        }


        public event Action<object, SelectionChangedEventArgs> OnExpenseSelectionChanged;
        private void ExpensesLIstView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnExpenseSelectionChanged?.Invoke(sender,e);
        }

        //Changing selected  Groups Visibility 
        private void ExpenseShowButton_OnClick(object sender, RoutedEventArgs e)
        {
            //if button rotation is 90 set it to 0 and vice versa
            if (sender is not Button sideButton)
            {
                return;
            }

            if (sideButton.DataContext is not ExpenseGroupingList expenseGroup)
            {
                return;
            }
            var expenseVisibility = true;

            foreach (var expense in expenseGroup)
            {
                if (expense is not ExpenseVobj expenseVm)
                {
                    continue;
                }
                expenseVm.Visibility = !expenseVm.Visibility;
                expenseVisibility = expenseVm.Visibility;
            }
            AssignButtonStyleBasedOnVisibility(expenseVisibility, sideButton);


        }


        private void AssignButtonStyleBasedOnVisibility(bool expenseVisibility, Button sideButton)
        {
            if (expenseVisibility)
            {

                var style = ShowExpenseButtonStyle;
                sideButton.Style = style;
            }
            else
            {
                var style = HideExpenseButtonStyle;
                sideButton.Style = style;
            }
        }

        
        public void SetCollectionListSource(IEnumerable Source)
        {
            // Add Csv Source for newValue.CollectionChanged 
            if (Source is not ObservableCollection<ExpenseGroupingList> newSource)
            {
                return;
            }
            CollectionViewSource.Source = newSource;
            ExpensesLIstView.ItemsSource = CollectionViewSource.View;

        }

        private void SortingFlyoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem selectedItem)
            {
                return;
            }
            //if Showing list view is Date Time sorted then No Need to change list view 
            if (selectedItem == DateMenuItem && (DateSortedList.Visibility != Visibility.Visible))
            {
                //Sorts Expenses Bases On Date and Add it to Observable Collection
                   _viewModel.SortExpenseBasedOnDate(_viewModel.GroupedExpenses);

                //Setting visibility of dateListview and Hiding Grouped Listview
                ExpensesLIstView.Visibility = Visibility.Collapsed;
                DateSortedList.Visibility = Visibility.Visible;

            }
            if (selectedItem == StatusMenuItem && (ExpensesLIstView.Visibility != Visibility.Visible))
            {

                ExpensesLIstView.Visibility = Visibility.Visible;
                DateSortedList.Visibility = Visibility.Collapsed;

            }

        }
    }

    /// <summary>
    /// Encapsulate Request obj To used by UserList Expense Control Parent 
    /// </summary>
    public class ExpenseListFilterObj
    {
        public ExpenseListFilterObj(Group group1, User user, ExpenseFilter filterType)
        {
            Group = group1;
            User = user;
            FilterType = filterType;
        }

        public Group Group { get; }

        public User User { get; }

        public ExpenseFilter FilterType { get; }
       public enum ExpenseFilter
        {
            RequestByMe,
            RequestToMe,
            AllExpenses,
            GroupExpense,
            UserExpense,
        }
    }
    
    public interface IExpenseListControl
    {
        void SetCollectionListSource(IEnumerable Source);

    }

    public class ExpensesListControlViewModel :ObservableObject,IViewModel
    {
        private readonly IExpenseListControl _view;
        private readonly IExpenseGrouper _expenseGrouper;
        private string _titleText;

        public ObservableCollection<ExpenseBobj> DateSortedExpenseList { get; } = new ObservableCollection<ExpenseBobj>();

        public ObservableCollection<ExpenseGroupingList> GroupedExpenses { get; } = new ObservableCollection<ExpenseGroupingList>();

        private List<ExpenseBobj> _userExpensesCache;

        public void SortExpenseBasedOnDate(ObservableCollection<ExpenseGroupingList> groupedExpenses)
        {
            DateSortedExpenseList.Clear();
            var expensesToBeSorted = new List<ExpenseBobj>();
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

        public ExpenseVobj SelectedExpenseObj { get; set; }

        public string TitleText
        {
            get => _titleText;
            set => SetProperty(ref _titleText, value);
        }



        public ExpensesListControlViewModel(IExpenseListControl view, IExpenseGrouper expenseGrouper)
        {
            _view = view;
            _expenseGrouper = expenseGrouper;

            SplittrNotification.ExpensesSplited += SplittrNotification_ExpensesSplited;
            SplittrNotification.ExpenseStatusChanged += SplittrNotification_ExpenseStatusChanged;
        }

        private async void SplittrNotification_ExpenseStatusChanged(ExpenseStatusChangedEventArgs obj)
        {
            await RunOnUiThread((() =>
            {
                //grouping and Populating Newly added expenses Groups
                PopuLateExpenses(_userExpensesCache);

            })).ConfigureAwait(false);
        }

        private async void SplittrNotification_ExpensesSplited(ExpenseSplittedEventArgs obj)
        {
            if (obj?.NewExpenses is null)
            {
                return;
            }
            //Populating Existing Cache
            _userExpensesCache?.AddRange(obj.NewExpenses);

            await RunOnUiThread((() =>
            {
                //grouping and Populating Newly added expenses Groups
                PopuLateExpenses(_userExpensesCache);

            })).ConfigureAwait(false);
        }

        public void ViewDisposed()
        {
            SplittrNotification.ExpensesSplited -= SplittrNotification_ExpensesSplited;
            SplittrNotification.ExpenseStatusChanged -= SplittrNotification_ExpenseStatusChanged;
        }

        public event Action BindingUpdateInvoked;

        public void OnExpenseListControlLoaded()
        {
            CallUseCaseToFetchCurrentUserExpense();
   
        }
        private void CallUseCaseToFetchCurrentUserExpense()
        {
            var getUserExpenses = new GetExpensesByIdRequest(CancellationToken.None, new ExpenseListVmPresenterCb(this), Store.CurreUserBobj);

            var getUserExpensesUseCase = InstanceBuilder.CreateInstance<GetExpensesByUserId>(getUserExpenses);

            getUserExpensesUseCase.Execute();
        }
        private void PopuLateExpenses(IEnumerable<ExpenseBobj> currentUserExpenses)
        {
            GroupedExpenses.Clear();

            TitleText = "All Expenses";

            GroupingAndPopulateExpensesList(FilterCurrentUserExpense(currentUserExpenses));

            _view.SetCollectionListSource(GroupedExpenses);

            IEnumerable<ExpenseBobj> FilterCurrentUserExpense(IEnumerable<ExpenseBobj> expenses)
            {
                return expenses.Where(IsNotOwnerExpense);
            }
        }

        #region GroupingLogicRegion

        private void GroupingAndPopulateExpensesList(IEnumerable<ExpenseBobj> filteredExpenses)
        {
            if (filteredExpenses == null)
            {
                return;
            }
            var groups = _expenseGrouper.CreateExpenseGroupList(filteredExpenses);

            GroupedExpenses.Clear();
            foreach (var group in groups)
            {
                GroupedExpenses.Add(group);
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
                return !expense.SplitRaisedOwner.Equals(Store.CurreUserBobj);
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
                return expense.SplitRaisedOwner.Equals(Store.CurreUserBobj);
            }
        }
        #endregion


        class ExpenseListVmPresenterCb : IPresenterCallBack<GetExpensesByIdResponse>
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
                _viewModel._userExpensesCache = result.CurrentUserExpenses.ToList();
                await RunOnUiThread((() =>
                {
                    _viewModel.PopuLateExpenses(result.CurrentUserExpenses);

                })).ConfigureAwait(false);
            }
            public void OnError(SplittrException ex)
            {
                ExceptionHandlerService.HandleException(ex);
            }
        }
    }

}
