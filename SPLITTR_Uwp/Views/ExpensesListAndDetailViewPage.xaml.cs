using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        public ExpensesListAndDetailViewPage()
        {
            this.InitializeComponent();
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


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (ItemsSource is ObservableCollection<ExpenseGroupingList> groupedExpenses)
            //{
            //    groupedExpenses.CollectionChanged += GroupedExpenses_CollectionChanged;
            //}
        }

        //private void GroupedExpenses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //   if(sender is not ObservableCollection<ExpenseGroupingList> groupedExpense)
        //   {
        //        return;
        //   }

        //   foreach (var group in groupedExpense)//from each group setting visibility to default  
        //   {
        //       foreach (var expense in group)
        //       {
        //           if(expense is ExpenseViewModel expenseVm)
        //           {
        //               expenseVm.Visibility = true;

        //           }
        //       }
        //   }
        //}

        private void ExpensesListAndDetailViewPage_OnLoaded(object sender, RoutedEventArgs args)
        {
            CollectionViewSource.Source = ItemsSource;
            ExpensesLIstView.ItemsSource = CollectionViewSource.View;

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
    }
}
