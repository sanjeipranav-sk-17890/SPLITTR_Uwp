using System;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.SettleUpExpense;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using SQLite;

namespace SPLITTR_Uwp.ViewModel;



internal class OwingMoneyPaymentExpenseViewModel :ObservableObject
{
    
    private bool _settleButtonVisibility;
    private bool _paymentButtonVisibility;


    public bool SettleButtonVisibility
    {
        get => _settleButtonVisibility;
        set => SetProperty(ref _settleButtonVisibility, value);
    }

    public bool PaymentControlVisibility
    {
        get => _paymentButtonVisibility;
        set => SetProperty(ref _paymentButtonVisibility, value);
    }


    private ExpenseViewModel _moneyPaymentExpense = null;
    public void PaymentWindowXamlRoot_SettleUpButtonClicked(bool isWalletPayment)
    {

        if (_moneyPaymentExpense is null)
        {
            Debug.WriteLine($"{nameof(_moneyPaymentExpense)} is null check Logic ");
            return;
        }
        var cts = new CancellationTokenSource();

        var settleUpReqObj = new SettleUPExpenseRequestObj(isWalletPayment, Store.CurreUserBobj, _moneyPaymentExpense, new OwingMoneyPaymentExpenseVmPresenterCallBack(this), cts.Token);

        var settleUpUseCaseObj = InstanceBuilder.CreateInstance<SettleUpSplit>(settleUpReqObj);

        settleUpUseCaseObj.Execute();

    }


    public void PaymetForExpenseButtonClicked(ExpenseViewModel expenseObj)
    {
        _moneyPaymentExpense = expenseObj;
    }


    public async void InvokeOnSettleUPSuccess(SettleUpExpenseResponseObj result)
    {
        await UiService.ShowContentAsync("Splittr Completed SuccessFully", "Payment Complete");
        await UiService.RunOnUiThread(() =>
        {
            SettleButtonVisibility = false;
            PaymentControlVisibility = false;
        });
    }
   class OwingMoneyPaymentExpenseVmPresenterCallBack : IPresenterCallBack<SettleUpExpenseResponseObj>
    {
        private readonly OwingMoneyPaymentExpenseViewModel _viewModel;

        public OwingMoneyPaymentExpenseVmPresenterCallBack(OwingMoneyPaymentExpenseViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public void OnSuccess(SettleUpExpenseResponseObj result)
        {
            _viewModel.InvokeOnSettleUPSuccess(result);
        }
        public void OnError(SplittrException ex)
        {
            if (ex.InnerException is NotSupportedException)
            {
                ExceptionHandlerService.HandleException(ex);
            }
            if (ex.InnerException is SQLiteException)
            {
                //logic to Show Something Went wrong
            }
        }
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