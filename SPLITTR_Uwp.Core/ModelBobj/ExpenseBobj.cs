using System;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class ExpenseBobj : Expense
    {
        

        public ICurrencyConverter CurrencyConverter { get; set; }

        public User UserDetails { get; set; }

        public virtual ExpenseStatus ExpenseStatus
        {
            get => (ExpenseStatus)ExpenseStatusindex;
            set
            {
                ExpenseStatusindex = (int)value;
                OnValueChanged();
            }
        }




        public event Action ValueChanged;


        public  double StrExpenseAmount
        {
            get => CurrencyConverter.ConvertCurrency(base.ExpenseAmount);
            set => base.ExpenseAmount = CurrencyConverter.ConvertToEntityCurrency(value);
        }

        protected void OnValueChanged()
        {
            ValueChanged?.Invoke();
        }

        private ExpenseBobj(Expense expense) : base(expense.ExpenseAmount, expense.RequestedOwner, expense.DateOfExpense, expense.Note, expense.GroupUniqueId, expense.ExpenseStatusindex, expense.ExpenseUniqueId, expense.UserEmailId, expense.ParentExpenseId)
        {

        }

        public ExpenseBobj(User user, ICurrencyConverter currencyConverter, Expense expense) : this(expense)
        {
            UserDetails = user;
            CurrencyConverter = currencyConverter;

        }
        public ExpenseBobj(ExpenseBobj expenseBobj) : this(expenseBobj.UserDetails, expenseBobj.CurrencyConverter, expenseBobj)
        {

        }
        public ExpenseBobj(ICurrencyConverter currencyConverter)
        {
            CurrencyConverter = currencyConverter;
        }

    }
}
