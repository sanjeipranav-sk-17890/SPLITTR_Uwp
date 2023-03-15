using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CancelExpense;
using SPLITTR_Uwp.Core.UseCase.MarkAsPaid;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SQLite;

namespace SPLITTR_Uwp.ViewModel;

internal class OwnerExpenseControlViewModel : ObservableObject
{
    private bool _expenseCancelButtonVisibility;
        

        
    public bool ExpenseCancelButtonVisibility
    {
        get => _expenseCancelButtonVisibility;
        set => SetProperty(ref _expenseCancelButtonVisibility, value);
    }

    public void DataContextLoaded(ExpenseBobj expense)
    {
        ChangeButtonVisibility(expense);

    }

    private void ChangeButtonVisibility(ExpenseBobj expense)
    {
        if (expense.ExpenseStatus == ExpenseStatus.Pending && expense.SplitRaisedOwner.Equals(Store.CurrentUserBobj))
        {
            ExpenseCancelButtonVisibility = true;
            return;
        }
        ExpenseCancelButtonVisibility = false;
    }

   
    public void OnExpenseCancellation(ExpenseBobj expense)
    {
            
        var cts = new CancellationTokenSource();
        var cancelExpenseRequestObj = new CancelExpenseRequestObj(new OwnerExpenseControlVmPresenterCallBack(this), Store.CurrentUserBobj, expense, cts.Token);

        var cancelExpensesUseCaseObj = InstanceBuilder.CreateInstance<CancelExpense>(cancelExpenseRequestObj);

        cancelExpensesUseCaseObj?.Execute();

    }

    public void OnExpenseMarkedAsPaid(ExpenseBobj expense)
    {
            
        var cts = new CancellationTokenSource();

        var markAsPaidRequestObj = new MarkAsPaidRequestObj(new OwnerExpenseControlVmPresenterCallBack(this), cts.Token,Store.CurrentUserBobj, expense);

        var markAsPaidUseCaseObj = InstanceBuilder.CreateInstance<MarkAsPaid>(markAsPaidRequestObj);

        markAsPaidUseCaseObj.Execute();

    }

    private async void InvokeOnMarkAsPaidCompleted(MarkAsPaidResponseObj result)
    {
        await UiService.ShowContentAsync($"{result.MarkedPaidExpenseBobj.Description} Paid", "Marked as PAid").ConfigureAwait(false);
        _ = UiService.RunOnUiThread(() => { ExpenseCancelButtonVisibility = false; }).ConfigureAwait(false);
    }
    private async void InvokeOnExpenseCancellationCompleted(CancelExpenseResponseObj result)
    {
        await UiService.ShowContentAsync($"{result.CancelledExpense.Description} Cancelled", "Split Cancelled").ConfigureAwait(false);
            
        _ = UiService.RunOnUiThread(() => { ExpenseCancelButtonVisibility = false; }).ConfigureAwait(false);

    }
    private class OwnerExpenseControlVmPresenterCallBack : IPresenterCallBack<MarkAsPaidResponseObj>, IPresenterCallBack<CancelExpenseResponseObj>
    {
        private readonly OwnerExpenseControlViewModel _viewModel;
        public OwnerExpenseControlVmPresenterCallBack(OwnerExpenseControlViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public void OnSuccess(MarkAsPaidResponseObj result)
        {
            _viewModel.InvokeOnMarkAsPaidCompleted(result);
        }
        public void OnSuccess(CancelExpenseResponseObj result)
        {
            _viewModel.InvokeOnExpenseCancellationCompleted(result);
        }
        void IPresenterCallBack<CancelExpenseResponseObj>.OnError(SplittrException ex)
        {
            HandleError(ex);
        }
        void IPresenterCallBack<MarkAsPaidResponseObj>.OnError(SplittrException ex)
        {
            HandleError(ex);
        }
        private void HandleError(SplittrException ex)
        {
            switch (ex.InnerException)
            {
                case ArgumentException or ArgumentNullException:
                    ExceptionHandlerService.HandleException(ex.InnerException);
                    break;
                case SQLiteException:
                    //Retry Code Logic Here
                    break;
            }
        }
    }

}