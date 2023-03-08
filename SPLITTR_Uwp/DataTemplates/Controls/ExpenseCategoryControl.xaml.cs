using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.FetchExpenseCategory;
using SPLITTR_Uwp.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.DataRepository;
using static SPLITTR_Uwp.Services.UiService;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class ExpenseCategoryControl : UserControl, IView
    {
        private readonly CategoryControlViewModel _viewModel;

        public event Action<ExpenseCategory> OnExpenseCategorySelected;

        public readonly static DependencyProperty IsFlyOutOpeningAllowedProperty = DependencyProperty.Register(
            nameof(IsFlyOutOpeningAllowed), typeof(bool), typeof(ExpenseCategoryControl), new PropertyMetadata(default(bool)));

        public bool IsFlyOutOpeningAllowed
        {
            get => (bool)GetValue(IsFlyOutOpeningAllowedProperty);
            set => SetValue(IsFlyOutOpeningAllowedProperty, value);
        }

        public readonly static DependencyProperty CategoryIconSourceProperty = DependencyProperty.Register(
            nameof(CategoryIconSource), typeof(ImageSource), typeof(ExpenseCategoryControl), new PropertyMetadata(default(ImageSource), new PropertyChangedCallback((CategoryImageSourcePropertyChangedCallBack))));


        public readonly static DependencyProperty ToolTipTextProperty = DependencyProperty.Register(
            nameof(ToolTipText), typeof(string), typeof(ExpenseCategoryControl), new PropertyMetadata("General",new PropertyChangedCallback((ToolTipTextPropertyChangedCallBack))));

        private static void ToolTipTextPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ExpenseCategoryControl categoryCtrl)
            {
                return;
            }
            if (e.NewValue?.Equals(e.OldValue) is true)
            {
                return;
            }
            if (e.NewValue is string and not null)
            {
                categoryCtrl.ToolTipText = e.NewValue as string;
                return;
            }
            categoryCtrl.ToolTipText = e.OldValue as string;
        }

        public string ToolTipText
        {
            get => (string)GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }


        private static void CategoryImageSourcePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExpenseCategoryControl control && e.NewValue is not null)
            {
                control.CategoryImage.Source = (ImageSource)e.NewValue;
            }
        }

        public ImageSource CategoryIconSource
        {
            get => (ImageSource)GetValue(CategoryIconSourceProperty);
            set => SetValue(CategoryIconSourceProperty, value);
        }




        public ExpenseCategoryControl()
        {
            _viewModel = ActivatorUtilities.CreateInstance<CategoryControlViewModel>(App.Container, this);
            InitializeComponent();
            
        }

        #region  EVENTS_REGION

        private void CatogoriesFlyOut_Opening(object sender, object e)
        {
            if (_viewModel.Categories.Count > 0)
            {
                return;
            }
            _viewModel.LoadData();
        }

        private void SubCategory_ListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListView selectedListView)
            {
                return;
            }
            var selectedCategory = GetMultiListViewSingularExpenseCategorySelection(selectedListView);
            if (selectedCategory != null)
            {
                ToolTipText = selectedCategory.Name;
                OnExpenseCategorySelected?.Invoke(selectedCategory);
            }
        }

        private void CategoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsFlyOutOpeningAllowed)
            {
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
                return;
            }
            CategoryFlyout.Hide();
        }

        #endregion

        private ListView _previousSelectionMadeListControl;
        /// <summary>
        /// only allows one item to be selected in a whole collection of ListView and
        /// Returns selected item and unselecting Previous selection
        /// </summary>
        /// <param name="selectedListView"></param>
        /// <returns></returns>
        private ExpenseCategory GetMultiListViewSingularExpenseCategorySelection(ListView selectedListView)
        {

            if (_previousSelectionMadeListControl is null)
            {
                _previousSelectionMadeListControl = selectedListView;
                return (ExpenseCategory)selectedListView.SelectedItem;

            }
            if (_previousSelectionMadeListControl != selectedListView)
            {
                _previousSelectionMadeListControl.SelectedIndex = -1;
                _previousSelectionMadeListControl = selectedListView;
                return (ExpenseCategory)selectedListView.SelectedItem;
            }
            return (ExpenseCategory)selectedListView.SelectedItem;

        }
    }


    public class CategoryControlViewModel : ObservableObject
    {
        private readonly IView _view;
        private bool _isCategoryLoading;


        public ObservableCollection<ExpenseCategoryBobj> Categories { get; } = new ObservableCollection<ExpenseCategoryBobj>();

        public bool IsCategoryLoading
        {
            get => _isCategoryLoading;
            set => SetProperty(ref _isCategoryLoading, value);
        }

        public CategoryControlViewModel(IView view)
        {
            _view = view;

        }

        
        /// <summary>
        /// Call to Expense category UseCase To Fetch Data
        /// </summary>
        public void LoadData()
        {
            if (Store.Categories.Count > 0)
            {
                Categories.AddRange(Store.Categories);
                return;
            }

            IsCategoryLoading = true;   

            var fetchCategoryReq = new FetchExpenseCategoryRequest(CancellationToken.None, new CategoryControlVmPresenterCb(this));

            var fetchCategoryUseCaseObj = InstanceBuilder.CreateInstance<FetchExpenseCategory>(fetchCategoryReq);

            fetchCategoryUseCaseObj?.Execute();
        }


        class CategoryControlVmPresenterCb : IPresenterCallBack<FetchExpenseCategoryResponse>
        {
            private readonly CategoryControlViewModel _viewModel;

            public CategoryControlVmPresenterCb(CategoryControlViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            public async void OnSuccess(FetchExpenseCategoryResponse result)
            {
                await RunOnUiThread(() =>
                {
                    _viewModel.IsCategoryLoading = false;
                    _viewModel.Categories.ClearAndAdd(result.Categories);

                }, _viewModel._view.Dispatcher).ConfigureAwait(false);
            }
            public void OnError(SplittrException ex)
            {
                ExceptionHandlerService.HandleException(ex);
            }
        }

    }
    public interface IView
    {
        CoreDispatcher Dispatcher { get; }
        XamlRoot XamlRoot { get; }
    }
}
