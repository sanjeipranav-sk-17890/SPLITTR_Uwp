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
    internal class MainPageViewModel :ObservableObject, IMainPageViewModel,IViewModel
    {
        
            
            private readonly IMainView _mainPage;
            private readonly IExpenseGrouper _expenseGrouper;
            private readonly IStateService _stateService;
            private string _userInitial;
            private bool _isUpdateWalletBalanceTeachingTipOpen;


            public MainPageViewModel( IMainView mainPage,IExpenseGrouper expenseGrouper,IStateService stateService)
            {
               
                _mainPage = mainPage;
                _expenseGrouper = expenseGrouper;
                _stateService = stateService;
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

        


            public async void UserObjUpdated(string property)
            {
                //since this Will be called by Worker thread it needs to invoked by Ui thread so calling dispatcher to user it
                await UiService.RunOnUiThread((() =>
                {
                    switch (property)
                    {
                        //refreshing value assigning
                        case nameof(UserViewModel.Groups):
                            UserGroups.ClearAndAdd(Store.CurreUserBobj.Groups);
                            break;
                    }
                    BindingUpdateInvoked?.Invoke();
                   //ViewLoaded();
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
                //Invoking Logout Event Before Clearing Current uSer Cache 
                _stateService.InvokeCurrentUserLoggedOut(Store.CurreUserBobj);

                //Clearing Reference to Current USer Cache
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


            public event Action BindingUpdateInvoked;
    }
}
