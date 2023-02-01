using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.SqliteConnection;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts
{
    
    public class ExpensePayment : UseCaseBase, IExpensePayment
    {
        private readonly IUserDataManager _userDataManager;
        private readonly ISqlDataServices _sqlDataServices;
        private readonly IExpenseDataHandler _expenseDataHandler;

        public ExpensePayment(IUserDataManager userDataManager, ISqlDataServices sqlDataServices, IExpenseDataHandler expenseDataHandler)
        {
            _userDataManager = userDataManager;
            _sqlDataServices = sqlDataServices;
            _expenseDataHandler = expenseDataHandler;

        }


        private void ValidateInputs(ExpenseBobj settleExpenseRef, UserBobj currentUser)
        {

            if (settleExpenseRef is null || currentUser is null)
            {
                throw new ArgumentException("One Or more Passed Argument is Null");
            }
            //current user cannot pay their own Expense 
            if (settleExpenseRef.SplitRaisedOwner.Equals(currentUser))
            {
                throw new NotSupportedException();
            }
        }

        /// <exception cref="NotSupportedException">thrown if Insufficient Wallet Balance</exception>
        public void SettleUpExpenses(ExpenseBobj settleExpenseRef, UserBobj currentUser, Action successCallBack, bool isWalletTransaction = false)
        {
            RunAsynchronously(async () =>
            {

                ValidateInputs(settleExpenseRef, currentUser);

                var toBeSettledExpenseObj = currentUser.Expenses.FirstOrDefault(ex => ex.ExpenseUniqueId.Equals(settleExpenseRef.ExpenseUniqueId));

                if (toBeSettledExpenseObj is null)
                {
                    return;
                }
                var requestedOwner = toBeSettledExpenseObj.SplitRaisedOwner;

                if (isWalletTransaction)
                {
                    if (currentUser.StrWalletBalance < toBeSettledExpenseObj.StrExpenseAmount)
                    {
                        throw new NotSupportedException("Insufficient Wallet Balance");

                    }
                    currentUser.WalletBalance -= toBeSettledExpenseObj.ExpenseAmount;

                    await _userDataManager.UpdateUserBobjAsync(currentUser).ConfigureAwait(false);

                }
                requestedOwner.WalletBalance += toBeSettledExpenseObj.ExpenseAmount;
                toBeSettledExpenseObj.ExpenseStatus = ExpenseStatus.Paid;


                await _sqlDataServices.RunInTransaction(() =>
                {
                    _userDataManager.UpdateUserBobjAsync(requestedOwner);
                    _expenseDataHandler.UpdateExpenseAsync(toBeSettledExpenseObj);
                }).ConfigureAwait(false);

                currentUser.Expenses.RemoveAndAdd(toBeSettledExpenseObj);

                successCallBack?.Invoke();

            });
        }

    }
}
