using System;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrEventArgs;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.ChangeExpenseCategory;

namespace SPLITTR_Uwp.Core.DataManager;

public class CategoryUpdateDataManager : ICategoryUpdateDataManager
{
    private readonly IExpenseDbHandler _expenseDbHandler;
    public CategoryUpdateDataManager(IExpenseDbHandler expenseDbHandler)
    {
        _expenseDbHandler = expenseDbHandler;

    }
    public async void UpdateExpenseCategory(ExpenseCategory category, ExpenseBobj expenseToBeUpdated, User currentUser, IUseCaseCallBack<ChangeExpenseCategoryResponse> callBack)
    {
        try
        {
            await  UpdateExpenseCategory(category, expenseToBeUpdated, currentUser).ConfigureAwait(false);

            callBack?.OnSuccess(new ChangeExpenseCategoryResponse(category,expenseToBeUpdated));

            SplittrNotification.InvokeExpenseCategoryChanged(new ExpenseCategoryChangedEventArgs(expenseToBeUpdated,category));
        }
        catch (Exception ex)
        {
            callBack?.OnError(new SplittrException(ex));
        }
    }
    private Task UpdateExpenseCategory(ExpenseCategory category, ExpenseBobj expenseToBeUpdated, User currentUser)
    {
        if (! expenseToBeUpdated.SplitRaisedOwner.Equals(currentUser))
        {
            throw new InvalidExpenseUpdationException();
        }
        expenseToBeUpdated.CategoryId = category.Id;

        return _expenseDbHandler.UpdateExpenseAsync(expenseToBeUpdated);
    }
}
