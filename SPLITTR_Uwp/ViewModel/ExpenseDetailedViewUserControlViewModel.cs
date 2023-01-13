using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
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
        private double _totalExpenditureAmount;

        public ObservableCollection<ExpenseViewModel> RelatedExpenses { get; } = new ObservableCollection<ExpenseViewModel>();

        public double TotalExpenditureAmount
        {
            get => _totalExpenditureAmount;
            set => SetProperty(ref _totalExpenditureAmount, value);
        }

        public ExpenseDetailedViewUserControlViewModel(IExpenseUtility expenseUtility)
        {
            _expenseUtility = expenseUtility;

        }

        //Storing Reference Of passed Expense ,utilized to make Ui manupulation
        private ExpenseViewModel _expense;
        public void ExpenseObjLoaded(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return;
            }
            _expense = expenseObj;

            _expenseUtility.GetRelatedExpenses(expenseObj,Store.CurreUserBobj ,RelatedExpensesCallBack);

        }
        private async void RelatedExpensesCallBack(IEnumerable<ExpenseBobj> relatedExpenses)
        {
          await UiService.RunOnUiThread(() =>
          {
               
               PopulateRelatedExpenses(relatedExpenses);

          }).ConfigureAwait(false);
        }
        private void PopulateRelatedExpenses(IEnumerable<ExpenseBobj> relatedExpenses)
        {
            //sum of all expenses before split 
            TotalExpenditureAmount = relatedExpenses.Sum(relatedExpense => relatedExpense.StrExpenseAmount) + _expense.StrExpenseAmount;

            RelatedExpenses.Clear();

            RelatedExpenses.Add(_expense);

            var expenseVMobjs = relatedExpenses.Select(ex => new ExpenseViewModel(ex));

            RelatedExpenses.AddRange(expenseVMobjs);

        }


        public string FetchGroupName(string groupUniqueId)
        {
            if (groupUniqueId is null)
            {
                return string.Empty;
            }
            var groupName=string.Empty;
            foreach (var userGroup in Store.CurreUserBobj.Groups)
            {
                if (groupUniqueId.Equals(userGroup.GroupUniqueId))
                {
                  groupName= userGroup.GroupName;
                }
                
            }
            return groupName;
        }
    }
}
