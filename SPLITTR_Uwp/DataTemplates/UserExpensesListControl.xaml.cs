using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using System.Collections.ObjectModel;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.VmLogic;
using SPLITTR_Uwp.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class UserExpensesListControl : UserControl,IExpenseListControl
    {
        public ExpensesListControlViewModel ViewModel { get; }

        private static UserExpensesListControl ListControl { get; set; }
        public UserExpensesListControl()
        {
            this.InitializeComponent();
            ViewModel = ActivatorUtilities.CreateInstance<ExpensesListControlViewModel>(App.Container, this);
            Loaded += UserExpensesListControl_Loaded;
            ListControl = this;

        }

        private void UserExpensesListControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnExpenseListControlLoaded();
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
                    ListControl.ViewModel.PopulateUserRaisedExpenses();
                    break;
                case ExpenseListFilterObj.ExpenseFilter.RequestToMe:
                    ListControl.ViewModel.PopulateUserReceivedExpenses();
                    break;
                case ExpenseListFilterObj.ExpenseFilter.AllExpenses:
                    ListControl.ViewModel.OnExpenseListControlLoaded();
                    break;
                case ExpenseListFilterObj.ExpenseFilter.GroupExpense:
                    ListControl.ViewModel.PopulateSpecificGroupExpenses(requestObj.Group);
                    break;
                case ExpenseListFilterObj.ExpenseFilter.UserExpense:
                    ListControl.ViewModel.PopulateUserRelatedExpenses(requestObj.User);
                    break;
            }
        }

        
        public event Action PaneButtonOnClick;
        private void NavigationPaneName_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationPaneControlButton.Style = NavigationPaneControlButton.Style == OpenPaneButtonStyle ? ClosePaneButtonStyle : OpenPaneButtonStyle;
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
                if (expense is ExpenseViewModel expenseVm)
                {
                    expenseVm.Visibility = !expenseVm.Visibility;
                    expenseVisibility = expenseVm.Visibility;
                }
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
                   ViewModel.SortExpenseBasedOnDate(ViewModel.GroupedExpenses);

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

        public ExpenseViewModel SelectedExpenseObj { get; set; }

        public string TitleText
        {
            get => _titleText;
            set => SetProperty(ref _titleText, value);
        }



        public ExpensesListControlViewModel(IExpenseListControl view, IExpenseGrouper expenseGrouper)
        {
            _view = view;
            _expenseGrouper = expenseGrouper;
            Store.CurreUserBobj.ValueChanged += CurreUserBobj_ValueChanged;
        }

        private async void CurreUserBobj_ValueChanged(string property)
        {
            await UiService.RunOnUiThread((() =>
            {
                switch (property)
                {
                    case nameof(UserViewModel.Expenses):
                        PopuLateExpenses();
                        break;
                }
                BindingUpdateInvoked?.Invoke();
                //ViewLoaded();
            }));
        }

        public event Action BindingUpdateInvoked;

        public void OnExpenseListControlLoaded()
        {
            PopuLateExpenses();
        }
        private void PopuLateExpenses()
        {
            GroupedExpenses.Clear();

            TitleText = "All Expenses";

            GroupingAndPopulateExpensesList(FilterCurrentUserExpense());

            _view.SetCollectionListSource(GroupedExpenses);

            IEnumerable<ExpenseBobj> FilterCurrentUserExpense()
            {
                return Store.CurreUserBobj.Expenses.Where(IsNotOwnerExpense);
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
            var groupSpecificExpenses = Store.CurreUserBobj?.Expenses.Where(ex => IsSelectedGroupExpense(ex,selectedGroup) && IsNotOwnerExpense(ex));

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
            var userSpecificExpenses = Store.CurreUserBobj?.Expenses.Where(CheckExpenseMatchesUser);

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

            var userReceivedExpenses = Store.CurreUserBobj?.Expenses.Where(ex => IsUserRecievedExpense(ex) && IsNotOwnerExpense(ex));

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

            var userRaisedExpenses = Store.CurreUserBobj?.Expenses.Where(ex =>IsUserRaisedExpense(ex) && IsNotOwnerExpense(ex));

            GroupingAndPopulateExpensesList(userRaisedExpenses);
            //Setting ExpenseControl Title
            TitleText = "Request By Me";

            bool IsUserRaisedExpense(ExpenseBobj expense)
            {
                return expense.SplitRaisedOwner.Equals(Store.CurreUserBobj);
            }
        }
        #endregion

    }

}
