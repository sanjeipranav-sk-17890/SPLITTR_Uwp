using SPLITTR_Uwp.ViewModel.VmLogic;

namespace SPLITTR_Uwp.DataTemplates.Controls.ExpenseListControl;

public class ExpenseListGroupingObj
{
    public ExpenseListGroupingObj(IExpenseGrouper grouperObj, string groupByName)
    {
        GrouperObj = grouperObj;
        GroupByName = groupByName;
    }

    public string GroupByName { get; }
    public IExpenseGrouper GrouperObj { get; }
}
