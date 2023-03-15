using System.Collections.Generic;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class ExpenseSplittedEventArgs : SplittrEventArgs
{
    public IEnumerable<ExpenseBobj> NewExpenses { get;}


    public ExpenseSplittedEventArgs(IEnumerable<ExpenseBobj> newExpenses)
    {
        NewExpenses = newExpenses;
    }

}
