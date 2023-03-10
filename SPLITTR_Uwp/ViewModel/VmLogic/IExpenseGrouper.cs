using System.Collections.Generic;
using SPLITTR_Uwp.ViewModel.Vobj;
using SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject;

namespace SPLITTR_Uwp.ViewModel.VmLogic;

public interface IExpenseGrouper
{
    IEnumerable<ExpenseGroupingList> CreateExpenseGroupList(IEnumerable<ExpenseVobj> expenses);
}