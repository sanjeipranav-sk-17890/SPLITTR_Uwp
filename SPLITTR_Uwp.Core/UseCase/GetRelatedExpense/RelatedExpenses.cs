using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.GetRelatedExpense
{
    public class RelatedExpense :UseCaseBase<RelatedExpenseResponseObj>
    {
        private readonly RelatedExpenseRequestObj _requestObj;
        private readonly IRelatedExpenseDataManager _dataManager;

        public RelatedExpense(RelatedExpenseRequestObj requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = SplittrDependencyService.GetInstance<IRelatedExpenseDataManager>();
        }
       protected override void Action()
        {
           _dataManager.GetRelatedExpenses(_requestObj.ReferenceExpense,_requestObj.CurrentUser,new UseCaseCallBackBase<RelatedExpenseResponseObj>(this));
        }
    }
}
