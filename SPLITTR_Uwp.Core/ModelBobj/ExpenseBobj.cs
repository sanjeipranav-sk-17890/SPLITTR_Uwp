using System;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class ExpenseBobj : Expense
    {
        

        public ICurrencyConverter CurrencyConverter { get; set; }

        public User CorrespondingUserObj { get; set; }

        public virtual ExpenseStatus ExpenseStatus
        {
            get => (ExpenseStatus)ExpenseStatusindex;
            set
            {
                ExpenseStatusindex = (int)value;
                OnValueChanged(nameof(ExpenseStatus));
            }
        }

        public User SplitRaisedOwner { get; set; }


        public virtual event Action<string> ValueChanged;


        public  double StrExpenseAmount
        {
            get => CurrencyConverter.ConvertCurrency(base.ExpenseAmount);
            set => base.ExpenseAmount = CurrencyConverter.ConvertToEntityCurrency(value);
        }

        protected void OnValueChanged(string property)
        {
            ValueChanged?.Invoke(property);
        }

        private ExpenseBobj(Expense expense) : base(expense.Description,expense.ExpenseAmount ,expense.RequestedOwner,dateOfExpense: expense.DateOfExpense,createdDate:expense.CreatedDate ,expense.Note, expense.GroupUniqueId, expense.ExpenseStatusindex, expense.ExpenseUniqueId, expense.UserEmailId, expense.ParentExpenseId)
        {

        }

        public ExpenseBobj(User correspondingUser,User splitRaisedOwner,ICurrencyConverter currencyConverter, Expense expense) : this(expense)
        {
            CorrespondingUserObj = correspondingUser;
            SplitRaisedOwner = splitRaisedOwner;
            CurrencyConverter = currencyConverter;

        }
        public ExpenseBobj(ExpenseBobj expenseBobj) : this(expenseBobj.CorrespondingUserObj,expenseBobj.SplitRaisedOwner ,expenseBobj.CurrencyConverter, expenseBobj)
        {

        }
        public ExpenseBobj(ICurrencyConverter currencyConverter)
        {
            CurrencyConverter = currencyConverter;
        }

    }
    public class ExpenseDateSorter : IComparer<ExpenseBobj>
    {

        public int Compare(ExpenseBobj x, ExpenseBobj y)
        {
            if (x is null || y is null)
            {
                return 0;
            }

            return DateTime.Compare(x.CreatedDate, y.CreatedDate);
        }

    }
}
