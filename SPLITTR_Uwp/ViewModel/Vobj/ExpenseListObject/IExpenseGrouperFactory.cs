using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.ViewModel.VmLogic;

namespace SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject
{
    public interface IExpenseGrouperFactory
    {
        public IExpenseGrouper GetGrouperInstance(string groupName);
    }
    public class ExpenseGrouperFactory : IExpenseGrouperFactory
    {
        private readonly ExpenseCategoryGrouper _categoryGrouper;
        private readonly ExpenseStatusGrouper _statusGrouper;

        public ExpenseGrouperFactory(ExpenseCategoryGrouper categoryGrouper,ExpenseStatusGrouper statusGrouper)
        {
            _categoryGrouper = categoryGrouper;
            _statusGrouper = statusGrouper;

        }

        public IExpenseGrouper GetGrouperInstance(string groupName)
        {
            return groupName switch
            {
                "Category" => _categoryGrouper,
                 _ => _statusGrouper
            };
        }
    }
}
