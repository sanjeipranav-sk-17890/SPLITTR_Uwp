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
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class OwnerExpenseUserControl : UserControl
    {
       

        public ExpenseViewModel ExpenseObj
        {
            get=> this.DataContext as ExpenseViewModel;
        }

        private OwnerExpenseControlViewModel _viewModel;

        public OwnerExpenseUserControl()
        {
            this.InitializeComponent();
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
    }
}
