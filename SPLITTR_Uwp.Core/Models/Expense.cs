using SQLite;
using System;

namespace SPLITTR_Uwp.Core.Models;

public class Expense
{
    private double _expenseAmount;
    private string _note;
    private string _description;
    private DateTime _dateOfExpense;

    [PrimaryKey, Unique]
    public string ExpenseUniqueId { get; set; }

    public int ExpenseStatusIndex { get; set; }


    public virtual string Description
    {
        get => _description;
        set => _description = value;
    }

    /// <summary>
    /// with Backing feild because it should not call Bobjs or ViewModel's Overrided feilds
    /// </summary>
    public virtual double ExpenseAmount
    {
        get => _expenseAmount;
        set => _expenseAmount = value;
    }

    public string RequestedOwner { get; set; }

    public virtual DateTime DateOfExpense
    {
        get => _dateOfExpense;
        set => _dateOfExpense = value;
    }

    public DateTime CreatedDate { get; set; }

    public virtual string Note
    {
        get => _note;
        set => _note = value;
    }

    public string GroupUniqueId { get; set; }

    public string ParentExpenseId { get; set; }

    public string UserEmailId { get; set; }

    public int CategoryId { get; set; } = 18;

    public Expense()
    {
        ExpenseUniqueId = Guid.NewGuid().ToString();
        CreatedDate = DateTime.Now;
    }

    public override bool Equals(object obj)
    {
        if (obj is Expense expense)
        {
            return ExpenseUniqueId.Equals(expense.ExpenseUniqueId);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public Expense(string description, double expenseAmount, string requestedOwner, DateTime dateOfExpense, DateTime createdDate, string note, string groupUniqueId, int expenseStatus, string expenseUniqueId, string userEmailId, string parentExpenseId, int categoryId)
    {
        _description = description;
        _expenseAmount = expenseAmount;
        RequestedOwner = requestedOwner;
        _dateOfExpense = dateOfExpense;
        _note = note;
        CategoryId = categoryId;
        CreatedDate = createdDate;
        GroupUniqueId = groupUniqueId;
        UserEmailId = userEmailId;
        ExpenseStatusIndex = expenseStatus;
        ExpenseUniqueId = expenseUniqueId;
        ParentExpenseId = parentExpenseId;
    }



}