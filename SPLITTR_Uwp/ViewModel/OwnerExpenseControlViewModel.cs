using System;
using System.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CancelExpense;
using SPLITTR_Uwp.Core.UseCase.MarkAsPaid;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SQLite;

namespace SPLITTR_Uwp.ViewModel
{
   

    internal class OwnerExpenseControlViewModel : ObservableObject
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
            var cancelExpenseRequestObj = new CancelExpenseRequestObj(new OwnerExpenseControlVmPresenterCallBack(this), Store.CurreUserBobj, expense.ExpenseUniqueId, cts.Token);

            var cancelExpensesUseCaseObj = InstanceBuilder.CreateInstance<CancelExpense>(cancelExpenseRequestObj);

            cancelExpensesUseCaseObj?.Execute();

        }

        public void OnExpenseMarkedAsPaid(ExpenseBobj expense)
        {
            
            var cts = new CancellationTokenSource();

            var markAsPaidRequestObj = new MarkAsPaidRequestObj(new OwnerExpenseControlVmPresenterCallBack(this), cts.Token, expense.ExpenseUniqueId, Store.CurreUserBobj);

            var markAsPaidUseCaseObj = InstanceBuilder.CreateInstance<MarkAsPaid>(markAsPaidRequestObj);

            markAsPaidUseCaseObj.Execute();

        }

        public async void InvokeOnMarkAsPaidCompleted(MarkAsPaidResponseObj result)
        {
            await UiService.ShowContentAsync($"{result.MarkedPaidExpenseBobj.Description} Paid", "Marked as PAid").ConfigureAwait(false);
            await UiService.RunOnUiThread(() =>
            {
                ExpenseCancelButtonVisibility = false;
            });
        }
        public async void InvokeOnExpenseCancellationCompleted(CancelExpenseResponseObj result)
        {
            await UiService.ShowContentAsync($"{result.CancelledExpense.Description} Cancelled", "Split Cancelled").ConfigureAwait(false);
            
            await UiService.RunOnUiThread(() =>
            {
                ExpenseCancelButtonVisibility = false;
            });

        }
        class OwnerExpenseControlVmPresenterCallBack : IPresenterCallBack<MarkAsPaidResponseObj>, IPresenterCallBack<CancelExpenseResponseObj>
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
}
