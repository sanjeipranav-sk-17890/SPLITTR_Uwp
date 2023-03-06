using System.Collections.Generic;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface IExpenseCategoryJsonToPoCoConverter
{
    IEnumerable<ExpenseCategoryBobj> DeserializeJsonToExpenseCategoryBobjs(string jsonString);
}
