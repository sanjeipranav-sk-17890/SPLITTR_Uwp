using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SQLite;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

public class ExpenseUtility : UseCaseBase, IExpenseUtility
{
    private readonly IExpenseDataHandler _expenseDataHandler;

    public event Action<EventArgs> PresenterCallBackOnSuccess;


    public ExpenseUtility(IExpenseDataHandler expenseDataHandler)
    {
        _expenseDataHandler = expenseDataHandler;

    }

    private ExpenseBobj ValidateExpenseBobjs(IEnumerable<ExpenseBobj> expenses, string expenseNote, double expenseAmount, DateTime dateOfExpense, int expenditureSplitType)
    {

        ExpenseBobj parentExpenseBobj = null;
        foreach (var expense in expenses)
        {

            //expenditureSplitType is equal split if expenditureSplitType is <=0 for equal split , expenditure amount cannot be negative 
            if (expenseAmount <= 0 && expenditureSplitType <= 0)
            {
                throw new ArgumentException("Equal Split Money must be greater than zero");
            }
            if (expense.StrExpenseAmount <= -1)
            {
                throw new ArgumentException("Money cannot be  negative");
            }

            expense.Note = expenseNote;
            expense.DateOfExpense = dateOfExpense;
            if (expense.RequestedOwner.Equals(expense.UserEmailId) && parentExpenseBobj is null)//expenseStatus for split raiser is always paid
            {
                parentExpenseBobj = expense;
                parentExpenseBobj.ExpenseStatus = ExpenseStatus.Paid;
            }

        }
        return parentExpenseBobj;

    }



    public async Task SplitNewExpensesAsync(UserBobj currentUser, IEnumerable<ExpenseBobj> expenses, string expenseNote, DateTime dateOfExpense, double expenseAmount, int expenditureSplitType)
    {
        await RunAsynchronously(() =>
         {
             ExpenseBobj parentExpenseBobj = ValidateExpenseBobjs(expenses, expenseNote, expenseAmount, dateOfExpense, expenditureSplitType);

             var noOfExpenses = expenses.Count();
             //assiging parentExpenditure Id to remaining Expenses except parent Expenses
             foreach (var expense in expenses)
             {
                 if (expenditureSplitType <= 0)// assinging equal expense amount if equal split option is selected
                 {
                     expense.StrExpenseAmount = expenseAmount / noOfExpenses;
                 }
                 if (expense.ExpenseUniqueId.Equals(parentExpenseBobj.ExpenseUniqueId) is not true)//expenseStatus for split raiser is pending for others
                 {
                     expense.ParentExpenseId = parentExpenseBobj.ExpenseUniqueId;
                     expense.ExpenseStatus = ExpenseStatus.Pending;
                 }

             }

             _expenseDataHandler.InsertExpenseAsync(expenses);

             //adds expenseObjs into 
             currentUser.Expenses.AddRange(expenses);

             //Calling Process Success Call back
             PresenterCallBackOnSuccess?.Invoke(SplittrEventArgs.Empty);
         });
        ;
    }

}
