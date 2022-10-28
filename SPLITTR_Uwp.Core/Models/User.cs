using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SPLITTR_Uwp.Core.Models
{
    public delegate void ValueChanged();
    public class User 
    {
        private string _userName;
        private double _walletBalance;


        [PrimaryKey, Unique]
        public string EmailId { get; set; }

        public virtual string UserName
        {
            get => _userName;
            set => _userName = value;
        }

        public virtual double WalletBalance
        {
            get => _walletBalance;
            set => _walletBalance = value;
        }

        public int CurrencyIndex { get; set; }





        public User()
        {

        }
        public User(string emailId, string userName, double walletBalance,int currencyIndex)
        {
            _walletBalance = walletBalance;
            EmailId = emailId.ToLower();
            _userName = userName;
            CurrencyIndex = currencyIndex;
        }

       
    }
}
