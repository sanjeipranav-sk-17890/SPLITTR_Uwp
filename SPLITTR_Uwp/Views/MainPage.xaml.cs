using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.DataTemplates.Controls;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Contracts;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IMainView
    {
        IMainPageViewModel _viewModel;
        private static MainPage _view;

        private void PageOnPaneButtonOnClick()
        {
            var isMainPaneOpen = MainPageNavigationView.IsPaneOpen;
            MainPageNavigationView.IsPaneOpen = !isMainPaneOpen;
        }

        public Frame ChildFrame
        {
            get => InnerFrame;
        }

        public MainPage()
        {
            _viewModel = ActivatorUtilities.CreateInstance<MainPageViewModel>(App.Container, this);
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;
            this.InitializeComponent();
            _view = this;
            _viewModel.UserGroups.CollectionChanged += UserGroups_CollectionChanged;
        }

        private void _viewModel_BindingUpdateInvoked()
        {
            Bindings.Update();
        }
        #region NavigationViewGroupsPopulating 


        private void UserGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //filtering GroupBobs From NavigationView Collection
            var groupNavigationViewItems = MainPageNavigationView.MenuItems
                .Where(i => i is NavigationViewItemBase { Content: GroupBobj })
                .Select(menuItem =>
            {
                var navigationViewItem = (NavigationViewItemBase)menuItem;
                return navigationViewItem.Content as GroupBobj;
            });

            if (sender is not ObservableCollection<GroupBobj> userGroups)
            {
                return;
            }
            foreach (var group in userGroups)
            {
                if (group != null && CheckIfGroupNotExistInNavigationView(group))
                {
                    AddGroupToNavigationView(group);
                }

                //return true if Group obj is Already in NavigationView menu item list
                bool CheckIfGroupNotExistInNavigationView(GroupBobj group)
                {
                    return !groupNavigationViewItems.Contains(group);
                }
            }

        }

        private void AddGroupToNavigationView(GroupBobj group)
        {
            MainPageNavigationView.MenuItems.Add(new Microsoft.UI.Xaml.Controls.NavigationViewItem()
            {
                Content = group,
                Style = UserGroupNavigationItemStyle,
            });
        }

        #endregion


        #region NavigationLogic

        private void AppIcon_OnClick(object sender, TappedRoutedEventArgs e)
        {
            NavigationRequested();
        }
        private void NavigationRequested()
        {

            MainPageNavigationView.IsPaneOpen = true;
            NavigationService.Frame = InnerFrame;
            NavigationService.Navigated += NavigationService_Navigated;
            NavigationService.Navigate(typeof(ExpensesListAndDetailViewPage));
        }
        public static void RequestMainPageNavigation()
        {
            _view?.NavigationRequested();
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {

            if (InnerFrame.Content is not ExpensesListAndDetailViewPage page)
            {
                return;
            }

            page.PaneButtonOnClick += (PageOnPaneButtonOnClick);

            NavigationService.Navigated -= NavigationService_Navigated;
        }

        #endregion

        #region ExpenseSelection Control Redirection 

        private void UserSelectedFromIndividualSplitList(User selectedUser)
        {
            if (selectedUser is null)
            {
                return;
            }
            UserExpensesListControl.FilterExpense(new ExpenseListFilterObj(null, selectedUser, ExpenseListFilterObj.ExpenseFilter.UserExpense));

        }

        private void MainPageNavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

            if (args?.SelectedItemContainer?.Content is GroupBobj selectedGroup)
            {
                UserExpensesListControl.FilterExpense(new ExpenseListFilterObj(selectedGroup, null, ExpenseListFilterObj.ExpenseFilter.GroupExpense));
                return;
            }
            if (args.SelectedItem is not StackPanel stackPanel)
            {
                return;
            }
            //setting Title and Calling respective viewModel To Polulate the Respective Expenses
            switch (stackPanel.Name)
            {
                case nameof(AllExpense):
                    UserExpensesListControl.FilterExpense(new ExpenseListFilterObj(null, null, ExpenseListFilterObj.ExpenseFilter.AllExpenses));
                    break;
                case nameof(RequestToMe):
                    UserExpensesListControl.FilterExpense(new ExpenseListFilterObj(null, null, ExpenseListFilterObj.ExpenseFilter.RequestToMe));
                    break;
                case nameof(RequestedByMe):
                    UserExpensesListControl.FilterExpense(new ExpenseListFilterObj(null, null, ExpenseListFilterObj.ExpenseFilter.RequestByMe));
                    break;
            }

        }

        #endregion

        #region DashBoardSplitViewLogicRegion
        private void DashBoardButton_OnClick(object sender, RoutedEventArgs e)
        {
            DashBoardSplitView.IsPaneOpen = !DashBoardSplitView.IsPaneOpen;
        }
        private void DashBoardSplitView_OnPaneChanged(SplitView sender, object args)
        {
            AssignDashBoardState();
        }
        private void AssignDashBoardState()
        {
            if (DashBoardSplitView.IsPaneOpen)
            {
                VisualStateManager.GoToState(this, nameof(CloseDashBoardState), false);
                return;
            }
            VisualStateManager.GoToState(this, nameof(OpenDashBoardState), false);
        }

        #endregion

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ExceptionHandlerService.HandleException(new Exception("This Is Test MEssage"));
        }
    }
}
