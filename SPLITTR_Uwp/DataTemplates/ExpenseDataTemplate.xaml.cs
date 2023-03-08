using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Vobj;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates;

public sealed partial class ExpenseDataTemplate : UserControl
{
    private readonly ExpenseItemViewModel _viewModel;

    public ExpenseVobj ExpenseObj
    {
        get => DataContext as ExpenseVobj;
    }

    public ExpenseDataTemplate()
    {
        _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseItemViewModel>(App.Container);
        InitializeComponent();
        DataContextChanged += ExpenseDataTemplateV2_DataContextChanged;
        Unloaded += ExpenseDataTemplate_Unloaded;
    }

    private void ExpenseDataTemplate_Unloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.ViewDisposed();
    }

    private void ExpenseDataTemplateV2_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, nameof(OnPointerLeaved), false);

        //work around Since TextBlock not Changes when Decoration set to none 
        DescriptionTextBox.Text = string.Empty;
        DescriptionTextBox.TextDecorations = TextDecorations.None;
        DescriptionTextBox.Text = ExpenseObj?.Description ?? string.Empty;
    }
    private void ExpenseDataTemplateV2_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, nameof(OnPointerOver), false);
    }

    private void ExpenseDataTemplateV2_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (ExpenseObj is null)
        {
            return;
        }
        _viewModel.ExpenseObjLoaded(ExpenseObj);
        Bindings.Update();
    }


    public event Action<Group> OnGroupInfoButtonClicked;
    private void GroupInfoButtonOnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: Group } groupInfoBtn)
        {
            OnGroupInfoButtonClicked?.Invoke(groupInfoBtn.DataContext as Group);
        }
    }
    private void ExpenseCategoryControl_OnOnExpenseCategorySelected(ExpenseCategory obj)
    { 
        _viewModel.ChangeExpenseCategory(obj);
    }
}