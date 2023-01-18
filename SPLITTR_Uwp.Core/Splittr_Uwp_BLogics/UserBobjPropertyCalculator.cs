using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics
{
    public class UserBobjPropertyCalculator : IUserBobjBalanceCalculator
    {
        public void ReCalculate(UserBobj userBobj)
        {
            var lendedAmount = 0.0;
            var pendingAmount = 0.0;

            foreach (var expense in userBobj.Expenses)
            {

                if (expense.ExpenseStatus == ExpenseStatus.Pending &&
                    expense.RequestedOwner == userBobj.EmailId)
                {
                    lendedAmount += expense.StrExpenseAmount;
                }

                if (expense.RequestedOwner != userBobj.EmailId &&
                    expense.ExpenseStatus == ExpenseStatus.Pending)
                {
                    pendingAmount += expense.StrExpenseAmount;
                }

            }
            userBobj.LentAmount = lendedAmount;
            userBobj.PendingAmount = pendingAmount;
        }
    }
}
