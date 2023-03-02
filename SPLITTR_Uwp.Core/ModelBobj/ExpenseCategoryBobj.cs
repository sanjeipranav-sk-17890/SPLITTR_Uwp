using System.Collections.Generic;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class ExpenseCategoryBobj : ExpenseCategory
    {
        public IEnumerable<ExpenseCategory> SubExpenseCategories { get; }

        public ExpenseCategoryBobj(ExpenseCategory baseCategory,IEnumerable<ExpenseCategory> subExpenseCategories):base(baseCategory)
        {
            SubExpenseCategories = subExpenseCategories;
        }

    }
}
