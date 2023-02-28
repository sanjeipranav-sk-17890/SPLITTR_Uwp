using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ExpensesListAndDetailViewPage : Page
{
    private readonly ExpenseListAndDetailedPageViewModel _viewModel;
    public ExpensesListAndDetailViewPage()
    {
        InitializeComponent();
        _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseListAndDetailedPageViewModel>(App.Container);
            
    }

    public event Action PaneButtonOnClick;

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {    //clearing previous subscriptions 
        PaneButtonOnClick = null;
        OnGroupInfoIconClicked = null;
    }


    private void ExpensesLIstView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count <= 0)
        {
            return;
        }
        TryChangeVisualState();
        _viewModel.SelectedExpenseObj = e.AddedItems[0] as ExpenseVobj;
        _viewModel.ExpenseSelectionMade();
    }
    private void TryChangeVisualState()
    {
        if (CurrentState.Equals(nameof(ListAndDetailViewState)))
        {
            return;
        }
        TrySetLayOutVisualState(nameof(DetailExpenseViewState));
        PreferedState = nameof(DetailExpenseViewState);
    }
    private void UserExpensesListControl_OnPaneButtonOnClick()
    {
        PaneButtonOnClick?.Invoke();
    }

    private void ExpensesListAndDetailViewPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!(Window.Current.Bounds.Width < 800))
        {
            return;
        }
        PreferedState ??= nameof(ExpenseListViewState);
        TrySetLayOutVisualState(PreferedState);
    }
    private string PreferedState { get; set; }

    private void OnBackButtonClicked(object sender, RoutedEventArgs arg2)
    {
        TrySetLayOutVisualState(nameof(ExpenseListViewState));
        PreferedState = nameof(ExpenseListViewState);
    }

    private void TrySetLayOutVisualState(string visualState)
    {
        VisualStateManager.GoToState(this,visualState, true);

    }
    private string CurrentState
    {
        get => AdaptiveListAndDetailView.CurrentState.Name;
    }

    public event Action<Group> OnGroupInfoIconClicked;
    private void ExpensesListControl_OnOnGroupInfoButtonClicked(Group obj)
    {
        OnGroupInfoIconClicked?.Invoke(obj);
    }
}