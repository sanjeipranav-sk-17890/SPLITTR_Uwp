using System;
using System.Collections.Generic;
using System.Text;

namespace SPLITTR_Uwp.Core.CurrencyCoverter
{
    public class YenConverter : ICurrencyConverter
    {
        public double ConvertCurrency(double expense)
        {
            return expense * 1.76;
        }
        public double ConvertToEntityCurrency(double expense)
        {
            return expense / 1.76;
        }

    }
}
