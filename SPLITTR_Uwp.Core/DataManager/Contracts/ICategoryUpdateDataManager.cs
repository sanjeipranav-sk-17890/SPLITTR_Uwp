using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.ChangeExpenseCategory;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface ICategoryUpdateDataManager
{
    void UpdateExpenseCategory(ExpenseCategory category, ExpenseBobj expenseToBeUpdated, User currentUser, IUseCaseCallBack<ChangeExpenseCategoryResponse> callBack);
}