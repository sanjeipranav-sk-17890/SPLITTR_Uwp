using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;

public class VerifyPaidExpenseRequestObj : IRequestObj<VerifyPaidExpenseResponseObj>
{
    public string ExpenseUniqueId { get; }

    public VerifyPaidExpenseRequestObj(string expenseUniqueId, CancellationToken cts, IPresenterCallBack<VerifyPaidExpenseResponseObj> presenterCallBack)
    {
        ExpenseUniqueId = expenseUniqueId;
        Cts = cts;
        PresenterCallBack = presenterCallBack;
    }

    public CancellationToken Cts { get; }

    public IPresenterCallBack<VerifyPaidExpenseResponseObj> PresenterCallBack { get; }
}