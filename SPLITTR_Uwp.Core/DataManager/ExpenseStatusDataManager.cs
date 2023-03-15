using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrEventArgs;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CancelExpense;
using SPLITTR_Uwp.Core.UseCase.MarkAsPaid;
using SPLITTR_Uwp.Core.UseCase.SplitExpenses;
using SQLite;

namespace SPLITTR_Uwp.Core.DataManager;

public class ExpenseStatusDataManager : ISplitExpenseDataManager, IMarkExpensePaidDataManager, IExpenseCancellationDataManager
{
    private readonly IExpenseDataManager _expenseDataManager;
    private readonly IExpenseHistoryManager _expenseHistoryManager;
    private readonly IUserDataManager _userDataManager;


    public async void CancelExpense(ExpenseBobj expenseToBeCancelled, UserBobj currentUser, IUseCaseCallBack<CancelExpenseResponseObj> callBack)
    {
        try
        {
            var cancelledExpenseObj = await ChangeExpenseStatus(expenseToBeCancelled, currentUser, ExpenseStatus.Cancelled).ConfigureAwait(false);

            callBack?.OnSuccess(new CancelExpenseResponseObj(cancelledExpenseObj));

            SplittrNotification.InvokeExpenseStatusChanged(new ExpenseStatusChangedEventArgs(ExpenseStatus.Cancelled,expenseToBeCancelled));
        }
        catch (SQLiteException ex)
        {
            callBack?.OnError(new SplittrException(ex, "Db Fetch Error"));
        }
        catch (ArgumentException ex)
        {
            callBack?.OnError(new SplittrException(ex, "Owner of Passed Expense Didn't match Current User"));
        }
        catch (Exception ex)
        {
            callBack?.OnError(new SplittrException(ex, ex.Message));
        }
    }



    public async void MarkExpenseAsPaid(ExpenseBobj expenseToBeMarkedAsPaid, UserBobj currentUser, IUseCaseCallBack<MarkAsPaidResponseObj> callBack)
    {
        try
        {
            var paidExpenseObj = await ChangeExpenseStatus(expenseToBeMarkedAsPaid, currentUser, ExpenseStatus.Paid).ConfigureAwait(false);
            //storing record about that expense in dataService
            _expenseHistoryManager.RecordExpenseMarkedAsPaid(expenseToBeMarkedAsPaid.ExpenseUniqueId);

            callBack?.OnSuccess(new MarkAsPaidResponseObj(paidExpenseObj));

            //raising Status Changed Notification
            SplittrNotification.InvokeExpenseStatusChanged(new ExpenseStatusChangedEventArgs(ExpenseStatus.Paid,expenseToBeMarkedAsPaid));
        }
        catch (SQLiteException e)
        {
            callBack?.OnError(new SplittrException(e, "Db Fetch Error"));
        }


    }

    private async Task<ExpenseBobj> ChangeExpenseStatus(ExpenseBobj statusChangeExpense, UserBobj currentUser, ExpenseStatus status)
    {
        //Fetching expense obj with matching id
        var expenseStatusChangeBobj = statusChangeExpense;

        expenseStatusChangeBobj.ExpenseStatus = status;

        await _expenseDataManager.UpdateExpenseAsync(expenseStatusChangeBobj).ConfigureAwait(false);

        //Updating User Lent and Owing Amount
        await UpdateCreditDetails(expenseStatusChangeBobj, currentUser).ConfigureAwait(false);

        return expenseStatusChangeBobj;

    }

    private async Task UpdateCreditDetails(ExpenseBobj expenseStatusChangeBobj, UserBobj currentUser)
    {
        var requestOwner = expenseStatusChangeBobj.SplitRaisedOwner;
        var correspondingUser = expenseStatusChangeBobj.CorrespondingUserObj;

        if (currentUser.Equals(requestOwner))//assign to Current USerBobj So Change Notification raised
        {
            currentUser.StrLentAmount -= expenseStatusChangeBobj.ExpenseAmount;
            requestOwner = currentUser;
        }
        else
        {
            requestOwner.LentAmount -= expenseStatusChangeBobj.ExpenseAmount;
        }
        if (currentUser.Equals(correspondingUser))
        {
            currentUser.StrOwingAmount -= expenseStatusChangeBobj.ExpenseAmount;
            correspondingUser = currentUser;
        }
        else
        {
            correspondingUser.OwingAmount -= expenseStatusChangeBobj.ExpenseAmount;
        }


        await _userDataManager.UpdateUserBobjAsync(requestOwner).ConfigureAwait(false);
        await _userDataManager.UpdateUserBobjAsync(correspondingUser).ConfigureAwait(false);
    }

