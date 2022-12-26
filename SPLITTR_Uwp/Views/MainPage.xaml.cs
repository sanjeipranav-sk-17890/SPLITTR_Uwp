using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel;

namespace SPLITTR_Uwp.Views
{
    public sealed partial class MainPage : Page
    {
        MainPageViewModel _viewModel;
        
        public Frame InnerFrameobjref { get; set; }
        public MainPage()
        {
            _viewModel = ActivatorUtilities.CreateInstance<MainPageViewModel>(App.Container,this);
            InitializeComponent();
            InnerFrameobjref = InnerFrame;
            Loaded += _viewModel.AppLicationStart;
        }


        public void ListBoxItemSelected(object sender, SelectionChangedEventArgs e)
        {
            //Closing home navigationButton's  flyOut  if opened 
            if (HomeNavigationButtonFlyout.IsOpen)
            {
                HomeNavigationButtonFlyout.Hide();
            }

            // setting the Title bar of the mainPage from the selected listbox item in ViewModel
            _viewModel.ListBoxItemSelected(sender,e);
           
            // To Navigated to the respective page on the user choice  in ViewModel

        }

        DispatcherTimer _timer=new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        private void ErrorMEssageTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void ErrorCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            //force stoping the timer if Cross button is clicked on error message
            if (_timer?.IsEnabled??false)
            {

                _timer?.Stop();
                ErrorShowingContent.Visibility= Visibility.Collapsed;

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
    }
}
