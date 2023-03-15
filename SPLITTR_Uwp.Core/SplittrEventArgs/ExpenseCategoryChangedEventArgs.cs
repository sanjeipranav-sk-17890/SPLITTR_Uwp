using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class ExpenseCategoryChangedEventArgs : SplittrEventArgs
{
    public ExpenseCategoryChangedEventArgs(ExpenseBobj updatedExpenseBobj, ExpenseCategory changedCategory)
    {
        this.UpdatedExpenseBobj = updatedExpenseBobj;
        ChangedCategory = changedCategory;
    }

    public ExpenseBobj UpdatedExpenseBobj { get; }
    public ExpenseCategory ChangedCategory { get; }
}
