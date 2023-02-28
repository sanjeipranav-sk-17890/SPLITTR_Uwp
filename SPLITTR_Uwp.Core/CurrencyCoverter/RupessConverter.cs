namespace SPLITTR_Uwp.Core.CurrencyCoverter;

public class RupessConverter : ICurrencyConverter
{

    public double ConvertCurrency(double expense)
    {
        return expense;
    }
    public double ConvertToEntityCurrency(double expense)
    {
        return expense;
    }
}