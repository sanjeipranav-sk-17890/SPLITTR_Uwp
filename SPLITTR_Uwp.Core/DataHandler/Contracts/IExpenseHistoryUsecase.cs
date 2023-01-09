using System;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.DataHandler.Contracts;

public interface IExpenseHistoryUsecase
{
    void IsExpenseMarkedAsPaid(string expenseId,Action<bool> ResultCallBack);
}
