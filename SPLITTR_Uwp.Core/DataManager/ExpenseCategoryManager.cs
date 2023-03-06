using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.NetHandler;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory;
using SPLITTR_Uwp.Core.UseCase.GetCategoryById;

namespace SPLITTR_Uwp.Core.DataManager
{
    public interface IExpenseCategoryManager
    {
        void FetchExpenseCategory(IUseCaseCallBack<FetchExpenseCategoryResponse> callBack);
        void FetchExpenseCategoryById(int id , IUseCaseCallBack<GetCategoryByIdResponse> callBack);
    }

    public class ExpenseCategoryManager : IExpenseCategoryManager
    {
        private readonly IExpenseCategoryNetHandler _expenseCategoryNetHandler;
        private readonly IExpenseCategoryJsonToPoCoConverter _expenseCategoriesDeserializer;
        private readonly ConcurrentDictionary<int,ExpenseCategoryBobj> _expenseCategoryCache = new ConcurrentDictionary<int,ExpenseCategoryBobj>();

        public ExpenseCategoryManager(IExpenseCategoryNetHandler expenseCategoryNetHandler,IExpenseCategoryJsonToPoCoConverter expenseCategoriesDeserializer)
        {
            _expenseCategoryNetHandler = expenseCategoryNetHandler;
            _expenseCategoriesDeserializer = expenseCategoriesDeserializer;
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
                callBack?.OnError(new SplittrException(e));
            }

        }

        public async void FetchExpenseCategoryById(int id, IUseCaseCallBack<GetCategoryByIdResponse> callBack)
        {
            try
            {
                var requestedCategory =await FetchExpenseCategoryById(id).ConfigureAwait(false);

                callBack?.OnSuccess(new GetCategoryByIdResponse(requestedCategory));
            }
            catch (Exception e)
            {
                callBack?.OnError(new SplittrException(e));
            }
        }

        private async Task<IEnumerable<ExpenseCategoryBobj>> FetchExpensesCategoryByNetHandler()
        {

            var categoryJsonResponse = await _expenseCategoryNetHandler.FetchExpenseCategory().ConfigureAwait(false);

            var expenseCategories = _expenseCategoriesDeserializer.DeserializeJsonToExpenseCategoryBobjs(categoryJsonResponse);

            var fetchExpensesCategoryByNetHandler = expenseCategories.ToList();

            foreach (var expenseCategory in fetchExpensesCategoryByNetHandler)
            {
                _expenseCategoryCache.AddOrUpdate(expenseCategory.Id, expenseCategory, ((i, bobj) => bobj));
            }

            return fetchExpensesCategoryByNetHandler;
        }

        private async Task<ExpenseCategory> FetchExpenseCategoryById(int id)
        {
            var subCategory = FetchExpenseCategoryOrDefault(_expenseCategoryCache.Values,id);

            if (subCategory is not null)
            {
                return subCategory;
            }

            var categories =await FetchExpensesCategoryByNetHandler().ConfigureAwait(false);

            return FetchExpenseCategoryOrDefault(categories,id);

            //queries SubCategories from Parent if Available
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


}
