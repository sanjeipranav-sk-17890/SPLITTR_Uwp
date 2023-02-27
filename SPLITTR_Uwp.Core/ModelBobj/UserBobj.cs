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
        
        /// <summary>
        /// Gets or Sets the walletBalance in Respective Currency Preference
        /// </summary>
        public virtual double StrWalletBalance
        {
            get => CurrencyConverter.ConvertCurrency(base.WalletBalance);
            set => base.WalletBalance = CurrencyConverter.ConvertToEntityCurrency(value);

        }

        public virtual Currency CurrencyPreference
        {
            get => (Currency)CurrencyIndex;
            set => CurrencyIndex = (int)value;
        }

        public virtual double StrLentAmount
        {
            get => CurrencyConverter.ConvertCurrency(base.LentAmount);
            set => LentAmount = CurrencyConverter.ConvertToEntityCurrency(value);
        }

        public virtual double StrOwingAmount
        {
            get => CurrencyConverter.ConvertCurrency(base.OwingAmount);
            set => OwingAmount = CurrencyConverter.ConvertToEntityCurrency(value);
        }



        public UserBobj(User user, ICurrencyConverter currencyConverter)
            : base(user.EmailId, user.UserName, user.WalletBalance, user.CurrencyIndex,user.OwingAmount,user.LentAmount)
        {
            CurrencyConverter = currencyConverter;
        }

        protected UserBobj(UserBobj userBobj) : this(userBobj, userBobj.CurrencyConverter)
        {

        }

    }
   
    
}
