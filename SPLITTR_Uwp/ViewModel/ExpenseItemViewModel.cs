using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    internal class ExpenseItemViewModel
    {
        

        public ExpenseItemViewModel()
        {
            
        }

        private string GetGroupNameByGroupId(string groupUniqueId)
        {
            if (groupUniqueId is null)
            {
                return String.Empty;
            }

            string groupName = String.Empty;
            foreach (var group in Store.CurreUserBobj.Groups)
            {
                if (group.GroupUniqueId.Equals(groupUniqueId))
                {
                    groupName = group.GroupName;
                    break;
                }
            }
            return groupName;

        }


        public string FormatExpenseTitle(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return String.Empty;
            }
            if (expenseObj.GroupUniqueId is not null)
            {
                
                return GetGroupNameByGroupId(expenseObj.GroupUniqueId);
            }
            //If Current user is Owner Showing the Name as You instead of Name
            if (expenseObj.SplitRaisedOwner.Equals(Store.CurreUserBobj))
            {
                return "You";
            }
            return expenseObj.SplitRaisedOwner?.UserName ?? string.Empty;
        }

        public string FormatExpenseAmount(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return String.Empty;
            }
            
            string expenseAmount = expenseObj.ExpenseAmount.ToString();
            if (expenseAmount.Length > 7)
            {
                expenseAmount = expenseAmount.Substring(0, 7);
            }
            if (expenseObj.SplitRaisedOwner.Equals(Store.CurreUserBobj))
            {
                return "+ " + expenseAmount;
            }
            return "- " + expenseAmount;

        }

    }
}
