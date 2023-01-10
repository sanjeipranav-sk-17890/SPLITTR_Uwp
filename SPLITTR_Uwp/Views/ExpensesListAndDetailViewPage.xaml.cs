using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ColorCode.Common;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpensesListAndDetailViewPage : Page
    {
        private DataStore _store;
        private ExpenseListAndDetailedPageViewModel _viewModel;
        public ExpensesListAndDetailViewPage()
        {
            this.InitializeComponent();
            _store = ActivatorUtilities.GetServiceOrCreateInstance<DataStore>(App.Container);
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseListAndDetailedPageViewModel>(App.Container);
        }


        public event Action PaneButtonOnClick;

        public readonly static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
                typeof(ExpensesListAndDetailViewPage), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));


        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ExpensesListAndDetailViewPage control)
                control.OnItemsSourceChanged((IEnumerable)e.NewValue);
        }

        public readonly static DependencyProperty TitleTextProperty = DependencyProperty.Register(
            nameof(TitleText), typeof(string), typeof(ExpensesListAndDetailViewPage), new PropertyMetadata(default(string)));

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }



        private void OnItemsSourceChanged(IEnumerable newValue)
        {

            // Add Csv Source for newValue.CollectionChanged (if possible)
            if (newValue is not ObservableCollection<ExpenseGroupingList> newSource)
            {
                return;
            }
            CollectionViewSource.Source = newSource;
            ExpensesLIstView.ItemsSource = CollectionViewSource.View;

        }

        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty) as IEnumerable;
            set => SetValue(ItemsSourceProperty, value);
        }

        public ObservableCollection<ExpenseBobj> DateSortedExpenseList { get; } = new ObservableCollection<ExpenseBobj>();




        private void ExpensesListAndDetailViewPage_OnLoaded(object sender, RoutedEventArgs args)
        {
            CollectionViewSource.Source = ItemsSource;
            ExpensesLIstView.ItemsSource = CollectionViewSource.View;

            //Subsribing To Collection Changed So DateTime Sorting ExpenseList Will Update AccordingLy
            if (ItemsSource is ObservableCollection<ExpenseGroupingList> groupedExpenseList)
            {
                groupedExpenseList.CollectionChanged += GroupedExpenseList_CollectionChanged;
            }

        }


        //Update Ui 
        private void GroupedExpenseList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<ExpenseGroupingList> groupedExpenseList)
            {
                SortExpenseBasedOnDate(groupedExpenseList);
            }
        }

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
                if (expense is ExpenseViewModel expenseVm)
                {
                    expenseVm.Visibility= ! expenseVm.Visibility;
                    expenseVisibility = expenseVm.Visibility;
                }
            }
            AssignButtonStyleBasedOnVisibility(expenseVisibility,sideButton);
           

        }
        private void AssignButtonStyleBasedOnVisibility(bool expenseVisibility,Button sideButton)
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
        private void NavigationPaneName_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationPaneControlButton.Style = NavigationPaneControlButton.Style == OpenPaneButtonStyle ? ClosePaneButtonStyle : OpenPaneButtonStyle;
            PaneButtonOnClick?.Invoke();
        }

        private void SortingFlyoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(sender is not MenuFlyoutItem selectedItem)
            {
                return; 
            }
            //if Showing list view is Date Time sorted then No Need to change list view 
            if (selectedItem == DateMenuItem && (DateSortedList.Visibility != Visibility.Visible))
            {
                
                if (ItemsSource is ObservableCollection<ExpenseGroupingList> groupedExpenses)
                {
                    //Sorts Expenses Bases On Date and Add it to Observable Collection
                    SortExpenseBasedOnDate(groupedExpenses);
                }

                //Setting visibility of dateListview and Hiding Grouped Listview
                ExpensesLIstView.Visibility = Visibility.Collapsed;
                DateSortedList.Visibility = Visibility.Visible;

            }
            if (selectedItem == StatusMenuItem && (ExpensesLIstView.Visibility != Visibility.Visible))
            {

                ExpensesLIstView.Visibility = Visibility.Visible;
                DateSortedList.Visibility= Visibility.Collapsed;

            }

        }

        private void SortExpenseBasedOnDate(ObservableCollection<ExpenseGroupingList> groupedExpenses)
        {
            DateSortedExpenseList.Clear();
            var expensesToBeSorted = new List<ExpenseBobj>();
            foreach (var expenses in groupedExpenses)
            {
                expensesToBeSorted.AddRange<ExpenseBobj>(expenses);
            }
            var expenseArray = expensesToBeSorted.ToArray();
            Array.Sort(expenseArray, new ExpenseDateSorter());

            foreach (var expense in expenseArray)
            {
                DateSortedExpenseList.Add(expense);
            }

        }

       

        private void ExpensesLIstView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.ExpenseSelectionMade();
        }
    }

 
}
