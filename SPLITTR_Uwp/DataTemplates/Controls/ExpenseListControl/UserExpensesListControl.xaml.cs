using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel.Vobj;
using SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls.ExpenseListControl;

public sealed partial class UserExpensesListControl : UserControl,IExpenseListControl
{
    private ExpensesListControlViewModel _viewModel;

    private readonly List<ExpenseListGroupingObj> _groupingObjects = new List<ExpenseListGroupingObj>();

    private readonly IExpenseGrouperFactory _expenseGrouperFactory;

    private static UserExpensesListControl ListControl { get; set; }

    public UserExpensesListControl()
    {
        _viewModel = ActivatorUtilities.CreateInstance<ExpensesListControlViewModel>(App.Container, this);
        _expenseGrouperFactory = ActivatorUtilities.GetServiceOrCreateInstance<IExpenseGrouperFactory>(App.Container);

        //From Factory Populating List from Di Container for Grouping Purposes
        PopulateGroupiungList();

        InitializeComponent();
        ListControl = this;
        Loaded += UserExpensesListControl_Loaded;
        Unloaded += OnUnloaded;
    }


    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        _viewModel.ViewDisposed();
    }

    private void PopulateGroupiungList()
    {
        _groupingObjects.Add(new ExpenseListGroupingObj(_expenseGrouperFactory.GetGrouperInstance("Category"),"Category"));
        _groupingObjects.Add(new ExpenseListGroupingObj(_expenseGrouperFactory.GetGrouperInstance("Status"), "Status"));
    }

    private void UserExpensesListControl_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.OnExpenseListControlLoaded();
    }

      
    public static void FilterExpense(ExpenseListFilterObj requestObj)
    {
        if (ListControl is null)
        {
            return;
        }
        ListControl._viewModel.FilterExpensesToBeShown(requestObj);

    }

        
    public event Action PaneButtonOnClick;
    private void NavigationPaneName_OnClick(object sender, RoutedEventArgs e)
    {
        //Changing Appearance of pane button and invoking click event 
        if (PaneButtonStateGroup.CurrentState is not null && PaneButtonStateGroup.CurrentState.Name == nameof(OpenPaneState))
        {
            VisualStateManager.GoToState(this, nameof(ClosePaneState), true);
            PaneButtonOnClick?.Invoke();
            return;
        }
        VisualStateManager.GoToState(this, nameof(OpenPaneState), true);
        PaneButtonOnClick?.Invoke();
    }


    public event Action<object, SelectionChangedEventArgs> OnExpenseSelectionChanged;
    private void ExpensesLIstView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OnExpenseSelectionChanged?.Invoke(sender,e);
    }

    //Changing selected  Groups Visibility 
    private void ExpenseShowButton_OnClick(object sender, RoutedEventArgs e)
    {
        //if button rotation is 90 set it to 0 and vice versa
        if (sender is not Button sideButton)
        {
            return;
        }

        if (sideButton.DataContext is not ExpenseGroupingList expenseGroup)
        {
            return;
        }
        var expenseVisibility = true;

        foreach (var expense in expenseGroup)
        {
            if (expense is not ExpenseVobj expenseVm)
            {
                continue;
            }
            expenseVm.Visibility = !expenseVm.Visibility;
            expenseVisibility = expenseVm.Visibility;
        }
        AssignButtonStyleBasedOnVisibility(expenseVisibility, sideButton);


    }


    private void AssignButtonStyleBasedOnVisibility(bool expenseVisibility, Button sideButton)
    {
        if (expenseVisibility)
        {

            var style = ShowExpenseButtonStyle;
            sideButton.Style = style;
        }
        else
        {
            var style = HideExpenseButtonStyle;
            sideButton.Style = style;
        }
    }

        
    public void SetCollectionListSource(IEnumerable Source)
    {
        // Add Csv Source for newValue.CollectionChanged 
        if (Source is not ObservableCollection<ExpenseGroupingList> newSource)
        {
            return;
        }
        CollectionViewSource.Source = newSource;
        ExpensesLIstView.ItemsSource = CollectionViewSource.View;

    }

    private void SortingFlyoutButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuFlyoutItem selectedItem)
        {
            return;
        }
        if (selectedItem == DateMenuItem)
        {
            //Setting visibility of dateListView and Hiding Grouped ListView
            VisualStateManager.GoToState(this, nameof(SortingState), false);
            return;

        }
        if (selectedItem == StatusMenuItem )
        {
          VisualStateManager.GoToState(this, nameof(GroupingState), false);
        }
    }

    public event Action<Group> OnGroupInfoButtonClicked ;
    private void ExpenseDataTemplate_OnOnGroupInfoButtonClicked(Group obj)
    {
        OnGroupInfoButtonClicked?.Invoke(obj);
    }

    private void GroupingList_ONSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListView groupList)
        {
            return;
        }
        GroupByNameTextBlock.Text = _groupingObjects[groupList.SelectedIndex].GroupByName;
        _viewModel.ExpenseGrouper = _groupingObjects[groupList.SelectedIndex].GrouperObj;
    }
}
public interface IExpenseListControl
{
    void SetCollectionListSource(IEnumerable Source);

}


/*Linq Code To Query Category From MainCategory List
 */
///// <summary>
///// Queries Icon From the list of Main Categories and Returns Its icon source link
///// </summary>
///// <param name="mainCategories"></param>
///// <param name="categoryId"></param>
///// <returns></returns>
//private ExpenseCategory FetchCategoryById(IEnumerable<ExpenseCategoryBobj> mainCategories, int categoryId)
//{
//    ExpenseCategory subCategory = default;
//    var parentCategory = mainCategories.FirstOrDefault(ex => ex.SubExpenseCategories.FirstOrDefault(IfSubCategoryExistAssign) is not null);

//    return subCategory;

//    //Checks if Requested Category id respective id matched, and assign to local variable if matches
//    bool IfSubCategoryExistAssign(ExpenseCategory sub)
//    {
//        if (sub.Id != categoryId)
//        {
//            return false;
//        }
//        subCategory = sub;
//        return true;
//    }
//}
