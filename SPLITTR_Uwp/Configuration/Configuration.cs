using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Windows.Media.Streaming.Adaptive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.Services;
using SPLITTR_Uwp.Core.Services.Contracts;
using SPLITTR_Uwp.Core.Services.SqliteConnection;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.Views;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataTemplates;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.VmLogic;

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
                .AddSingleton<IUserDataServices, UserDataService>()
                .AddSingleton<IExpenseDataServices, ExpenseDataServices>()
                .AddSingleton<IGroupDataServices, GroupDataServices>()
                .AddSingleton<IGroupToUserDataServices, GroupToUserDataServices>()
                .AddSingleton<IUserDataHandler, UserDataHandler>()
                .AddSingleton<IGroupDataHandler, GroupDataHandler>()
                .AddSingleton<IExpenseDataHandler, ExpenseDataManager>()
                .AddSingleton<DataStore>()
                .AddSingleton<IExpenseHistoryUsecase,ExpenseHistoryManager>()
                .AddTransient<IUserBobjBalanceCalculator, UserBobjPropertyCalculator>()
                .AddSingleton<ICurrencyCalcFactory, CalculatorFactory>()
                .AddSingleton<IExpenseHistoryManager,ExpenseHistoryManager>()
                .AddTransient<RupessConverter>()
                .AddTransient<DollarConverter>()
                .AddTransient<YenConverter>()
                .AddTransient<EuroConverter>()
                .AddTransient<IStringManipulator, Manipulator>()
                .AddTransient<IUserUtility, UserUtility>()
                .AddTransient<IGroupUtility,GroupUtility>()
                .AddTransient<IExpenseGrouper,ExpenseGrouper>()
                .AddTransient<IExpenseUtility,ExpenseUtility>();



        }

        /// <summary>
        /// Adding ViewModel's of the Spliiter into the container 
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
