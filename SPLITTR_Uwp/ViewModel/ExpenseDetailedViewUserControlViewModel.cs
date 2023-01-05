using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    internal class ExpenseDetailedViewUserControlViewModel :ObservableObject
    {
        private readonly IExpenseUtility _expenseUtility;
        private readonly DataStore _store;
        private double _totalExpenditureAmount;


        public double TotalExpenditureAmount
        {
            get => _totalExpenditureAmount;
            set => SetProperty(ref _totalExpenditureAmount, value);
        }

        public ExpenseDetailedViewUserControlViewModel(IExpenseUtility expenseUtility,DataStore store)
        {
            _expenseUtility = expenseUtility;
            _store = store;

        }

        public void ExpenseObjLoaded(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return;
            }
            _expenseUtility.GetRelatedExpenses(expenseObj,_store.UserBobj ,RelatedExpensesCallBack);

        }
        private async void RelatedExpensesCallBack(IEnumerable<ExpenseBobj> relatedExpenses)
        {
          await UiService.RunOnUiThread(() =>
          {
               var totalExpense = relatedExpenses.Sum(relatedExpense => relatedExpense.StrExpenseAmount);

               TotalExpenditureAmount = totalExpense;

          }).ConfigureAwait(false);
        }


    }
}
