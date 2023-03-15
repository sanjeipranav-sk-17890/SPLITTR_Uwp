using System;
using SPLITTR_Uwp.Core.SplittrEventArgs;

namespace SPLITTR_Uwp.Core.SplittrNotifications;

public static class SplittrNotification
{

    public static event Action<UserBobjUpdatedEventArgs> UserObjUpdated;

    public static event Action<CurrencyPreferenceChangedEventArgs> CurrencyPreferenceChanged;

    public static event Action<ExpenseStatusChangedEventArgs> ExpenseStatusChanged;

    public static event Action<GroupCreatedEventArgs> GroupCreated;

    public static event Action<ExpenseSplittedEventArgs> ExpensesSplitted;

    public static event Action<ExpenseCategoryChangedEventArgs> ExpenseCategoryChanged;

    public static event Action<ExpenseEditedEventArgs> ExpenseEdited;

    internal static void InvokeExpenseObjEditedEvent(ExpenseEditedEventArgs obj)
    {
        ExpenseEdited?.Invoke(obj);
    }

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