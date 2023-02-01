using System;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;

public interface IExpensePayment : IUseCase
{

    /// <param name="toBeSettledExpense"></param>
    /// <param name="isWalletTransaction"></param>
    /// <param name="currentUser"></param>
    /// <param name="successCallBack"></param>
    void SettleUpExpenses(ExpenseBobj toBeSettledExpense, UserBobj currentUser, Action successCallBack, bool isWalletTransaction = false);
}
