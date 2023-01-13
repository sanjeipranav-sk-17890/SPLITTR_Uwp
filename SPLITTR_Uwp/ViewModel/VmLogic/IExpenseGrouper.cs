using System.Collections.Generic;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;

namespace SPLITTR_Uwp.ViewModel.VmLogic
{
    internal interface IExpenseGrouper
    {
        IEnumerable<ExpenseGroupingList> CreateExpenseGroupList(IEnumerable<ExpenseBobj> expenses);
    }
}
