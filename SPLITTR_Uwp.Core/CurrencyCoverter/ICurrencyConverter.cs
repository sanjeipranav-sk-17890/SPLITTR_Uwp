namespace SPLITTR_Uwp.Core.CurrencyCoverter;

public interface ICurrencyConverter
{
    double ConvertCurrency(double expense);
    double ConvertToEntityCurrency(double expense);
}