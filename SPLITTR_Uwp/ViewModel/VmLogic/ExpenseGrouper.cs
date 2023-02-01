using System.Collections.Generic;
using System.Linq;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;

namespace SPLITTR_Uwp.ViewModel.VmLogic
{
    internal class ExpenseGrouper : IExpenseGrouper
    {

        private IEnumerable<ExpenseBobj> _emptyList = new List<ExpenseBobj>().DefaultIfEmpty();
        private ExpenseGroupingList _emptyPaidExpensesGroup;
        private ExpenseGroupingList _emptyPendingExpensesGroup;
        private ExpenseGroupingList _emptyCancellledList;



        public ExpenseGrouper()
        {
            _emptyPaidExpensesGroup =  new ExpenseGroupingList(ExpenseStatus.Paid, _emptyList);
            _emptyPendingExpensesGroup = new ExpenseGroupingList(ExpenseStatus.Pending, _emptyList);
            _emptyCancellledList = new ExpenseGroupingList(ExpenseStatus.Cancelled, _emptyList);

        }

        public IEnumerable<ExpenseGroupingList> CreateExpenseGroupList(IEnumerable<ExpenseBobj> expenses)
        {
            //grouped Expenses based on Types into ExpenseGroupingList
            var groupedExpenses = expenses.GroupBy(e => e.ExpenseStatus).Select(grouped => new ExpenseGroupingList(grouped.Key, grouped)).ToList();


            //if list not contains a particualar specific Dummy grouping is added with empty list for Ui Purposes
            if (groupedExpenses.Count == 3)
            {
                return groupedExpenses;
            }
            if (!groupedExpenses.Contains(_emptyCancellledList))
            {
                groupedExpenses.Add(_emptyCancellledList);
            }
            if (!groupedExpenses.Contains(_emptyPaidExpensesGroup))
            {
                groupedExpenses.Add(_emptyPaidExpensesGroup);
                
            }
            if (!groupedExpenses.Contains(_emptyPendingExpensesGroup))
            {
                groupedExpenses.Add(_emptyPendingExpensesGroup);
            }
            return groupedExpenses;

        }

    }

}
