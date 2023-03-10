using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Security.Cryptography.Core;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory;
using SPLITTR_Uwp.Core.UseCase.GetCategoryById;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Vobj;

namespace SPLITTR_Uwp.DataRepository;

internal static class Store
{

    private readonly static ConcurrentDictionary<int, ExpenseCategory> CategoryDictionary = new ConcurrentDictionary<int, ExpenseCategory>();

    private readonly static ConcurrentDictionary<int, ExpenseCategoryBobj> ParentCategoryCollection = new ConcurrentDictionary<int, ExpenseCategoryBobj>();


    public static UserBobj CurrentUserBobj { get; set; }

    public static IReadOnlyList<ExpenseCategoryBobj> Categories
    {
        get => ParentCategoryCollection.Values.ToList();
    }

    public static IReadOnlyDictionary<int, ExpenseCategory> CategoryOnlyDictionary
    {
        get => CategoryDictionary;
    }

    public static event Action<CategoryLoadedEventArgs> CategoriesLoaded; 


    private static void PopulateCategoryObj()
    {
        var fetchCategoryReq = new FetchExpenseCategoryRequest(CancellationToken.None, new StorePresenterCb());

        var fetchCategoryUseCaseObj = InstanceBuilder.CreateInstance<FetchExpenseCategory>(fetchCategoryReq);

        fetchCategoryUseCaseObj?.Execute();
    }

    public static void LoadInitialNecessaryData()
    {
        PopulateCategoryObj();
    }

    private class StorePresenterCb : IPresenterCallBack<FetchExpenseCategoryResponse>
    {

        public void OnSuccess(FetchExpenseCategoryResponse result)
        {
            foreach (var parentCategory in result.Categories)
            {
                ParentCategoryCollection.AddOrUpdate(parentCategory.Id, parentCategory, (i, bobj) => bobj);
            }
            PopulateCacheDictionary(result.Categories);

            CategoriesLoaded?.Invoke(new CategoryLoadedEventArgs(ParentCategoryCollection.Values.ToList(), CategoryDictionary));

            NetworkInfoService.NetWorkConnectionChanged -= NetworkInfoService_NetWorkConnectionChanged;
        }

        private void PopulateCacheDictionary(IEnumerable<ExpenseCategoryBobj> mainCategoryBobjs)
        {

            AddCollectionToDictionary(mainCategoryBobjs);
            foreach (var mainCategory in mainCategoryBobjs)
            {
               AddCollectionToDictionary(mainCategory.SubExpenseCategories);
            }
            void AddCollectionToDictionary(IEnumerable<ExpenseCategory> categories)
            {
                foreach (var category in categories)
                {
                    CategoryDictionary.AddOrUpdate(category.Id, category, UpdateValueFactory);
                }
            }
            ExpenseCategory UpdateValueFactory(int i, ExpenseCategory category) => category;
        }

        public void OnError(SplittrException ex)
        {
            ExceptionHandlerService.HandleException(ex);
            if (!ex.IsNetworkCallError)
            {
                return;
            }
            SubscribeToNetWorkChangeIfNot();
        }

        #region NetworkCallSubscriptionRegion

        private static bool _isNetWorkSubscribed;

        private void SubscribeToNetWorkChangeIfNot()
        {
            if (_isNetWorkSubscribed)
            {
                return;
            }
            NetworkInfoService.NetWorkConnectionChanged += NetworkInfoService_NetWorkConnectionChanged;
            _isNetWorkSubscribed =true;
        }

        private void NetworkInfoService_NetWorkConnectionChanged(NetWorkConnectionChangedEventArgs obj)
        {
            if (obj?.IsInterNetAvailable is true)
            {
                PopulateCategoryObj();
            }
        }

        #endregion
    }

}
public class CategoryLoadedEventArgs : EventArgs
{
    public IReadOnlyList<ExpenseCategoryBobj> ExpenseCategories { get;}

    public IReadOnlyDictionary<int, ExpenseCategory> ExpenseCategoriesDict { get;}

    public CategoryLoadedEventArgs(IReadOnlyList<ExpenseCategoryBobj> expenseCategories, IReadOnlyDictionary<int, ExpenseCategory> expenseCategoriesDict)
    {
        ExpenseCategories = expenseCategories;
        ExpenseCategoriesDict = expenseCategoriesDict;
    }

}