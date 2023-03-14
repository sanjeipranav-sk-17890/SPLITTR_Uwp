using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.EditExpense;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{
    internal interface IEditExpenseDataManager
    {
        public void EditExpense(ExpenseBobj expenseToBeEdited, UserBobj currentUser,string newExpenseNote, string newExpenseTitle, DateTime newDateOfExpense, IUseCaseCallBack<EditExpenseResponse> callBack);

    }

}
