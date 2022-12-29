using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using SPLITTR_Uwp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Services;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPageVersion2 : Page
    {
        MainPageViewModelV2 _viewModel;

        public MainPageVersion2()
        {
            _viewModel = ActivatorUtilities.CreateInstance<MainPageViewModelV2>(App.Container, this); ;
            this.InitializeComponent();
        }


        DispatcherTimer _timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };
       
        private void ErrorCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            //force stoping the timer if Cross button is clicked on error message
            if (_timer?.IsEnabled ?? false)
            {

                _timer?.Stop();
                ErrorShowingContent.Visibility = Visibility.Collapsed;

            }

        }
        private void ExceptionHandlerService_OnNotifyErrorMessage(string message)
        {
            //assigning value to error showing pop up box 
            ErrorMEssageTExtBox.Text = message ?? string.Empty;

            //Enabling visibility of the error box
            ErrorShowingContent.Opacity = 5;
            ErrorShowingContent.Visibility = Visibility.Visible;

            _timer.Tick += (sender, e) =>
            {
                //using dispatch timer to slowly fading the error box
                ErrorShowingContent.Opacity -= .2;
                if (ErrorShowingContent.Opacity < 0)
                {
                    _timer.Stop();
                    ErrorShowingContent.Visibility = Visibility.Collapsed;
                }
            };
            _timer.Start();
        }

        //private void NavigationViewss_OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    var navView = MainPageNavigationView;
        //    var rootGrid = VisualTreeHelper.GetChild(navView, 0) as Grid;
        //    //finding root split view and setting background
        //    var grid = VisualTreeHelper.GetChild(rootGrid, 1) as Grid;
        //    var rootSplitView = VisualTreeHelper.GetChild(grid, 1) as SplitView;
        //    var rootinnerGrid = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
        //    var color = (SolidColorBrush)Application.Current.Resources["ApplicationMainThemeColor"];
        //    rootinnerGrid.Background = color;
        //}




        private void AppIcon_OnClick(object sender, TappedRoutedEventArgs e)
        {
            MainPageNavigationView.IsPaneOpen = true;
            NavigationService.Frame = InnerFrame;
            NavigationService.Navigate(typeof(ExpensesListAndDetailViewPage),_viewModel.ExpensesList);
        }
        private void UserSelectedFromIndividualSplitList(User selectedUser)
        {
            if (selectedUser is null)
            {
                return;
            }
            _viewModel.PopulateUserRelatedExpenses(selectedUser);
        }
        private void MainPageNavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args .SelectedItem is not StackPanel stackpanel)
            {
                return;
            }
            switch (stackpanel.Name)
            {
                case nameof(AllExpense)  :
                    _viewModel.PopulateAllExpense();
                    break;
                case nameof(RequestToMe) :
                    _viewModel.PopulateUserRecievedExpenses();
                    break;
                case nameof(RequestedByMe):
                    _viewModel.PopulateUserRaisedExpenses();
                    break;

            }
            
        }
        private void GroupList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var groupObject = e.AddedItems[0] as GroupBobj;
                _viewModel.PopulateSpecificGroupExpenses(groupObject);
            }
        }
    }


}
