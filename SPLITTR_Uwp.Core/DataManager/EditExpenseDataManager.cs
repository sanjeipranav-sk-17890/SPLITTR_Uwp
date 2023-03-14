using System;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.EditExpense;

namespace SPLITTR_Uwp.Core.DataManager;

public class EditExpenseDataManager : IEditExpenseDataManager
{
    private readonly IRelatedExpenseDataManager _relatedExpenseDataManager;
    private readonly IExpenseDataManager _expenseDataManager;

    public EditExpenseDataManager(IRelatedExpenseDataManager relatedExpenseDataManager, IExpenseDataManager expenseDataManager)
    {
        _relatedExpenseDataManager = relatedExpenseDataManager;
        _expenseDataManager = expenseDataManager;
    }

    public async void EditExpense(ExpenseBobj expenseToBeEdited, UserBobj currentUser, string newExpenseNote, string newExpenseTitle, DateTime newDateOfExpense, IUseCaseCallBack<EditExpenseResponse> callBack)
    {
        try
        {
            await EditExpense(expenseToBeEdited, currentUser, newExpenseNote, newExpenseTitle, newDateOfExpense).ConfigureAwait(false);

            callBack?.OnSuccess(new EditExpenseResponse(expenseToBeEdited));
        }
        catch (Exception e)
        {
            callBack?.OnError(new SplittrException(e));
        }
    }
    async Task EditExpense(ExpenseBobj expenseToBeEdited, UserBobj currentUser, string newExpenseNote, string newExpenseTitle, DateTime newDateOfExpense)
    {
        ThrowIfInvalidRequest(expenseToBeEdited, newExpenseNote, newExpenseTitle, newDateOfExpense);

        var relatedExpense = await _relatedExpenseDataManager.GetRelatedExpenses(expenseToBeEdited, currentUser).ConfigureAwait(false);

        var updateExpenseTask = relatedExpense.Select(UpdateExpenseDetails);

        await UpdateExpenseDetails(expenseToBeEdited).ConfigureAwait(false);
        
        await Task.WhenAll(updateExpenseTask).ConfigureAwait(false);

        foreach (var editedExpense in relatedExpense)
        {
            SplittrNotification.InvokeExpenseObjEditedEvent(new ExpenseEditedEventArgs(editedExpense)); 
        }

        Task UpdateExpenseDetails(ExpenseBobj ex)
        {
            ex.DateOfExpense = newDateOfExpense;
            ex.Description = newExpenseTitle;
            ex.Note = newExpenseNote;
            return _expenseDataManager.UpdateExpenseAsync(ex);
        }
    }
    private void ThrowIfInvalidRequest(ExpenseBobj expenseToBeEdited, string newExpenseNote, string newExpenseTitle, DateTime newDateOfExpense)
    {
        if (expenseToBeEdited == null)
        {
            throw new ArgumentNullException(nameof(expenseToBeEdited));
        }
        if (string.IsNullOrEmpty(newExpenseTitle))
        {
            throw new ArgumentNullException(nameof(newExpenseTitle) + " Expense Title Cannot Be Empty");
        }

    }


}
