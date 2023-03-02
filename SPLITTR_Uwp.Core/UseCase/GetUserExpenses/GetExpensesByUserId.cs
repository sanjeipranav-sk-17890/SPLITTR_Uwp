using System.Collections.Generic;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase.GetUserExpenses;

public class GetExpensesByUserId :UseCaseBase<GetExpensesByIdResponse>
{
    private readonly GetExpensesByIdRequest _requestObj;
    private readonly IExpenseFetchDataManager _fetchDataManager;

    public GetExpensesByUserId(GetExpensesByIdRequest requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
    {
        _requestObj = requestObj;
        _fetchDataManager = SplittrDependencyService.GetInstance<IExpenseFetchDataManager>();
    }
    protected override void Action()
    {
        _fetchDataManager.GetUserExpensesAsync(_requestObj.CurrentUser,new GetExpenseByUserIdCb(this));
    }

    private class GetExpenseByUserIdCb :IUseCaseCallBack<GetExpensesByIdResponse>
    {
        private readonly GetExpensesByUserId _useCase;

        public GetExpenseByUserIdCb(GetExpensesByUserId useCase)
        {
            _useCase = useCase;
        }
        public void OnSuccess(GetExpensesByIdResponse responseObj)
        {
            _useCase?.PresenterCallBack?.OnSuccess(responseObj);   
        }
        public void OnError(SplittrException error)
        {
            _useCase?.PresenterCallBack?.OnError(error);
        }
    }
        

}
public class GetExpensesByIdRequest : SplittrRequestBase<GetExpensesByIdResponse>
{

    public User CurrentUser { get; }

    public GetExpensesByIdRequest(CancellationToken cts, IPresenterCallBack<GetExpensesByIdResponse> presenterCallBack, User currentUser) : base(cts, presenterCallBack)
    {
        CurrentUser = currentUser;
    }
}
public class GetExpensesByIdResponse
{
    public GetExpensesByIdResponse(IEnumerable<ExpenseBobj> currentUserExpenses)
    {
        CurrentUserExpenses = currentUserExpenses;
    }

    public IEnumerable<ExpenseBobj> CurrentUserExpenses { get; }
}