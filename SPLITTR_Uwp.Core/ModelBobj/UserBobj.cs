using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
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
      

        public ICurrencyConverter CurrencyConverter { get; set; }
        public event Action<string> ValueChanged;


        public ObservableCollection<ExpenseBobj> Expenses { get; } = new ObservableCollection<ExpenseBobj>();

        public ObservableCollection<GroupBobj> Groups { get; } = new ObservableCollection<GroupBobj>();


        public override string UserName
        {
            get => base.UserName;
            set
            {
                base.UserName = value;
                OnValueChanged(nameof(UserName));
            }
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

        public virtual double StrLentAmount
        {
            get => CurrencyConverter.ConvertCurrency(base.LentAmount);
            set
            {
                LentAmount = CurrencyConverter.ConvertToEntityCurrency(value);
                OnValueChanged(nameof(StrLentAmount));
            }
        }

        public virtual double StrOwingAmount
        {
            get => CurrencyConverter.ConvertCurrency(base.OwingAmount);
            set
            {
                OwingAmount = CurrencyConverter.ConvertToEntityCurrency(value);
                OnValueChanged(nameof(StrOwingAmount));
            }
        }

        protected void OnValueChanged(string property)
        {
            ValueChanged?.Invoke(property);
        }


        public UserBobj(User user, ICollection<ExpenseBobj> expenses, ICollection<GroupBobj> groups, ICurrencyConverter currencyConverter)
            : base(user.EmailId, user.UserName, user.WalletBalance, user.CurrencyIndex,user.OwingAmount,user.LentAmount)
        {
            CurrencyConverter = currencyConverter;
            Expenses.AddRange(expenses);
            Groups.AddRange(groups);

            Groups.CollectionChanged += GroupCollectionChanged;
            Expenses.CollectionChanged += ExpenseCollectionChanged;
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

        protected UserBobj(UserBobj userBobj) : this(userBobj, userBobj.Expenses, userBobj.Groups, userBobj.CurrencyConverter)
        {

        }

    }
   
    
}
