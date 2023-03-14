using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase.EditExpense
{
    public class EditExpense : UseCaseBase<EditExpenseResponse>
    {
        private readonly EditExpenseRequest _request;

        private readonly IEditExpenseDataManager _dataManager;

        public EditExpense(EditExpenseRequest request) : base(request.PresenterCallBack,request.Cts)
        {
            _request = request;
            _dataManager = SplittrDependencyService.GetInstance<IEditExpenseDataManager>();
        }
        protected override void Action()
        {
           _dataManager.EditExpense(_request.ExpenseToBeEdited,_request.CurrentUser,_request.NewExpenseNote,_request.NewExpenseTitle,_request.NewDateOfExpense,new EditExpenseCb(this));
        }


        class EditExpenseCb : IUseCaseCallBack<EditExpenseResponse>
        {
            private readonly EditExpense _useCase;

            public EditExpenseCb(EditExpense useCase)
            {
                _useCase = useCase;

            }
            public void OnSuccess(EditExpenseResponse responseObj)
            {
                _useCase?.PresenterCallBack?.OnSuccess(responseObj);
            }
            public void OnError(SplittrException error)
            {
               _useCase?.PresenterCallBack?.OnError(error);
            }
        }
    }
    public class EditExpenseRequest : SplittrRequestBase<EditExpenseResponse>
    {
        public ExpenseBobj ExpenseToBeEdited { get; }

        public string NewExpenseNote { get; }

        public string NewExpenseTitle { get; }

        public DateTime NewDateOfExpense { get; }

        public UserBobj CurrentUser { get; }

        public EditExpenseRequest(CancellationToken cts, IPresenterCallBack<EditExpenseResponse> presenterCallBack, DateTime newDateOfExpense, string newExpenseTitle, string newExpenseNote, ExpenseBobj expenseToBeEdited, UserBobj currentUser) : base(cts, presenterCallBack)
        {
            NewDateOfExpense = newDateOfExpense;
            NewExpenseTitle = newExpenseTitle;
            NewExpenseNote = newExpenseNote;
            ExpenseToBeEdited = expenseToBeEdited;
            CurrentUser = currentUser;

        }
    }
    public class EditExpenseResponse
    {
        public ExpenseBobj EditedExpenseObj { get; }

        public EditExpenseResponse(ExpenseBobj updatedExpenseObj)
        {
            EditedExpenseObj = updatedExpenseObj;
        }

    }
}
