using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class UserBobj :User
    {
        private double _lendedAmount = 0.0;
        private double _pendingAmount = 0.0;
        
        private readonly IUserBobjBalanceCalculator _balanceCalculator;
        public  ICurrencyConverter CurrencyConverter { get; set; }
        public event Action ValueChanged;


        public ICollection<ExpenseBobj> Expenses { get; private set; } = new List<ExpenseBobj>();

        public ICollection<GroupBobj> Groups { get; private set; } = new List<GroupBobj>();


        /// <summary>
        /// Gets or Sets the walletBalance in Respective Currency Preference
        /// </summary>
        // marked as New because when updating this object it is sended to method which takes parent User as parameter so in order to avoid 
        //reading Wrapper WalletBalance We marked it as new , marked as virtual so WalletBalance property can be overrided by 
        //userVm and it should Provide info to UI through Composition of UserBobj
        public new virtual double WalletBalance
        {
            get => CurrencyConverter.ConvertCurrency(base.WalletBalance);
            set
            {
                base.WalletBalance = CurrencyConverter.ConvertToEntityCurrency(value);
                OnValueChanged(); 
            }

        }

        public Currency CurrencyPreference
        {
            get => (Currency)CurrencyIndex;
            set
            {
               CurrencyIndex = (int)value;
               OnValueChanged();
            }
        }

        public double LendedAmount
        {
            get
            {
                _balanceCalculator.ReCalculate(this);
                return _lendedAmount;

            }
            set => _lendedAmount = value;
            // OnValueChanged();
        }

        public double PendingAmount
        {
            get
            {
                _balanceCalculator.ReCalculate(this);
                return _pendingAmount;

            }
            set => _pendingAmount = value;
            // OnValueChanged();
        }

        protected void OnValueChanged()
        {
            ValueChanged?.Invoke();
        }


        public UserBobj(User user,ICollection<ExpenseBobj> expenses,ICollection<GroupBobj> groups, IUserBobjBalanceCalculator balanceCalculator,ICurrencyConverter currencyConverter )
            :base(user.EmailId,user.UserName,user.WalletBalance,user.CurrencyIndex)
        {
            _balanceCalculator = balanceCalculator;
            CurrencyConverter = currencyConverter;
            Expenses.AddRange(expenses);
            Groups.AddRange(groups);
        }

        protected UserBobj(UserBobj userBobj) : this(userBobj,userBobj.Expenses,userBobj.Groups,userBobj._balanceCalculator,userBobj.CurrencyConverter)
        {

        }

    }
}
