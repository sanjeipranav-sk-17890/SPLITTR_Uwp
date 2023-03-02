using System.Collections.Generic;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory
{
    public class FetchExpenseCategory : UseCaseBase<FetchExpenseCategoryResponse>
    {
        private readonly FetchExpenseCategoryRequest _request;
        private readonly IExpenseCategoryManager _categoryManager;
        public FetchExpenseCategory(FetchExpenseCategoryRequest request) : base(request.PresenterCallBack,request.Cts)
        {
            _request = request;
            _categoryManager = SplittrDependencyService.GetInstance<IExpenseCategoryManager>();
        }
        protected override void Action()
        {
           _categoryManager.FetchExpenseCategory(new FetchExpenseCategoryUsCallBack(this));
        }

        private class FetchExpenseCategoryUsCallBack : IUseCaseCallBack<FetchExpenseCategoryResponse>
        {
            private readonly FetchExpenseCategory _useCase;

            public FetchExpenseCategoryUsCallBack(FetchExpenseCategory useCase)
            {
                _useCase = useCase;

            }
            public void OnSuccess(FetchExpenseCategoryResponse responseObj)
            {
               _useCase?.PresenterCallBack?.OnSuccess(responseObj);
            }
            public void OnError(SplittrException error)
            {
                _useCase?.PresenterCallBack?.OnError(error);
            }
        }
    }
    public class FetchExpenseCategoryRequest : SplittrRequestBase<FetchExpenseCategoryResponse>
    {
        public FetchExpenseCategoryRequest(CancellationToken cts, IPresenterCallBack<FetchExpenseCategoryResponse> presenterCallBack) : base(cts, presenterCallBack)
        {

        }
    }

    public class FetchExpenseCategoryResponse
    {
        public IEnumerable<ExpenseCategoryBobj> Categories { get; }

        public FetchExpenseCategoryResponse(IEnumerable<ExpenseCategoryBobj> categories)
        {
            Categories = categories;
        }
    }

}
