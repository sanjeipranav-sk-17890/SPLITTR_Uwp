using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;
using SPLITTR_Uwp.ViewModel.VmLogic;

namespace SPLITTR_Uwp.ViewModel
{
    internal class MainPageViewModel :ObservableObject, IMainPageViewModel
    {
        
            private readonly DataStore _store;
            private readonly IMainView _mainPage;
            private readonly IExpenseGrouper _expenseGrouper;
            private string _userInitial;
            private bool _isUpdateWalletBalanceTeachingTipOpen;


            public MainPageViewModel(DataStore store, IMainView mainPage,IExpenseGrouper expenseGrouper)
            {
                _store = store;
                _mainPage = mainPage;
                _expenseGrouper = expenseGrouper;
                UserViewModel = new UserViewModel(_store.UserBobj);
                _store.UserBobj.ValueChanged += UserObjUpdated;

            }


            public string UserInitial
            {
                get => UserViewModel.UserName.GetUserInitial();
                set => SetProperty(ref _userInitial, value);
            }
  
            public UserViewModel UserViewModel { get; }
  
            public bool IsUpdateWalletBalanceTeachingTipOpen
            {
                get => _isUpdateWalletBalanceTeachingTipOpen;
                set => SetProperty(ref _isUpdateWalletBalanceTeachingTipOpen, value);
            }

            public ObservableCollection<GroupBobj> UserGroups { get; } = new ObservableCollection<GroupBobj>();

            public ObservableCollection<User> RelatedUsers { get; } = new ObservableCollection<User>();

            public ObservableCollection<ExpenseGroupingList> ExpensesList { get; } = new ObservableCollection<ExpenseGroupingList>();

            #region GroupingLogicRegion

            private void GroupingAndPopulateExpensesList(IEnumerable<ExpenseBobj> filteredExpenses)
            {
                if (filteredExpenses == null)
                {
                    return;
                }
                var groups = _expenseGrouper.CreateExpenseGroupList(filteredExpenses);

               ExpensesList.Clear();
                foreach (var group in groups)
                {
                   ExpensesList.Add(group);
                }
            }
            #endregion



            #region RelatedUserLogicRegion

            public void ViewLoaded() 
            {

                  if (_store.UserBobj == null)
                  {
                      //Setting frame reference to Default windows Frame
                      NavigationService.Frame = null;
                      NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());
                      return;
                  }
                  UserGroups.ClearAndAdd(_store.UserBobj.Groups);
                  PopulateIndividualSplitUsers();
                  PopulateAllExpense();
                  
            }
            public void PopulateAllExpense()
            {
                ExpensesList.Clear();
                
                GroupingAndPopulateExpensesList(_store.UserBobj.Expenses);
            }


            public void PopulateSpecificGroupExpenses(Group selectedGroup)
            {
                if (selectedGroup is null)
                {
                    return;
                }
                ExpensesList.Clear();
                //filter expenses based on particular group 
                var groupSpecificExpenses = _store.UserBobj?.Expenses.Where(e => e.GroupUniqueId is not null && e.GroupUniqueId.Equals(selectedGroup.GroupUniqueId));
                
                GroupingAndPopulateExpensesList(groupSpecificExpenses);
            }
            public void PopulateUserRelatedExpenses(User selectedUser)
            {
                if (selectedUser is null)
                {
                    return;
                }
                ExpensesList.Clear();
                //filter expenses based on Related User
                var userSpecificExpenses = _store.UserBobj?.Expenses.Where(e => e.SplitRaisedOwner.Equals(selectedUser) || e.CorrespondingUserObj.Equals(selectedUser));

                GroupingAndPopulateExpensesList(userSpecificExpenses);
            }
            public void PopulateUserRecievedExpenses()
            {
                ExpensesList.Clear();

                var userRecievedExpenses = _store.UserBobj?.Expenses.Where(e => !e.SplitRaisedOwner.Equals(UserViewModel));

                GroupingAndPopulateExpensesList(userRecievedExpenses);
            }
            public void PopulateUserRaisedExpenses()
            {
                ExpensesList.Clear();

                var userRaisedExpenses = _store.UserBobj?.Expenses.Where(e => e.SplitRaisedOwner.Equals(UserViewModel));

                GroupingAndPopulateExpensesList(userRaisedExpenses);
            }





        private void PopulateIndividualSplitUsers()
            {
                RelatedUsers.Clear();
                foreach (var expense in _store.UserBobj.Expenses)
                {
                    // Adds user object to Individual Split users  list Excluding Current User 
                    if (expense.GroupUniqueId is not null)
                    {
                        continue;
                    }
                    if (!expense.SplitRaisedOwner.Equals(_store.UserBobj) && !RelatedUsers.Contains(expense.SplitRaisedOwner))
                    {
                        RelatedUsers.Add(expense.SplitRaisedOwner);
                    }
                    if (!expense.CorrespondingUserObj.Equals(_store.UserBobj) && !RelatedUsers.Contains(expense.CorrespondingUserObj))
                    {
                        RelatedUsers.Add(expense.CorrespondingUserObj);
                    }
                }
                
            }

            #endregion


            public async void UserObjUpdated()
            {
                //since this Will be called by Worker thread it needs to invoked by Ui thread so calling dispatcher to user it
                await UiService.RunOnUiThread((() =>
                {
                    OnPropertyChanged(nameof(UserInitial));
                    ViewLoaded();//refreshing value assigning
                }));

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
                _store.UserBobj = null;
                NavigationService.Frame = null;
                NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());

            }

            public void PersonProfileClicked()
            {
                PaneVisibility=false;
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
                if (string.Compare(title, "Add Exepense", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    NavigationService.Navigate(typeof(AddExpenseTestPage), new DrillInNavigationTransitionInfo());
                }
                else
                {
                    NavigationService.Navigate(typeof(GroupCreationPage), new DrillInNavigationTransitionInfo());
                }
            }

            #endregion



            public void AddWalletBalanceButtonClicked()
            {
                IsUpdateWalletBalanceTeachingTipOpen = !IsUpdateWalletBalanceTeachingTipOpen;
            }




            
    }
}
