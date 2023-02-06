using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.SettleUpExpense;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface ISettleUpSplitDataManager
{
    public void SettleUpExpenses(ExpenseBobj settleExpenseRef, UserBobj currentUser, IUseCaseCallBack<SettleUpExpenseResponseObj> callBack, bool isWalletTransaction = false);

}
