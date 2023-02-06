using System;
using System.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CancelExpense;
using SPLITTR_Uwp.Core.UseCase.MarkAsPaid;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SQLite;

namespace SPLITTR_Uwp.ViewModel
{
    internal class OwnerExpenseControlViewModel : ObservableObject,IPresenterCallBack<MarkAsPaidResponseObj>,IPresenterCallBack<CancelExpenseResponseObj>
    {
        private bool _expenseCancelButtonVisibility = false;
        

        
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
            if (expense.ExpenseStatus == ExpenseStatus.Pending && expense.SplitRaisedOwner.Equals(Store.CurreUserBobj))
            {
                ExpenseCancelButtonVisibility = true;
                return;
            }
            ExpenseCancelButtonVisibility = false;
        }

   
        public void OnExpenseCancellation(ExpenseBobj expense)
        {
            
            var cts = new CancellationTokenSource();
            var cancelExpenseRequestObj = new CancelExpenseRequestObj(this, Store.CurreUserBobj, expense.ExpenseUniqueId, cts.Token);

            var cancelExpensesUseCaseObj = InstanceBuilder.CreateInstance<CancelExpense>(cancelExpenseRequestObj);

            cancelExpensesUseCaseObj?.Execute();

        }

        public void OnExpenseMarkedAsPaid(ExpenseBobj expense)
        {
            
            var cts = new CancellationTokenSource();

            var markAsPaidRequestObj = new MarkAsPaidRequestObj(this, cts.Token, expense.ExpenseUniqueId, Store.CurreUserBobj);

            var markAsPaidUseCaseObj = InstanceBuilder.CreateInstance<MarkAsPaid>(markAsPaidRequestObj);

            markAsPaidUseCaseObj.Execute();

        }

        public async void OnSuccess(MarkAsPaidResponseObj result)
        {
            await UiService.ShowContentAsync($"{result.MarkedPaidExpenseBobj.Description} Paid", "Marked as PAid").ConfigureAwait(false);
            await UiService.RunOnUiThread(() =>
            {
                ExpenseCancelButtonVisibility = false;
            });
        }
        public async void OnSuccess(CancelExpenseResponseObj result)
        {
            await UiService.ShowContentAsync($"{result.CancelledExpense.Description} Cancelled", "Split Cancelled").ConfigureAwait(false);
            
            await UiService.RunOnUiThread(() =>
            {
                ExpenseCancelButtonVisibility = false;
            });

        }
        public void OnError(SplittrException ex)
        {
            if (ex.InnerException is SQLiteException)
            {
               //Some UI Logic to show Something Went Wrong
            }
            else if(ex.InnerException is ArgumentException )
            {
                ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}
