using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.UseCase.AddWalletAmount;
using SPLITTR_Uwp.Core.UseCase.CancelExpense;
using SPLITTR_Uwp.Core.UseCase.CreateGroup;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;
using SPLITTR_Uwp.Core.UseCase.LoginUser;
using SPLITTR_Uwp.Core.UseCase.MarkAsPaid;
using SPLITTR_Uwp.Core.UseCase.SettleUpExpense;
using SPLITTR_Uwp.Core.UseCase.SignUpUser;
using SPLITTR_Uwp.Core.UseCase.SplitExpenses;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;
using SPLITTR_Uwp.Core.UseCase.UserSuggestion;
using SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataTemplates;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.VmLogic;

namespace SPLITTR_Uwp.Configuration
{

    public static class Configuration
    {
        /// <summary>
        /// Adding SplittrUwp Dependencies to Service Collection 
        /// </summary>
        public static IServiceCollection AddDependencies(this IServiceCollection container)
        {
            //Storing Apps local storage for db file location
            var connectionString= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SPLITTR.db3");
            ConfigurationManager.AppSettings.Set("ConnectionString",connectionString);

            AddViewModels(container);
            AddServiceDependencies(container);

            return container;
        }


        private static void AddServiceDependencies(IServiceCollection container)
        {
            
            container.AddSingleton<IStateService, StateService>()
                .AddTransient<IStringManipulator, Manipulator>()
                .AddTransient<IExpenseGrouper, ExpenseGrouper>()
                .AddTransient<UpdateUser>()
                .AddTransient<AddWalletAmount>()
                .AddTransient<GroupCreation>()
                .AddTransient<SplitExpenses>()
                .AddTransient<RelatedExpense>()
                .AddTransient<MarkAsPaid>()
                .AddTransient<SettleUpSplit>()
                .AddTransient<CancelExpense>()
                .AddTransient<UserLogin>()
                .AddTransient<SignUpUser>()
                .AddTransient<VerifyPaidExpense>()
                .AddTransient<GroupDetailById>()
                .AddTransient<GetUserGroups>()
                .AddTransient<UserSuggestion>();
        }

        /// <summary>
        /// Adding ViewModel's of the Splitter into the container 
        /// </summary>
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
