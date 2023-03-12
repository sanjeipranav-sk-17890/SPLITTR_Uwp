using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Vobj;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls;

public sealed partial class ExpenseDetailedViewUserControl : UserControl
{
    public ExpenseVobj ExpenseObj
    {
        get => DataContext as ExpenseVobj;
    }

    private readonly ExpenseDetailedViewUserControlViewModel _viewModel;

    public ExpenseDetailedViewUserControl()
    {
        _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseDetailedViewUserControlViewModel>(App.Container);
        InitializeComponent();
        DataContextChanged += ExpenseDetailedViewUserControl_DataContextChanged; 
        Unloaded += (sender, args) => _viewModel.ViewDisposed();
    }

    private ExpenseVobj _previousExpenseVobj = default;
    private void ExpenseDetailedViewUserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {

        //No change in Detailed View Subscription if Same Object is been Selected
        if (ExpenseObj is null || _previousExpenseVobj?.ExpenseUniqueId.Equals(ExpenseObj.ExpenseUniqueId) is true)
        {
            return;
        }
        _previousExpenseVobj = ExpenseObj;
        ExpenseObj.PropertyChanged += ExpenseObj_PropertyChanged;
        ManipulateUiBasedOnDataContext();
        Bindings.Update();
    }

    private void ExpenseObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        //Listing To Only status Change in Expense Status
        if (e.PropertyName.Equals(nameof(ExpenseObj.ExpenseStatus)))
        {
            ManipulateUiBasedOnDataContext();
        }
    }

    private void ManipulateUiBasedOnDataContext()
    {

        //UnderGoing VM logic to Fetch Related Expenses 
        _viewModel.ExpenseObjLoaded(ExpenseObj);

        GroupNameTextBlock.Visibility = ExpenseObj.GroupUniqueId is null ? Visibility.Visible: Visibility.Collapsed;

        IndividualSplitIndicatorTextBlock.Visibility = ExpenseObj.GroupUniqueId is null ? Visibility.Collapsed : Visibility.Visible;

        AssignExpenseStatusForeGround();

    }
    private  void AssignExpenseStatusForeGround()
    {
        ExpenseStatusBrush.Color = ExpenseObj.ExpenseStatus switch
        {
            ExpenseStatus.Pending => Colors.DarkRed,
            ExpenseStatus.Cancelled => Colors.Orange,
            _ =>Colors.DarkGreen
        };

    }

    public event Action<object, RoutedEventArgs> BackButtonClicked; 
    private void ListViewBackButton_OnClick(object sender, RoutedEventArgs e)
    {
        BackButtonClicked?.Invoke(this, e);
    }
    private void ExpenseCategoryControl_OnOnExpenseCategorySelected(ExpenseCategory obj)
    {
        _viewModel.CategoryChangeSelected(obj);
    }
}