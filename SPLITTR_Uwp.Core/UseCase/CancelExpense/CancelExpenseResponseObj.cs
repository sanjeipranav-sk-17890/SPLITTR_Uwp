using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense;

public class CancelExpenseResponseObj
{
    public CancelExpenseResponseObj(ExpenseBobj cancelledExpense)
    {
        CancelledExpense = cancelledExpense;
    }

    public ExpenseBobj CancelledExpense { get; }
}