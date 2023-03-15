using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Vobj;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls;

public sealed partial class ExpenseDetailedViewControl : UserControl
{
    public ExpenseVobj ExpenseObj
    {
        get => DataContext as ExpenseVobj;
    }

    private readonly ExpenseDetailedViewUserControlViewModel _viewModel;

    public ExpenseDetailedViewControl()
    {
        _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseDetailedViewUserControlViewModel>(App.Container);
        InitializeComponent();
        DataContextChanged += ExpenseDetailedViewUserControl_DataContextChanged; 
        Unloaded += (sender, args) => _viewModel.ViewDisposed();
        Loaded += OnLoaded;
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DateOfExpenseDatePicker.MaxYear = new DateTimeOffset(DateTime.Today);
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
    private void DateOfExpenseDatePicker_OnSelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
    {
        if (args.NewDate?.DateTime > DateTime.Now)
        {
            DateOfExpenseDatePicker.SelectedDate = ExpenseObj?.DateOfExpense;
            return;
        }
        _viewModel.DateOfExpense = DateOfExpenseDatePicker.SelectedDate ?? ExpenseObj.DateOfExpense;
        _viewModel.EditExpenseDetails();
    }

    private void ExpenseTitle_FocusLost(object sender, RoutedEventArgs e)
    {
        if (!ExpenseTitleTextBox.Text.Equals(ExpenseObj?.Description))
        {
            _viewModel.ExpenseTitle = ExpenseTitleTextBox.Text;
            _viewModel.EditExpenseDetails();
        }
    }
    private void ExpenseNotesTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (!ExpenseNotesTextBox.Text.Equals(ExpenseObj?.Note))
        {
            _viewModel.ExpenseNote = ExpenseNotesTextBox.Text;
            _viewModel.EditExpenseDetails();
        }
    }
    #region  VisualStates&Behaviour

    private void DateOfExpenseDatePicker_OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, nameof(DateInfoFocusReceived), false);
    }
    private void DateOfExpenseDatePicker_OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, nameof(DateInfoFocusLost), false);
    }
    #endregion


 
    private void ExpenseNotesTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, nameof(ExpenseNoteOnFocusRecieved), false);
       
    }
    private void ExpenseNotesTextBox_OnPoniterExited(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, nameof(ExpenseNoteOnFocusLost), false);
    }


   
}