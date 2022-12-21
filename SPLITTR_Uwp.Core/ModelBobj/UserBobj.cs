using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class UserBobj : User
    {
        private double _lendedAmount = 0.0;
        private double _pendingAmount = 0.0;
        private ObservableCollection<ExpenseBobj> _expenses = new ObservableCollection<ExpenseBobj>();
        private ObservableCollection<GroupBobj> _groups = new ObservableCollection<GroupBobj>();

        private readonly IUserBobjBalanceCalculator _balanceCalculator;

        public ICurrencyConverter CurrencyConverter { get; set; }
        public event Action ValueChanged;


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
            set
            {
                SetField(ref _lendedAmount, value);
            }
        }

        public double PendingAmount
        {
            get
            {
                _balanceCalculator.ReCalculate(this);
                return _pendingAmount;

            }
            set
            {
                SetField(ref _pendingAmount, value);
            }
        }

        protected void OnValueChanged()
        {
            ValueChanged?.Invoke();
        }


        public UserBobj(User user, ICollection<ExpenseBobj> expenses, ICollection<GroupBobj> groups, IUserBobjBalanceCalculator balanceCalculator, ICurrencyConverter currencyConverter)
            : base(user.EmailId, user.UserName, user.WalletBalance, user.CurrencyIndex)
        {
            _balanceCalculator = balanceCalculator;
            CurrencyConverter = currencyConverter;
            Expenses.AddRange(expenses);
            Groups.AddRange(groups);

            _groups.CollectionChanged += CollectionChanged;
            _expenses.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //calling value changed  event when collection is modified
            OnValueChanged();
        }

        protected UserBobj(UserBobj userBobj) : this(userBobj, userBobj.Expenses, userBobj.Groups, userBobj._balanceCalculator, userBobj.CurrencyConverter)
        {

        }


        private void SetField<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;
            OnValueChanged();
            return;
        }

    }
}
