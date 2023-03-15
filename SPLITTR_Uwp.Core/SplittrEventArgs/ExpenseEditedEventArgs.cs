using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class ExpenseEditedEventArgs : SplittrEventArgs
{
    public ExpenseEditedEventArgs(ExpenseBobj editedExpenseObj)
    {
        EditedExpenseObj = editedExpenseObj;
    }

    public ExpenseBobj EditedExpenseObj { get;}
}
