using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class OwingMoneyPayemtExpenseTemplate : UserControl
    {
        private OwingMoneyPaymentExpenseViewModel _viewModel;
        
        public OwingMoneyPayemtExpenseTemplate()
        {
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<OwingMoneyPaymentExpenseViewModel>(App.Container);
            this.InitializeComponent();
            DataContextChanged += OwingMoneyPayemtExpenseTemplate_DataContextChanged;
        }

        private void OwingMoneyPayemtExpenseTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext is not ExpenseViewModel expense)
            {
                return;
            }

            InvertPaymentControlVisibility(false);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            if (DataContext is not ExpenseViewModel expenseObj)
            {
                return;
            }
          
            InvertPaymentControlVisibility(true);

            _viewModel.PaymetForExpenseButtonClicked(expenseObj);
           
        }


        private void InvertPaymentControlVisibility(bool isVisible)
        {
            if (isVisible)
            {
                SettleUpButton.Visibility = Visibility.Collapsed;
                PaymentControl.Visibility = Visibility.Visible;
                return;
            }
            SettleUpButton.Visibility = Visibility.Visible;
            PaymentControl.Visibility = Visibility.Collapsed;

        }
    }
}
