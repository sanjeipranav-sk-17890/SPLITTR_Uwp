using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense;

public class CancelExpense :UseCaseBase<CancelExpenseResponseObj>
{
    private readonly CancelExpenseRequestObj _requestObj;
    private readonly IExpenseCancellationDataManager _dataManager;
    public CancelExpense(CancelExpenseRequestObj requestObj) : base(requestObj.CallBack,requestObj.Cts)
    {
        _requestObj = requestObj;
        _dataManager = SplittrDependencyService.GetInstance<IExpenseCancellationDataManager>();
    }
    protected override void Action()
    {
        _dataManager.CancelExpense(_requestObj.ExpenseToBeCancelled,_requestObj.CurrentUser,new UseCaseCallBackBase<CancelExpenseResponseObj>(this));
    }
}