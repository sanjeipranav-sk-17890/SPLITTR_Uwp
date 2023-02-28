using SQLite;

namespace SPLITTR_Uwp.Core.Models
{
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

        public  int CurrencyIndex { get; set; }

        public  double OwingAmount { get; set; }

        public  double LentAmount { get; set; }

        public User()
        {

        }
        public User(string emailId, string userName, double walletBalance,int currencyIndex,double owingAmount,double lentAmount)
        {
            _walletBalance = walletBalance;
            EmailId = emailId.ToLower();
            _userName = userName;
            OwingAmount = owingAmount;
            LentAmount = lentAmount;
            CurrencyIndex = currencyIndex;
        }

        public override bool Equals(object obj)
        {
            var user = obj as User;
            if (user == null || string.IsNullOrEmpty(EmailId)) return false;
            return EmailId.Equals(user.EmailId);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return UserName;
        }

    }
}
