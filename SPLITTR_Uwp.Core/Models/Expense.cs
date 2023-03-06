using System;
using SQLite;

namespace SPLITTR_Uwp.Core.Models;

public class Expense
{
    private double _expenseAmount;
    private string _note;
    private int _categoryId = 18;

    [PrimaryKey, Unique]
    public string ExpenseUniqueId { get; set; }

    public int ExpenseStatusIndex { get; set; }


    public string Description { get; set; }

    /// <summary>
    /// with Backing feild because it should not call Bobjs or ViewModel's Overrided feilds
    /// </summary>
    public virtual double ExpenseAmount
    {
        get => _expenseAmount;
        set => _expenseAmount = value;
    }

    public string RequestedOwner { get; set; }

    public DateTime DateOfExpense { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual string Note
    {
        get => _note;
        set => _note = value;
    }

    public string GroupUniqueId { get; set; }

    public string ParentExpenseId { get; set; }

    public string UserEmailId { get; set; }

    public int CategoryId
    {
        get => _categoryId;
        set => _categoryId = value;
    }

    public Expense()
    {
        ExpenseUniqueId = Guid.NewGuid().ToString();
        CreatedDate = DateTime.Now;
    }




    public Expense(string description,double expenseAmount, string requestedOwner, DateTime dateOfExpense,DateTime createdDate,string note, string groupUniqueId, int expenseStatus, string expenseUniqueId, string userEmailId, string parentExpenseId,int categoryId)
    {
        Description = description;
        _expenseAmount = expenseAmount;
        RequestedOwner = requestedOwner;
        DateOfExpense = dateOfExpense;
        _note = note;
        _categoryId = categoryId;
        CreatedDate = createdDate;
        GroupUniqueId = groupUniqueId;
        UserEmailId = userEmailId;
        ExpenseStatusIndex = expenseStatus;
        ExpenseUniqueId = expenseUniqueId;
        ParentExpenseId = parentExpenseId;
    }



}