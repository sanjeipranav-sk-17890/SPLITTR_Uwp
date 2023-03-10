using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.DataTemplates.Controls.ExpenseListControl;

/// <summary>
/// Encapsulate Request obj To used by UserList Expense Control Parent 
/// </summary>
public class ExpenseListFilterObj
{
    public ExpenseListFilterObj(Group group1, User user, ExpenseFilter filterType)
    {
        Group = group1;
        User = user;
        FilterType = filterType;
    }

    public Group Group { get; }

    public User User { get; }

    public ExpenseFilter FilterType { get; }
    public enum ExpenseFilter
    {
        RequestByMe,
        RequestToMe,
        AllExpenses,
        GroupExpense,
        UserExpense,
    }
}
