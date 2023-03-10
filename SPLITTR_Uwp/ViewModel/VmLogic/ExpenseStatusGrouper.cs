using System;
using System.Collections.Generic;
using System.Linq;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.ViewModel.Vobj;
using SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject;

namespace SPLITTR_Uwp.ViewModel.VmLogic;

public class ExpenseStatusGrouper : IExpenseGrouper
{

    private readonly IEnumerable<ExpenseBobj> _emptyList = new List<ExpenseBobj>().DefaultIfEmpty();
    private readonly ExpenseGroupingList _emptyPaidExpensesGroup;
    private readonly ExpenseGroupingList _emptyPendingExpensesGroup;
    private readonly ExpenseGroupingList _emptyCancelledList;



    public ExpenseStatusGrouper()
    {
        _emptyPaidExpensesGroup =  new ExpenseGroupingList(ExpenseStatus.Paid.ToString(), _emptyList);
        _emptyPendingExpensesGroup = new ExpenseGroupingList(ExpenseStatus.Pending.ToString(), _emptyList);
        _emptyCancelledList = new ExpenseGroupingList(ExpenseStatus.Cancelled.ToString(), _emptyList);

    }

    public IEnumerable<ExpenseGroupingList> CreateExpenseGroupList(IEnumerable<ExpenseVobj> expenses)
    {
        //grouped Expenses based on Types into ExpenseGroupingList
        var groupedExpenses = expenses.GroupBy(e => e.ExpenseStatus).Select(grouped => new ExpenseGroupingList(grouped.Key.ToString(), grouped)).ToList();

        //if list not contains a particular specific Dummy grouping is added with empty list for Ui Purposes
        if (groupedExpenses.Count == 3)
        {
            return groupedExpenses;
        }
        if (!groupedExpenses.Contains(_emptyCancelledList))
        {
            groupedExpenses.Add(_emptyCancelledList);
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