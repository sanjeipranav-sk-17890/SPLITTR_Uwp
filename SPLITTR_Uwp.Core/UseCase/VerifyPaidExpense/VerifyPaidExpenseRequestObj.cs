using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;

public class VerifyPaidExpenseRequestObj : SplittrRequestBase<VerifyPaidExpenseResponseObj>
{
    public string ExpenseUniqueId { get; }

    public VerifyPaidExpenseRequestObj(string expenseUniqueId, CancellationToken cts, IPresenterCallBack<VerifyPaidExpenseResponseObj> presenterCallBack) : base(cts, presenterCallBack)
    {
        ExpenseUniqueId = expenseUniqueId;
    }
}