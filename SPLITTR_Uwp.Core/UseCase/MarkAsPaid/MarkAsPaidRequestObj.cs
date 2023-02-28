using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.MarkAsPaid;

public class MarkAsPaidRequestObj : SplittrRequestBase<MarkAsPaidResponseObj>
{
    public MarkAsPaidRequestObj(IPresenterCallBack<MarkAsPaidResponseObj> presenterCallBack, CancellationToken cts, UserBobj userBobj, ExpenseBobj expenseToBeMarkedAsPaid) : base(cts,presenterCallBack)
    {
        ExpenseToBeMarkedAsPaid = expenseToBeMarkedAsPaid;
        CurrentUser = userBobj;
        ExpenseToBeMarkedAsPaid = expenseToBeMarkedAsPaid;
    }
    public ExpenseBobj ExpenseToBeMarkedAsPaid { get; }

    public UserBobj CurrentUser { get; }

}