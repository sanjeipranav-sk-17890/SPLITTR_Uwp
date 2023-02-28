namespace SPLITTR_Uwp.Core.CurrencyCoverter;

public class DollarConverter : ICurrencyConverter
{
    public double ConvertCurrency(double expense)
    {
        //Inr To Dlr convertion Rate
        return expense / 82.13;
    }
    public double ConvertToEntityCurrency(double expense)
    {
        return expense * 82.13;
    }
}