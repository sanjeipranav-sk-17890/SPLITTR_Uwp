using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class OwnerExpenseUserControl : UserControl
    {


        private ExpenseVobj ExpenseObj
        {
            get=> DataContext as ExpenseVobj;
        }

        private readonly OwnerExpenseControlViewModel _viewModel;

        public OwnerExpenseUserControl()
        {
            InitializeComponent();
            DataContextChanged += OwnerExpenseUserControl_DataContextChanged;
            _viewModel = ActivatorUtilities.CreateInstance<OwnerExpenseControlViewModel>(App.Container);
        }

        private void OwnerExpenseUserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Bindings.Update();
            if (ExpenseObj is null)
            {
                return;
            }
            _viewModel.DataContextLoaded(ExpenseObj);
        }
        private void MarkAsPaidButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ExpenseObj is not null)
            {
                _viewModel.OnExpenseMarkedAsPaid(ExpenseObj);
            }
        }
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ExpenseObj is not null)
            {
                _viewModel.OnExpenseCancellation(ExpenseObj);
            }
        }
        public event Action<object, RoutedEventArgs> BackButtonClicked;

        private void ExpenseDetailedViewUserControl_OnBackButtonClicked(object arg1, RoutedEventArgs arg2)
        {
            BackButtonClicked?.Invoke(arg1,arg2);
        }
    }
}
