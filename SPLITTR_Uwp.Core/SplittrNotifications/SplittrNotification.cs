using System;
using System.Collections.Generic;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.SplittrNotifications;

public static class SplittrNotification
{

    public static event Action<UserBobjUpdatedEventArgs> UserObjUpdated;

    public static event Action<CurrencyPreferenceChangedEventArgs> CurrencyPreferenceChanged;

    public static event Action<ExpenseStatusChangedEventArgs> ExpenseStatusChanged;

    public static event Action<GroupCreatedEventArgs> GroupCreated;

    public static event Action<ExpenseSplittedEventArgs> ExpensesSplitted;

    public static event Action<ExpenseCategoryChangedEventArgs> ExpenseCategoryChanged;
    

    internal static void InvokeUserObjUpdated(UserBobjUpdatedEventArgs obj)
    {
        UserObjUpdated?.Invoke(obj);
    }

    internal static void InvokePreferenceChanged(CurrencyPreferenceChangedEventArgs obj)
    {
        CurrencyPreferenceChanged?.Invoke(obj);
    }
    internal static void InvokeExpenseStatusChanged(ExpenseStatusChangedEventArgs obj)
    {
        ExpenseStatusChanged?.Invoke(obj);
    }
    internal static void InvokeGroupCreated(GroupCreatedEventArgs obj)
    {
        GroupCreated?.Invoke(obj);
    }

    internal static void InvokeExpensesSplit(ExpenseSplittedEventArgs obj)
    {
        ExpensesSplitted?.Invoke(obj);
    }
    internal static void InvokeExpenseCategoryChanged(ExpenseCategoryChangedEventArgs obj)
    {
        ExpenseCategoryChanged?.Invoke(obj);
    }
}

public class ExpenseCategoryChangedEventArgs : SplittrEventArgs
{
    public ExpenseCategoryChangedEventArgs(ExpenseBobj updatedExpenseBobj, ExpenseCategory changedCategory)
    {
        this.UpdatedExpenseBobj = updatedExpenseBobj;
        ChangedCategory = changedCategory;
    }

    public ExpenseBobj UpdatedExpenseBobj { get; }
    public ExpenseCategory ChangedCategory { get; }
}

public class ExpenseSplittedEventArgs : SplittrEventArgs
{
    public IEnumerable<ExpenseBobj> NewExpenses { get;}


    public ExpenseSplittedEventArgs(IEnumerable<ExpenseBobj> newExpenses)
    {
        NewExpenses = newExpenses;
    }

}
public class GroupCreatedEventArgs : SplittrEventArgs
{
    public GroupCreatedEventArgs(GroupBobj createdGroup)
    {
        CreatedGroup = createdGroup;
    }

    public GroupBobj CreatedGroup { get; }
}
public class ExpenseStatusChangedEventArgs : SplittrEventArgs
{
    public ExpenseStatus ExpenseStatus { get; }

    public ExpenseBobj StatusChangedExpense { get; }

    public ExpenseStatusChangedEventArgs(ExpenseStatus expenseStatus, ExpenseBobj statusChangedExpense)
    {
        ExpenseStatus = expenseStatus;
        StatusChangedExpense = statusChangedExpense;
    }

}
public class CurrencyPreferenceChangedEventArgs :SplittrEventArgs
{
    public Currency PreferredCurrency { get; }

    public CurrencyPreferenceChangedEventArgs(Currency preferedCurrency)
    {
        PreferredCurrency = preferedCurrency;
    }
}

public class SplittrEventArgs : EventArgs
{

}
public class UserBobjUpdatedEventArgs : SplittrEventArgs
{
    public UserBobj UpdatedUser { get;}

    public UserBobjUpdatedEventArgs(UserBobj updatedUser)
    {
        UpdatedUser = updatedUser;
    }
}