using SQLite;
using System;
using System.Linq;
using System.Text;

namespace SPLITTR_Uwp.Core.Models
{
    public class Expense
    {
        private double _expenseAmount;
        private string _note;

        [PrimaryKey, Unique]
        public string ExpenseUniqueId { get; set; }

        public int ExpenseStatusindex { get; set; }


        /// <summary>
        /// with Backing feild because it should not call Bobjs or ViewModel's Overrided feilds
        /// </summary>
        public virtual double ExpenseAmount
        {
            get => _expenseAmount;
            set => _expenseAmount = value;
        }

        public string RequestedOwner { get; set; }

        public DateTime DateOfExpense { get; set; }

        public virtual string Note
        {
            get => _note;
            set => _note = value;
        }

        public string GroupUniqueId { get; set; }

        public string ParentExpenseId { get; set; }

        public string UserEmailId { get; set; }



        public Expense()
        {
            ExpenseUniqueId = GenerateUniqueId();
        }




        public Expense(double expenseAmount, string requestedOwner, DateTime dateOfExpense, string note, string groupUniqueId, int expenseStatus, string expenseUniqueId, string userEmailId, string parentExpenseId)
        {
            _expenseAmount = expenseAmount;
            RequestedOwner = requestedOwner;
            DateOfExpense = dateOfExpense;
            _note = note;
            GroupUniqueId = groupUniqueId;
            UserEmailId = userEmailId;
            ExpenseStatusindex = expenseStatus;
            ExpenseUniqueId = expenseUniqueId;
            ParentExpenseId = parentExpenseId;
        }


        private string GenerateUniqueId()
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString();
            return id;
        }

    }
}
