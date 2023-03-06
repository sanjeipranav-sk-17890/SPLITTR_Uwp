using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataManager.Services;

public  class ExpenseCategoriesDeserializer : IExpenseCategoryJsonToPoCoConverter
{
    public  IEnumerable<ExpenseCategoryBobj> DeserializeJsonToExpenseCategoryBobjs(string jsonString)
    {
        var jsonObj = JObject.Parse(jsonString);

        if(jsonObj.TryGetValue("categories",out JToken root) && root is JArray JmainCategoriesArray)
        { 
            return JmainCategoriesArray.AsParallel().Select(jToken =>
            {
                var mainCategory = ConvertJObjectToExpenseCategory(jToken as JObject);

                mainCategory.ParentCategoryId = -1;

                var subCategory = InitializeSubCategories(jToken["subcategories"] as JArray,mainCategory.Id);

                return new ExpenseCategoryBobj(mainCategory,subCategory);
            });
        }

        return new List<ExpenseCategoryBobj>().DefaultIfEmpty();

        //Traversing Each Subcategory Array And Instantiating a Enumerable SubCategory
        IEnumerable<ExpenseCategory> InitializeSubCategories(JArray subCategoryJArray,int mappingParentId)
        {
            return  subCategoryJArray?.AsParallel().Cast<JObject>().Select(jobj =>
            {
                var subCategory = ConvertJObjectToExpenseCategory(jobj);
                subCategory.ParentCategoryId = mappingParentId;
                return subCategory;
            });
        }
           
    }
    private static ExpenseCategory ConvertJObjectToExpenseCategory(JObject o)
    {
        try
        {
            var subCategory = new ExpenseCategory()
            {
                Id = o.Value<int>("id"),
                Name = o.Value<string>("name"),
                Icon = o["icon_types"]["square"]["large"].Value<string>()
            };
            return subCategory;
        }
        catch (NullReferenceException)
        {
            return default;
        }
    }
}
