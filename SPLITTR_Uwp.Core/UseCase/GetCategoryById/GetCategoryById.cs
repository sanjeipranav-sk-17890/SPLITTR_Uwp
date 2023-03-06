using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase.SplitExpenses;

namespace SPLITTR_Uwp.Core.UseCase.GetCategoryById
{
    public class GetCategoryById : UseCaseBase<GetCategoryByIdResponse>
    {
        private readonly GetCategoryByIdReq _request;
        private readonly IExpenseCategoryManager _expenseCategoryManager;

        public GetCategoryById(GetCategoryByIdReq request) : base(request.PresenterCallBack,request.Cts)
        {
            _request = request;
            _expenseCategoryManager = SplittrDependencyService.GetInstance<IExpenseCategoryManager>();
        }
        protected override void Action()
        {
            _expenseCategoryManager.FetchExpenseCategoryById(_request.RequestedCategoryId,new UseCaseCallBackBase<GetCategoryByIdResponse>(this));
        }

    }
    public class GetCategoryByIdReq : SplittrRequestBase<GetCategoryByIdResponse>
    {
        public int RequestedCategoryId { get; }

        public GetCategoryByIdReq(CancellationToken cts, IPresenterCallBack<GetCategoryByIdResponse> presenterCallBack, int requestedCategoryId) : base(cts, presenterCallBack)
        {
            RequestedCategoryId = requestedCategoryId;
        }
    }
    public class GetCategoryByIdResponse
    {
        public GetCategoryByIdResponse(ExpenseCategory requestedCategoryObj)
        {
            RequestedCategoryObj = requestedCategoryObj;
        }

        public ExpenseCategory RequestedCategoryObj { get; }
    }

}
