using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.Models;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using SPLITTR_Uwp.Views;
using Windows.UI.Xaml.Hosting;
using SPLITTR_Uwp.DataTemplates;

namespace SPLITTR_Uwp.ViewModel;

internal class OwingMoneyPaymentExpenseViewModel
{



    public async void PaymetForExpenseButtonClicked(Expense expense)
    {
        

        
    }

    public void PaymentWindowXamlRoot_SettleUpButtonClicked(bool isWalletPayment)
    {
        Debug.WriteLine($"is Wallet Payment.....{isWalletPayment}");
    }

}
//private static AppWindow _paymentWindow = null;
/*
 * 
 * 
 * //// C# code to create a new window
        //var newWindow = WindowHelper.CreateWindow();
        //var rootPage = new NavigationRootPage();
        //rootPage.RequestedTheme = ThemeHelper.RootTheme;
        //newWindow.Content = rootPage;
        //newWindow.Activate();
        if (_paymentWindow == null)
        {


            _paymentWindow = await AppWindow.TryCreateAsync();

            //Manually Assigning xamlRoot 
            var paymentWindowXamlRoot = new PaymentWindowExpenseView
            {
                DataContext = expense
            };
            paymentWindowXamlRoot.SettleUpButtonClicked += PaymentWindowXamlRoot_SettleUpButtonClicked;

            _paymentWindow.Title = expense.Description + " Settle Up";

            _paymentWindow.Changed += _paymentWindow_Changed;

            //Call the ElementCompositionPreview.SetAppWindowContent method to attach the XAML content to the AppWindow.
            ElementCompositionPreview.SetAppWindowContent(_paymentWindow, paymentWindowXamlRoot);

            _paymentWindow.RequestSize(new Size(400,400));
            _paymentWindow.Closed += PaymentWindowOnClosed;

}
        await _paymentWindow.TryShowAsync();


private async void _paymentWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (!args.DidVisibilityChange)
        {
            return;
        }
        if (sender.IsVisible is true)
        {
            return;
        }
        await sender.CloseAsync();
        _paymentWindow = null;
    }

    private void PaymentWindowOnClosed(AppWindow sender, AppWindowClosedEventArgs args)
    {
        _paymentWindow = null;
    }
 */