using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory;
using SPLITTR_Uwp.Services;
using static SPLITTR_Uwp.Services.UiService;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class ExpenseCategoryControl : UserControl
    {
        private CategoryControlViewModel _viewModel;
        public ExpenseCategoryControl()
        {
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<CategoryControlViewModel>(App.Container);
            this.InitializeComponent();
        }
        private void CatogoriesFlyOut_Opening(object sender, object e)
        {
            if (_viewModel.Categories.Count > 0)
            {
                return;
            }
            _viewModel.LoadData();
        }

        private void CategoryImageClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
            }

        }
        public class CategoryControlViewModel : ObservableObject
        {
            public ObservableCollection<ExpenseCategoryBobj> Categories { get; } = new ObservableCollection<ExpenseCategoryBobj>();

            /// <summary>
            /// Call to Expense category UseCase To Fetch Data
            /// </summary>
            public void LoadData()
            {
                var fetchCategoryReq = new FetchExpenseCategoryRequest(CancellationToken.None, new CategoryControlVmPresenterCb(this));

                var fetchCategoryUseCaseObj = InstanceBuilder.CreateInstance<FetchExpenseCategory>(fetchCategoryReq);

                fetchCategoryUseCaseObj?.Execute();
            }


            class CategoryControlVmPresenterCb : IPresenterCallBack<FetchExpenseCategoryResponse>
            {
                private readonly CategoryControlViewModel _viewModel;

                public CategoryControlVmPresenterCb(CategoryControlViewModel viewModel)
                {
                    this._viewModel = viewModel;
                }

                public async void OnSuccess(FetchExpenseCategoryResponse result)
                {
                    await RunOnUiThread(() =>
                    {
                        _viewModel.Categories.ClearAndAdd(result.Categories);
                    }).ConfigureAwait(false);
                }
                public void OnError(SplittrException ex)
                {
                    ExceptionHandlerService.HandleException(ex);
                }
            }

        }

    }
}
