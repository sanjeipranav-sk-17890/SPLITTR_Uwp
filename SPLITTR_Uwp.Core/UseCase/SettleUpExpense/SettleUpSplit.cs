using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.SettleUpExpense;

public class SettleUpSplit : UseCaseBase<SettleUpExpenseResponseObj>
{
    private readonly SettleUPExpenseRequestObj _requestObj;
    private readonly ISettleUpSplitDataManager _dataManager;
    public SettleUpSplit(SettleUPExpenseRequestObj requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
    {
        _requestObj = requestObj;
        _dataManager =SplittrDependencyService.GetInstance<ISettleUpSplitDataManager>();
    }
    protected override void Action()
    {
        _dataManager.SettleUpExpenses(_requestObj.SettleExpense,_requestObj.CurrentUser,new UseCaseCallBackBase<SettleUpExpenseResponseObj>(this),_requestObj.IsWalletTransaction);
    }
}