    private Task UpdateDebitDetails(IEnumerable<ExpenseBobj> expenses, UserBobj splitOwner)
    {
        var updatingUsers = new List<User>();

        foreach (var expense in expenses)
        {
            if (expense.ExpenseStatus != ExpenseStatus.Pending)
            {
                continue;
            }
            var correspondingUser = expense.CorrespondingUserObj;
            splitOwner.StrLentAmount += expense.StrExpenseAmount;
            correspondingUser.OwingAmount += expense.ExpenseAmount;
            updatingUsers.Add(correspondingUser);
        }
        updatingUsers.Add(splitOwner);

        var userUpdation = updatingUsers.Select(u => _userDataManager.UpdateUserBobjAsync(u));

        return Task.WhenAll(userUpdation);
    }


    public ExpenseStatusDataManager(IExpenseDataManager expenseDataManager, IExpenseHistoryManager expenseHistoryManager, IUserDataManager userDataManager)
    {
        _expenseDataManager = expenseDataManager;
        _expenseHistoryManager = expenseHistoryManager;
        _userDataManager = userDataManager;

    }

    private ExpenseBobj ValidateExpenseBobjs(string expenseDescription, IEnumerable<ExpenseBobj> expenses, string expenseNote, double expenseAmount, DateTime dateOfExpense, int expenditureSplitType)
    {
        //verify description
        if (string.IsNullOrEmpty(expenseDescription))
        {
            throw new ArgumentException("Expense Title Cannot be Empty");
        }
        ExpenseBobj parentExpenseBobj = null;
        foreach (var expense in expenses)
        {

            //expenditureSplitType is equal split if expenditureSplitType is <=0 for equal split , expenditure amount cannot be negative 
            if (expenseAmount <= 0 && expenditureSplitType <= 0)
            {
                throw new ArgumentException("Equal Split Money must be greater than zero");
            }
            if (expense.StrExpenseAmount <= 0 && expenditureSplitType > 0)
            {
                throw new ArgumentException("Minimum Split Amount is 0.1 ");
            }

            expense.Note = expenseNote;
            expense.Description = expenseDescription;
            expense.DateOfExpense = dateOfExpense;
            if (expense.RequestedOwner.Equals(expense.UserEmailId) && parentExpenseBobj is null)//expenseStatus for split raiser is always paid
            {
                parentExpenseBobj = expense;
                parentExpenseBobj.ExpenseStatus = ExpenseStatus.Paid;
            }

        }
        return parentExpenseBobj;

    }



    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public async void SplitNewExpensesAsync(string expenseDescription, UserBobj currentUser, IEnumerable<ExpenseBobj> expenses, string expenseNote, DateTime dateOfExpense, double expenseAmount, int expenditureSplitType, IUseCaseCallBack<SplitExpenseResponseObj> callBack)
    {
        try
        {

            ExpenseBobj parentExpenseBobj = ValidateExpenseBobjs(expenseDescription, expenses, expenseNote, expenseAmount, dateOfExpense, expenditureSplitType);

            var noOfExpenses = expenses.Count();
            //assiging parentExpenditure Id to remaining Expenses except parent Expenses
            foreach (var expense in expenses)
            {
                if (expenditureSplitType <= 0) // assinging equal expense amount if equal split option is selected
                {
                    expense.StrExpenseAmount = expenseAmount / noOfExpenses;
                }
                if (expense.ExpenseUniqueId.Equals(parentExpenseBobj.ExpenseUniqueId) is not true) //expenseStatus for split raiser is pending for others  
                {
                    expense.ParentExpenseId = parentExpenseBobj.ExpenseUniqueId;
                    expense.ExpenseStatus = ExpenseStatus.Pending;

                    if (expense.StrExpenseAmount == 0) //if split amount is 0 mark it as paid
                    {
                        expense.ExpenseStatus = ExpenseStatus.Paid;
                    }
                }

            }

            await _expenseDataManager.InsertExpenseAsync(expenses);

            await UpdateDebitDetails(expenses, currentUser);

            //Calling Process Success Call back
            callBack?.OnSuccess(new SplitExpenseResponseObj(expenses));

            //Invoking New ExpensesHas been Split
            SplittrNotification.InvokeExpensesSplit(new ExpenseSplittedEventArgs(expenses));
        }
        catch (ArgumentException ex)
        {
            callBack?.OnError(new SplittrException(ex, ex.Message));
        }
        catch (SQLiteException ex)
        {
            callBack?.OnError(new SplittrException(ex, "Db Fetch Error"));
        }

    }


}
