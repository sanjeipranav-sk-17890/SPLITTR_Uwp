using System;
using System.Collections.Generic;
using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory;
using SPLITTR_Uwp.Core.UseCase.GetCategoryById;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Vobj;

namespace SPLITTR_Uwp.DataRepository;

internal static class Store
{
    public static UserBobj CurrentUserBobj { get; set; }


    public static IReadOnlyList<ExpenseCategoryBobj> Categories
    {
        get => CategoriesRepo;
    }

    private readonly static List<ExpenseCategoryBobj> CategoriesRepo = new List<ExpenseCategoryBobj>();

    public static event Action<IEnumerable<ExpenseCategoryBobj>> CategoriesLoaded; 


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
            CategoriesRepo.AddRange(result.Categories);
            CategoriesLoaded?.Invoke(Categories);

            NetworkInfoService.NetWorkConnectionChanged -= NetworkInfoService_NetWorkConnectionChanged;
        }
        public void OnError(SplittrException ex)
        {
            ExceptionHandlerService.HandleException(ex);
            if (ex.IsNetworkCallError)
            {
                SubscribeToNetWorkChangeIfNot();
                NetworkInfoService.NetWorkConnectionChanged += NetworkInfoService_NetWorkConnectionChanged;
            }
        }
        private static bool _isNetWorkSubscribed;

        private void SubscribeToNetWorkChangeIfNot()
        {
            if (!_isNetWorkSubscribed)
            {
                NetworkInfoService.NetWorkConnectionChanged += NetworkInfoService_NetWorkConnectionChanged;
                _isNetWorkSubscribed =true;
            }
        }

        private void NetworkInfoService_NetWorkConnectionChanged(NetWorkConnectionChangedEventArgs obj)
        {
            if (obj?.IsInterNetAvailable is true)
            {
                PopulateCategoryObj();
            }
        }
    }
}