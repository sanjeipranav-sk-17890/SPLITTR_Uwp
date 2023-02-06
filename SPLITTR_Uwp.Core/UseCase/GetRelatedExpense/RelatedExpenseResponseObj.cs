using System.Collections.Generic;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;

public class RelatedExpenseResponseObj
{
    public IEnumerable<ExpenseBobj> RelatedExpenses { get; }


    public RelatedExpenseResponseObj(IEnumerable<ExpenseBobj> relatedExpenses)
    {
        RelatedExpenses = relatedExpenses;
    }


}
