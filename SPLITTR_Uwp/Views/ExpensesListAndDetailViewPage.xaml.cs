using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.DataTemplates;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpensesListAndDetailViewPage : Page
    {
        private ExpenseListAndDetailedPageViewModel _viewModel;
        public ExpensesListAndDetailViewPage()
        {
            this.InitializeComponent();
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseListAndDetailedPageViewModel>(App.Container);
            
        }

        public event Action PaneButtonOnClick;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {    //clearing previous subscriptions 
            PaneButtonOnClick = null;
        }


        private void ExpensesLIstView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
            {
                return;
            }
            _viewModel.SelectedExpenseObj = e.AddedItems[0] as ExpenseViewModel;
            _viewModel.ExpenseSelectionMade();
        }
        private void UserExpensesListControl_OnPaneButtonOnClick()
        {
            PaneButtonOnClick?.Invoke();
        }

    }

 
}
