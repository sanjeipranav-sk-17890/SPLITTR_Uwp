using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SPLITTR_Uwp.Core.Models;

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
        private void ErrorMEssageTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }
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

            //Enbling visibility of the error box
            ErrorShowingContent.Opacity = 5;
            ErrorShowingContent.Visibility = Visibility.Visible;

            _timer.Start();
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
        }

        private void NavigationViewss_OnLoaded(object sender, RoutedEventArgs e)
        {
            var navView = NavigationViewss;
            var rootGrid = VisualTreeHelper.GetChild(navView, 0) as Grid;
            //finding root split view and setting background
            var grid = VisualTreeHelper.GetChild(rootGrid, 1) as Grid;
            var rootSplitView = VisualTreeHelper.GetChild(grid, 1) as SplitView;
            var rootinnerGrid = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
            var color = (SolidColorBrush)Application.Current.Resources["ApplicationMainThemeColor"];
            rootinnerGrid.Background = color;
        }


      

    }


}
