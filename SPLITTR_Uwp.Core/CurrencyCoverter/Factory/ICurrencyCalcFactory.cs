using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.CurrencyCoverter.Factory
{
    public interface ICurrencyCalcFactory
    {
        /// <summary>
        /// Returns CurrencyConverter Instance Based On Currency Type
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        public ICurrencyConverter GetCurrencyCalculator(Currency currencyPreference);

    }
}
