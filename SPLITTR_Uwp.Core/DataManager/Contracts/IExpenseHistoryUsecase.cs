using System;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface IExpenseHistoryUsecase
{
    void IsExpenseMarkedAsPaid(string expenseId,Action<bool> resultCallBack);
}
