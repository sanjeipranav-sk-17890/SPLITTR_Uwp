using System;
using System.Collections.Generic;
using System.Text;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics
{
    public interface IUserBobjBalanceCalculator
    {
        void ReCalculate(UserBobj userBobj);
    }
}
