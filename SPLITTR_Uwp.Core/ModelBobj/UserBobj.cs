using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class UserBobj : User
    {
        private double _lentAmount = 0.0;
        private double _pendingAmount = 0.0;
        private readonly ObservableCollection<ExpenseBobj> _expenses = new ObservableCollection<ExpenseBobj>();
        private readonly ObservableCollection<GroupBobj> _groups = new ObservableCollection<GroupBobj>();

        private readonly IUserBobjBalanceCalculator _balanceCalculator;

        public ICurrencyConverter CurrencyConverter { get; set; }
        public event Action<string> ValueChanged;


        public ICollection<ExpenseBobj> Expenses
        {
            get => _expenses;
        }

        public ICollection<GroupBobj> Groups
        {
            get => _groups;
        }


        /// <summary>
        /// Gets or Sets the walletBalance in Respective Currency Preference
        /// </summary>
        public virtual double StrWalletBalance
        {
            get => CurrencyConverter.ConvertCurrency(base.WalletBalance);
            set
            {
                base.WalletBalance = CurrencyConverter.ConvertToEntityCurrency(value);
                OnValueChanged(nameof(StrWalletBalance));
            }

        }

        public Currency CurrencyPreference
        {
            get => (Currency)CurrencyIndex;
            set
            {
                CurrencyIndex = (int)value;
                OnValueChanged(nameof(CurrencyPreference));
            }
        }

        public double LentAmount
        {
            get
            {
                _balanceCalculator.ReCalculate(this);
                return _lentAmount;

            }
            set => SetField(ref _lentAmount, value);
        }

        public double PendingAmount
        {
            get
            {
                _balanceCalculator.ReCalculate(this);
                return _pendingAmount;

            }
            set => SetField(ref _pendingAmount, value);
        }

        protected void OnValueChanged(string property)
        {
            ValueChanged?.Invoke(property);
        }


        public UserBobj(User user, ICollection<ExpenseBobj> expenses, ICollection<GroupBobj> groups, IUserBobjBalanceCalculator balanceCalculator, ICurrencyConverter currencyConverter)
            : base(user.EmailId, user.UserName, user.WalletBalance, user.CurrencyIndex)
        {
            _balanceCalculator = balanceCalculator;
            CurrencyConverter = currencyConverter;
            Expenses.AddRange(expenses);
            Groups.AddRange(groups);

            _groups.CollectionChanged += GroupCollectionChanged;
            _expenses.CollectionChanged += ExpenseCollectionChanged;
        }
        private void ExpenseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
          OnValueChanged(nameof(Expenses));
        }

        private void GroupCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //calling value changed  event when collection is modified
            OnValueChanged(nameof(Groups));
        }

        protected UserBobj(UserBobj userBobj) : this(userBobj, userBobj.Expenses, userBobj.Groups, userBobj._balanceCalculator, userBobj.CurrencyConverter)
        {

        }


        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
           OnValueChanged(propertyName);
            return true;
        }

    }
   
    
}
