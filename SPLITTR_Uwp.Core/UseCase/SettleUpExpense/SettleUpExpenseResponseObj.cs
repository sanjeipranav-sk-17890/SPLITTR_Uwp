using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.SettleUpExpense;

public class SettleUpExpenseResponseObj
{
    public SettleUpExpenseResponseObj(ExpenseBobj settledExpenseObj)
    {
        SettledExpenseObj = settledExpenseObj;
    }

    public ExpenseBobj SettledExpenseObj { get;  }
}