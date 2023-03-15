using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class ExpenseStatusChangedEventArgs : SplittrEventArgs
{
    public ExpenseStatus ExpenseStatus { get; }

    public ExpenseBobj StatusChangedExpense { get; }

    public ExpenseStatusChangedEventArgs(ExpenseStatus expenseStatus, ExpenseBobj statusChangedExpense)
    {
        ExpenseStatus = expenseStatus;
        StatusChangedExpense = statusChangedExpense;
    }

}
