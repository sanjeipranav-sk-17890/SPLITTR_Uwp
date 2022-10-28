using System;
using System.Collections.Generic;
using System.Text;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.CurrencyCoverter.Factory
{
    public class CalculatorFactory : ICurrencyCalcFactory
    {
        readonly RupessConverter _converter;
        readonly DollarConverter _converter2;
        readonly EuroConverter _converter3;
        readonly YenConverter _converter4;
        public CalculatorFactory(RupessConverter converter, YenConverter converter4, EuroConverter converter3, DollarConverter converter2)
        {
            this._converter = converter;
            _converter4 = converter4;
            _converter3 = converter3;
            _converter2 = converter2;
        }

        public ICurrencyConverter GetCurrencyCalculator(Currency currencyPreference)
        {

            ICurrencyConverter converter = currencyPreference switch
            {
                Currency.Rupee => _converter,
                Currency.Dollar => _converter2,
                Currency.Euro => _converter3,
                Currency.Yen => _converter4,
                _ => throw new NotSupportedException("Currency Preference index is not supported,something wrong with Db data Handling")
            };
            return converter;
        }
    }
}
