using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SQLite;

namespace SPLITTR_Uwp.Core.UseCase.SplitExpenses
{
    public class SplitExpenseRequestObj : IRequestObj<SplitExpenseResponseObj>
    {
        public SplitExpenseRequestObj(string expenseDescription, UserBobj currentUser, IEnumerable<ExpenseBobj> expenses, string expenseNote, DateTime dateOfExpense, double expenseAmount, int expenseSplitType, CancellationToken cts, IPresenterCallBack<SplitExpenseResponseObj> presenterCallBack)
        {
            ExpenseDescription = expenseDescription;
            CurrentUser = currentUser;
            Expenses = expenses;
            ExpenseNote = expenseNote;
            DateOfExpense = dateOfExpense;
            ExpenseAmount = expenseAmount;
            ExpenseSplitType = expenseSplitType;
            Cts = cts;
            PresenterCallBack = presenterCallBack;
        }

        public  string ExpenseDescription { get;  }

        public UserBobj CurrentUser { get;  }

        public IEnumerable<ExpenseBobj> Expenses { get; }

        public string ExpenseNote { get; }

        public DateTime DateOfExpense { get; }

        public double ExpenseAmount { get; }

        public int ExpenseSplitType { get; }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<SplitExpenseResponseObj> PresenterCallBack { get; }
    }
    public class SplitExpenseResponseObj
    {
        public SplitExpenseResponseObj(IEnumerable<ExpenseBobj> splittedExpenseBObjs)
        {
            SplittedExpenseBObjs = splittedExpenseBObjs;
        }

        public  IEnumerable<ExpenseBobj> SplittedExpenseBObjs { get; }
    }
    public class SplitExpenses : UseCaseBase<SplitExpenseResponseObj>
    {
        private readonly ISplitExpenseDataManager _dataManager;
        private readonly SplitExpenseRequestObj _requestObj;

        public SplitExpenses(SplitExpenseRequestObj requestObj, ISplitExpenseDataManager dataManager) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = dataManager;
        }
        public override void Action()
        {
           _dataManager.SplitNewExpensesAsync(_requestObj.ExpenseDescription,_requestObj.CurrentUser,_requestObj.Expenses,_requestObj.ExpenseNote,_requestObj.DateOfExpense,_requestObj.ExpenseAmount,_requestObj.ExpenseSplitType,new UseCaseCallBackBase<SplitExpenseResponseObj>(this)); 
        }
      
    }

}
