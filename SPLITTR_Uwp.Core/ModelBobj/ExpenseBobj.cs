using System;
using System.Collections.Generic;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.ModelBobj;

public class ExpenseBobj : Expense
{
    private readonly ExpenseCategory _expenseCategory;


    public static ICurrencyConverter CurrencyConverter { get; set; }

    public User CorrespondingUserObj { get; set; }

    public virtual ExpenseStatus ExpenseStatus
    {
        get => (ExpenseStatus)ExpenseStatusIndex;
        set => ExpenseStatusIndex = (int)value;
    }

    public User SplitRaisedOwner { get; set; }

    public  double StrExpenseAmount
    {
        get => CurrencyConverter.ConvertCurrency(base.ExpenseAmount);
        set => base.ExpenseAmount = CurrencyConverter.ConvertToEntityCurrency(value);
    }

    private ExpenseBobj(Expense expense) : base(expense.Description,expense.ExpenseAmount ,expense.RequestedOwner,dateOfExpense: expense.DateOfExpense,createdDate:expense.CreatedDate ,expense.Note, expense.GroupUniqueId, expense.ExpenseStatusIndex, expense.ExpenseUniqueId, expense.UserEmailId, expense.ParentExpenseId,expense.CategoryId)
    {
        

    }

    public ExpenseBobj(User correspondingUser,User splitRaisedOwner, Expense expense) : this(expense)
    {
        CorrespondingUserObj = correspondingUser;
        SplitRaisedOwner = splitRaisedOwner;
    }
    public ExpenseBobj(ExpenseBobj expenseBobj) : this(expenseBobj.CorrespondingUserObj,expenseBobj.SplitRaisedOwner, expenseBobj)
    {

    }
    public ExpenseBobj(ICurrencyConverter currencyConverter)
    {
        CurrencyConverter = currencyConverter;
        
    }

}
public class ExpenseDateSorter : IComparer<ExpenseBobj>
{

    public int Compare(ExpenseBobj x, ExpenseBobj y)
    {
        if (x is null || y is null)
        {
            return 0;
        }

        return DateTime.Compare(x.CreatedDate, y.CreatedDate);
    }

}