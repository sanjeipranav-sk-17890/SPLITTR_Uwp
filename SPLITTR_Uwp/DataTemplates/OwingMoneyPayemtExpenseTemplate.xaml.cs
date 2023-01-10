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
            this.InitializeComponent();
        }
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            if (DataContext is not ExpenseViewModel expenseObj)
            {
                return;
            }
            _viewModel.PaymetForExpenseButtonClicked(expenseObj);
            Debug.WriteLine(expenseObj.Description);
        }
    }
}
