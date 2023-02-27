using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserExpenses;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface IExpenseFetchDataManager
{
    void GetUserExpensesAsync(User user, IUseCaseCallBack<GetExpensesByIdResponse> callBack);
}
