using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.DbHandler.SqliteConnection;
using SPLITTR_Uwp.Core.UseCase.CreateGroup;
using SPLITTR_Uwp.Core.UseCase.LoginUser;
using SPLITTR_Uwp.Core.UseCase.MarkAsPaid;

namespace SPLITTR_Uwp.Core.DependencyInjector
{
    public class SplittrDependencyService
    {
        private  static IServiceProvider _serviceProvider;

        private static IServiceProvider Container
        {
            get
            {
                _serviceProvider ??= RegisterServices(new ServiceCollection());
                return _serviceProvider;
            } 
        }

        public static T GetInstance<T>()
        {
            return Container.GetService<T>();
        }


        private static IServiceProvider RegisterServices(IServiceCollection container)
        {
            container.AddSingleton<ISqlDataServices, SqlDataBaseAdapter>()
                .AddSingleton<IUserDbHandler, UserDbHandler>()
                .AddSingleton<IExpenseDbHandler, ExpenseDbHandler>()
                .AddSingleton<IGroupDbHandler, GroupDbHandler>()
                .AddSingleton<IGroupToUserDbHandler, GroupToUserDbHandler>()
                .AddSingleton<IUserDataManager, UserDataManagerBase>()
                .AddSingleton<IGroupDataManager, GroupDataManagerBase>()
                .AddSingleton<IExpenseDataManager, ExpenseDataManager>()
                .AddSingleton<IGroupCreationDataManager, GroupCreationDataManager>()
                .AddSingleton<ICurrencyCalcFactory, CalculatorFactory>()
                .AddSingleton<IExpenseHistoryManager, ExpenseHistoryManager>()
                .AddSingleton<IUserProfileUpdateDataManager, UserUpdateDataManager>()
                .AddSingleton<IAddWalletBalanceDataManager, UserUpdateDataManager>()
                .AddSingleton<IUserSuggestionDataManager, UserUpdateDataManager>()
                .AddSingleton<IRelatedExpenseDataManager, RelatedExpenseDataManager>()
                .AddSingleton<ISplitExpenseDataManager, ExpenseStatusDataManager>()
                .AddSingleton<IMarkExpensePaidDataManager, ExpenseStatusDataManager>()
                .AddSingleton<IExpenseCancellationDataManager, ExpenseStatusDataManager>()
                .AddSingleton<ISettleUpSplitDataManager, SettleUpExpenseDataManager>()
                .AddSingleton<ISignUpDataManager, AuthenticationManager>()
                .AddSingleton<IExpenseFetchDataManager,ExpenseFetchDataManager>()
                .AddSingleton<IAuthenticationManager, AuthenticationManager>()
                .AddTransient<RupessConverter>()
                .AddTransient<DollarConverter>()
                .AddTransient<YenConverter>()
                .AddTransient<EuroConverter>();
           return container.BuildServiceProvider();

        }
        
    }
}
