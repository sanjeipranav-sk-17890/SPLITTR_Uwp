using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.MarkAsPaid
{
    public class MarkAsPaid :UseCaseBase<MarkAsPaidResponseObj>
    {
        private readonly MarkAsPaidRequestObj _requestObj;
        private readonly IMarkExpensePaidDataManager _dataManager;

        public MarkAsPaid(MarkAsPaidRequestObj requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager =SplittrDependencyService.GetInstance<IMarkExpensePaidDataManager>();
        }
       protected override void Action()
        {
           _dataManager.MarkExpenseAsPaid(_requestObj.ExpenseToBeMarkedAsPaid,_requestObj.CurrentUser,new UseCaseCallBackBase<MarkAsPaidResponseObj>(this));
        }
    }
    public interface IMarkExpensePaidDataManager
    {
        void MarkExpenseAsPaid(ExpenseBobj expenseToBeMarkedAsPaid, UserBobj currentUser,IUseCaseCallBack<MarkAsPaidResponseObj> callBack);

    }
}
