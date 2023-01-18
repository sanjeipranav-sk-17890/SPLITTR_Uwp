using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;

namespace SPLITTR_Uwp.ViewModel
{
    internal class OwnerExpenseControlViewModel : ObservableObject
    {
        private bool _expenseCancelButtonVisibility = false;
   
        private readonly IExpenseUseCase _expenseUseCase;

        public OwnerExpenseControlViewModel(IExpenseUseCase expenseUseCase)
        {
            _expenseUseCase = expenseUseCase;
            
        }

        private void Utility_OnError(Exception exception, string arg2)
        {
            ExceptionHandlerService.HandleException(exception);
        }

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
            _expenseUseCase.PresenterCallBackOnSuccess += ExpenseUtilityPresenterCallBackOnSuccess;
            _expenseUseCase.OnError += (exception, s) => ExceptionHandlerService.HandleException(exception); 

            _expenseUseCase.CancelExpense(expense.ExpenseUniqueId,Store.CurreUserBobj);


            async void ExpenseUtilityPresenterCallBackOnSuccess(EventArgs obj)
            {
                await UiService.ShowContentAsync($"{expense.Description} Cancelled", "Split Cancelled").ConfigureAwait(false);
                _expenseUseCase.PresenterCallBackOnSuccess -= ExpenseUtilityPresenterCallBackOnSuccess;

                
               await UiService.RunOnUiThread(() =>
                {
                    ExpenseCancelButtonVisibility = false;
                });
            }
        }

       

        public void OnExpenseMarkedAsPaid(ExpenseBobj expense)
        {
            _expenseUseCase.PresenterCallBackOnSuccess += ExpenseUtilityPresenterCallBackOnSuccess;
            _expenseUseCase.OnError += (exception, s) => ExceptionHandlerService.HandleException(exception);

            _expenseUseCase.MarkExpenseAsPaid(expense.ExpenseUniqueId, Store.CurreUserBobj);


            async void ExpenseUtilityPresenterCallBackOnSuccess(EventArgs obj)
            {
                await UiService.ShowContentAsync($"{expense.Description} Cancelled", "Split Cancelled").ConfigureAwait(false);
                _expenseUseCase.PresenterCallBackOnSuccess -= ExpenseUtilityPresenterCallBackOnSuccess;
                await UiService.RunOnUiThread(() =>
                {
                    ExpenseCancelButtonVisibility = false;
                });
            }

        }

        
    }
}
