using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ConversationalAgent;
using Windows.UI.Xaml;
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
        private readonly DataStore _store;
        private readonly IExpenseUtility _expenseUtility;

        public OwnerExpenseControlViewModel(DataStore store,IExpenseUtility expenseUtility)
        {
            _store = store;
            _expenseUtility = expenseUtility;
            
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
            if (expense.ExpenseStatus == ExpenseStatus.Pending && expense.SplitRaisedOwner.Equals(_store.UserBobj))
            {
                ExpenseCancelButtonVisibility = true;
                return;
            }
            ExpenseCancelButtonVisibility = false;
        }

   
        public void OnExpenseCancellation(ExpenseBobj expense)
        {
            _expenseUtility.PresenterCallBackOnSuccess += async args =>
            {
               await UiService.ShowContentAsync($"{expense.Description} Cancelled", "Split Cancelled");
            };
            _expenseUtility.OnError += (exception, s) => ExceptionHandlerService.HandleException(exception); 

            _expenseUtility.CancelExpense(expense.ExpenseUniqueId,_store.UserBobj);
        }


        public void OnExpenseMarkedAsPaid(ExpenseBobj expense)
        {
            _expenseUtility.PresenterCallBackOnSuccess += async args =>
            {
                await UiService.ShowContentAsync($"{expense.Description} Marked as Paid", "Split Completed");
            };
            _expenseUtility.OnError += (exception, s) => ExceptionHandlerService.HandleException(exception);

            _expenseUtility.MarkExpenseAsPaid(expense.ExpenseUniqueId, _store.UserBobj);

        }


    }
}
