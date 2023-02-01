using System;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.Services;
using SPLITTR_Uwp.Core.Services.Contracts;
using SPLITTR_Uwp.Core.Services.SqliteConnection;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataTemplates;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.VmLogic;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;

namespace SPLITTR_Uwp.Configuration
{

    public static class Configuration
    {
        /// <summary>
        /// Adding SpliitrUwp Dependencies to Service Collection 
        /// </summary>
        public static IServiceCollection AddDependencies(this IServiceCollection container)
        {
            //Storing App's local storage for db file location
            var connectionString= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SPLITTR.db3");
            ConfigurationManager.AppSettings.Set("ConnectionString",connectionString);

            AddViewModels(container);
            AddServiceDependencies(container);

            return container;
        }

        private static void AddServiceDependencies(IServiceCollection container)
        {
            container.AddSingleton<ISqlDataServices, SqlDataBaseAdapter>()
                .AddSingleton<IUserDBHandler, UserDbHandler>()
                .AddSingleton<IExpenseDBHandler, ExpenseDbHandler>()
                .AddSingleton<IGroupDBHandler, GroupDbHandler>()
                .AddSingleton<IGroupToUserDBHandler, GroupToUserDbHandler>()
                .AddSingleton<IUserDataManager, UserDataManager>()
                .AddSingleton<IGroupDataManager, GroupDataManager>()
                .AddSingleton<IExpenseDataHandler, ExpenseDataManager>()
                .AddSingleton<IExpenseHistoryUsecase,ExpenseHistoryManager>()
                .AddTransient<IUserBobjBalanceCalculator, UserBobjPropertyCalculator>()
                .AddSingleton<ICurrencyCalcFactory, CalculatorFactory>()
                .AddSingleton<IExpenseHistoryManager,ExpenseHistoryManager>()
                .AddTransient<IStateService,StateService>()
                .AddTransient<RupessConverter>()
                .AddTransient<DollarConverter>()
                .AddTransient<YenConverter>()
                .AddTransient<EuroConverter>()
                .AddTransient<IStringManipulator, Manipulator>()
                .AddTransient<IUserUseCase, UserUseCase>()
                .AddTransient<IExpensePayment, ExpensePayment>()
                .AddTransient<IGroupUseCase, GroupCreationUseCase>()
                .AddTransient<ISplitExpenseView,SplitExpenseUserControl>()
                .AddTransient<IExpenseGrouper,ExpenseGrouper>()
                .AddTransient<IExpenseUseCase,ExpenseUseCase>();



        }

        /// <summary>
        /// Adding ViewModel's of the Splitter into the container 
        /// </summary>
        ///
        private static void AddViewModels(IServiceCollection container)
        {
            container.AddTransient<LoginPageViewModel>();
            container.AddTransient<SignPageViewModel>();
            container.AddTransient<UserProfilePageViewModel>();
            container.AddTransient<SplitExpenseViewModel>();
            container.AddTransient<IMainPageViewModel,MainPageViewModel>();
            container.AddTransient<MainPageViewModel>();
            container.AddTransient<ExpenseListAndDetailedPageViewModel>();
            container.AddTransient<OwnerExpenseControlViewModel>();
            container.AddTransient<WalletBalanceUpdateViewModel>();
            container.AddTransient<GroupCreationPageViewModel>();
            container.AddTransient<RelatedExpenseTemplateViewModel>();
            container.AddTransient<PaymentWindowExpenseViewModel>();
            container.AddTransient<ExpenseDetailedViewUserControlViewModel>();
            container.AddTransient<OwingMoneyPaymentExpenseViewModel>();

        }

    }


}
