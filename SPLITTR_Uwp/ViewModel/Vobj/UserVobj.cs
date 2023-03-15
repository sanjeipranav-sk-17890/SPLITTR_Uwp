using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Services;

namespace SPLITTR_Uwp.ViewModel.Vobj;

public class UserVobj : UserBobj, INotifyPropertyChanged
{
        
    public override string UserName
    {
        get => base.UserName;
        set
            
        {if (value == base.UserName)
                return;
            base.UserName = value;
            OnPropertyChanged();
        }
    }

    public override double StrWalletBalance
    {
        get => base.StrWalletBalance;
        set
        {
            if (value == base.StrWalletBalance)
                return;
            base.StrWalletBalance = value;
            OnPropertyChanged();
        }
    }

    public override double StrLentAmount
    {
        get => base.StrLentAmount;
        set
        {
                
            if (value == base.StrLentAmount)
                return;
            base.StrLentAmount = value;
            OnPropertyChanged();
        }
    }
    public override double StrOwingAmount
    {
        get => base.StrOwingAmount;
        set
        {

            if (value == base.StrOwingAmount)
                return;
            base.StrOwingAmount = value;
            OnPropertyChanged();
        }
    }

    public override Currency CurrencyPreference
    {
        get => base.CurrencyPreference;
        set
        {
            if (value == base.CurrencyPreference)
                return;
            base.CurrencyPreference = value;
            OnPropertyChanged();
        }
    }

    public UserVobj(UserBobj user) : base(user)
    {
        SplittrNotification.UserObjUpdated += SplittrNotification_UserObjUpdated;
    }

    private void SplittrNotification_UserObjUpdated(UserBobjUpdatedEventArgs obj)
    {
        if (obj?.UpdatedUser?.EmailId.Equals(EmailId) is false)
        {
            return;   
        }
        if (obj?.UpdatedUser is null)
        {
            return;
        }
        UserName = obj.UpdatedUser.UserName;
        StrWalletBalance = obj.UpdatedUser.StrWalletBalance;
        StrLentAmount = obj.UpdatedUser.StrLentAmount;
        StrOwingAmount = obj.UpdatedUser.StrOwingAmount;
        CurrencyPreference = obj.UpdatedUser.CurrencyPreference;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        _ = UiService.RunOnUiThread(
            () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }).ConfigureAwait(false);
    }
}