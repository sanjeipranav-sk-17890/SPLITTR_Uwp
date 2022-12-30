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




        public readonly static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
                typeof(ExpensesListAndDetailViewPage), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));


        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ExpensesListAndDetailViewPage control)
                control.OnItemsSourceChanged((IEnumerable)e.NewValue);
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
            if (e.Parameter is ObservableCollection<ExpenseGroupingList> collection)
            {
                ItemsSource = collection;
            }

            base.OnNavigatedFrom(e);
        }

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
            bool expenseVisibility = false;

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
            };
        }
        private void NavigationPaneName_OnClick(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
