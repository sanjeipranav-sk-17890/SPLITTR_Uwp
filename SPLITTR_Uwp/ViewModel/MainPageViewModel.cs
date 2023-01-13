using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
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
        
            
            private readonly IMainView _mainPage;
            private readonly IExpenseGrouper _expenseGrouper;
            private string _userInitial;
            private bool _isUpdateWalletBalanceTeachingTipOpen;


            public MainPageViewModel( IMainView mainPage,IExpenseGrouper expenseGrouper)
            {
               
                _mainPage = mainPage;
                _expenseGrouper = expenseGrouper;
                UserViewModel = new UserViewModel(Store.CurreUserBobj);
                Store.CurreUserBobj.ValueChanged += UserObjUpdated;

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

                  if (Store.CurreUserBobj == null)
                  {
                      //Setting frame reference to Default windows Frame
                      NavigationService.Frame = null;
                      NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());
                      return;
                  }
                  UserGroups.ClearAndAdd(Store.CurreUserBobj.Groups);
                  PopulateIndividualSplitUsers();
                  PopulateAllExpense();
                  
            }
            public void PopulateAllExpense()
            {
                ExpensesList.Clear();
                
                GroupingAndPopulateExpensesList(Store.CurreUserBobj.Expenses);
            }


            public void PopulateSpecificGroupExpenses(Group selectedGroup)
            {
                if (selectedGroup is null)
                {
                    return;
                }
                ExpensesList.Clear();
                //filter expenses based on particular group 
                var groupSpecificExpenses = Store.CurreUserBobj?.Expenses.Where(e => e.GroupUniqueId is not null && e.GroupUniqueId.Equals(selectedGroup.GroupUniqueId));
                
                GroupingAndPopulateExpensesList(groupSpecificExpenses);
            }
            public void PopulateUserRelatedExpenses(User selectedUser)
            {
                if (selectedUser is null)
                {
                    return;
                }
                ExpensesList.Clear();
                var userSpecificExpenses = Store.CurreUserBobj?.Expenses.Where(CheckExpenseMatchesUser);

                GroupingAndPopulateExpensesList(userSpecificExpenses);


                //filter expenses based on Related User
                bool CheckExpenseMatchesUser(ExpenseBobj e)
                {
                    if (e.SplitRaisedOwner.Equals(selectedUser) || e.CorrespondingUserObj.Equals(selectedUser) && e.GroupUniqueId is null)
                        return true;
                    return false;
                }

            }
            public void PopulateUserRecievedExpenses()
            {
                ExpensesList.Clear();

                var userRecievedExpenses = Store.CurreUserBobj?.Expenses.Where(e => !e.SplitRaisedOwner.Equals(UserViewModel));

                GroupingAndPopulateExpensesList(userRecievedExpenses);
            }
            public void PopulateUserRaisedExpenses()
            {
                ExpensesList.Clear();

                var userRaisedExpenses = Store.CurreUserBobj?.Expenses.Where(e => e.SplitRaisedOwner.Equals(UserViewModel));

                GroupingAndPopulateExpensesList(userRaisedExpenses);
            }





        private void PopulateIndividualSplitUsers()
            {
                RelatedUsers.Clear();
                foreach (var expense in Store.CurreUserBobj.Expenses)
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
                Store.CurreUserBobj = null;
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
                NavigationService.Navigate(string.Compare(title, "Add Exepense", StringComparison.InvariantCultureIgnoreCase) == 0 ?
                    typeof(AddExpenseTestPage) :
                    typeof(GroupCreationPage), new DrillInNavigationTransitionInfo());
            }

            #endregion



            public void AddWalletBalanceButtonClicked()
            {
                IsUpdateWalletBalanceTeachingTipOpen = !IsUpdateWalletBalanceTeachingTipOpen;
            }




            
    }
}
