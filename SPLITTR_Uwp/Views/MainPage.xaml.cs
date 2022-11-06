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



    }
}
