using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface IRelatedExpenseDataManager
{
    void GetRelatedExpenses(ExpenseBobj referenceExpense, UserBobj currentUser,IUseCaseCallBack<RelatedExpenseResponseObj> callBack);

}
