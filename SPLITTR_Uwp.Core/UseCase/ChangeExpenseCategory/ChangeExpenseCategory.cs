using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.UseCase.ChangeExpenseCategory
{
    public class ChangeExpenseCategory : UseCaseBase<ChangeExpenseCategoryResponse>
    {
        private readonly ChangeExpenseCategoryReq _request;
        private readonly ICategoryUpdateDataManager _updateDataManager;
        public ChangeExpenseCategory(ChangeExpenseCategoryReq request) : base(request.PresenterCallBack, request.Cts)
        {
            _request = request;
            _updateDataManager = SplittrDependencyService.GetInstance<ICategoryUpdateDataManager>();
        }
        protected override void Action()
        {
           _updateDataManager.UpdateExpenseCategory(_request.NewExpenseCategory,_request.CategoryChangeExpense,_request.CurrentUser,new UseCaseCallBackBase<ChangeExpenseCategoryResponse>(this));
        }

    }
    public class ChangeExpenseCategoryReq : SplittrRequestBase<ChangeExpenseCategoryResponse>
    {
        public ExpenseBobj CategoryChangeExpense { get; }

        public ExpenseCategory NewExpenseCategory { get; }

        public User CurrentUser { get; }

        public ChangeExpenseCategoryReq(CancellationToken cts, IPresenterCallBack<ChangeExpenseCategoryResponse> presenterCallBack, ExpenseBobj categoryChangeExpense, ExpenseCategory newExpenseCategory, User currentUser) : base(cts, presenterCallBack)
        {
            CategoryChangeExpense = categoryChangeExpense;
            NewExpenseCategory = newExpenseCategory;
            CurrentUser = currentUser;
        }
    }
    public  class ChangeExpenseCategoryResponse
    {
        public ChangeExpenseCategoryResponse(ExpenseCategory changedExpenseCategory, ExpenseBobj updatedExpenseBobj)
        {
            ChangedExpenseCategory = changedExpenseCategory;
            UpdatedExpenseBobj = updatedExpenseBobj;
        }

        public ExpenseCategory ChangedExpenseCategory { get; }

        public  ExpenseBobj UpdatedExpenseBobj { get; }
    }


}
