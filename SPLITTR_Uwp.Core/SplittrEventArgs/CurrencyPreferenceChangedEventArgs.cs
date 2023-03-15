using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class CurrencyPreferenceChangedEventArgs : SplittrEventArgs
{
    public Currency PreferredCurrency { get; }

    public CurrencyPreferenceChangedEventArgs(Currency preferedCurrency)
    {
        PreferredCurrency = preferedCurrency;
    }
}
