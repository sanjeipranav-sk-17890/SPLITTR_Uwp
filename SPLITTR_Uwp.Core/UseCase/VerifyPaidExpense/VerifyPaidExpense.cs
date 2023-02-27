using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;

public class VerifyPaidExpense : UseCaseBase<VerifyPaidExpenseResponseObj>
{
    private readonly VerifyPaidExpenseRequestObj _requestObj;
    private readonly IExpenseHistoryManager _dataManager;
    public VerifyPaidExpense(VerifyPaidExpenseRequestObj requestObj):base(requestObj.PresenterCallBack,requestObj.Cts)
    {
        _requestObj = requestObj;
        _dataManager = SplittrDependencyService.GetInstance<IExpenseHistoryManager>();

    }
   protected override void Action()
    {
        _dataManager.IsExpenseMarkedAsPaid(_requestObj.ExpenseUniqueId,new UseCaseCallBackBase<VerifyPaidExpenseResponseObj>(this));
    }
}
