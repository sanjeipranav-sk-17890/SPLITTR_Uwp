using System;
using System.Collections.Generic;
using System.Text;

namespace SPLITTR_Uwp.Core.CurrencyCoverter
{
    public class EuroConverter : ICurrencyConverter
    {
        public double ConvertCurrency(double expense)
        {
            return expense / 81.2;
        }
        public double ConvertToEntityCurrency(double expense)
        {
            return expense * 81.2;
        }
    }
}
