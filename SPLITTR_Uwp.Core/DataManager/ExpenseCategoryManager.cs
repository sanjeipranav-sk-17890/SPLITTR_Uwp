using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory;

namespace SPLITTR_Uwp.Core.DataManager
{
    internal interface IExpenseCategoryManager
    {
        void FetchExpenseCategory(IUseCaseCallBack<FetchExpenseCategoryResponse> callBack);
    }

    internal class ExpenseCategoryManager : IExpenseCategoryManager
    {
        private readonly IExpenseCategoryNetHandler _expenseCategoryNetHandler;
        private readonly ConcurrentDictionary<int,ExpenseCategoryBobj> _expenseCategoryCache = new ConcurrentDictionary<int,ExpenseCategoryBobj>();

        public ExpenseCategoryManager(IExpenseCategoryNetHandler expenseCategoryNetHandler)
        {
            _expenseCategoryNetHandler = expenseCategoryNetHandler;
        }


        public async void FetchExpenseCategory(IUseCaseCallBack<FetchExpenseCategoryResponse> callBack)
        {
            try
            {
                //ifCache Available Returning without further network call
                if (!_expenseCategoryCache.IsEmpty)
                {
                    callBack?.OnSuccess(new FetchExpenseCategoryResponse(_expenseCategoryCache.Values));
                    return;
                }

                var expenseCategories = await FetchExpensesCategoryByNetHandler().ConfigureAwait(false);

                callBack?.OnSuccess(new FetchExpenseCategoryResponse(expenseCategories));
            }
            catch (Exception e)
            {
                callBack?.OnError(new SplittrException.SplittrException(e));
            }

        }

        private async Task<IEnumerable<ExpenseCategoryBobj>> FetchExpensesCategoryByNetHandler()
        {

            var categoryJsonResponse = await _expenseCategoryNetHandler.FetchExpenseCategory().ConfigureAwait(false);

            var expenseCategories = ExpenseCategoriesDeserializer.DeserializeJsonToExpenseCategoryBobjs(categoryJsonResponse);

            var fetchExpensesCategoryByNetHandler = expenseCategories.ToList();

            foreach (var expenseCategory in fetchExpensesCategoryByNetHandler)
            {
                _expenseCategoryCache.AddOrUpdate(expenseCategory.Id, expenseCategory, ((i, bobj) => bobj));
            }

            return fetchExpensesCategoryByNetHandler;
        }

        public async Task<ExpenseCategory> FetchExpenseCategoryById(int id)
        {
            var subCategory = FetchExpenseCategoryOrDefault(_expenseCategoryCache.Values,id);

            if (subCategory is not null)
            {
                return subCategory;
            }

            var categories =await FetchExpensesCategoryByNetHandler().ConfigureAwait(false);

            return FetchExpenseCategoryOrDefault(categories,id);

            //queries Sub from Parent if Available
            ExpenseCategory FetchExpenseCategoryOrDefault(IEnumerable<ExpenseCategoryBobj> _expenseCategoryBobjs,int categoryId)
            {

                var parentCategoryBobj = _expenseCategoryBobjs.FirstOrDefault(mainCategory => mainCategory.SubExpenseCategories
                    .Any(sub => sub.Id == categoryId));
                var subCategory = parentCategoryBobj?.SubExpenseCategories
                    .FirstOrDefault(sub => sub.Id == categoryId);
                return subCategory;
            }
        }

    }


    public static class ExpenseCategoriesDeserializer
    {
        public static IEnumerable<ExpenseCategoryBobj> DeserializeJsonToExpenseCategoryBobjs(string jsonString)
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
                    Icon = o["icon_types"]["transparent"]["large"].Value<string>()
                };
                return subCategory;
            }
            catch (NullReferenceException)
            {
                return default;
            }
        }
    }
  
}
