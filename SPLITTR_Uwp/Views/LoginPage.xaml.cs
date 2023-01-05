using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        LoginPageViewModel _viewModel;
        public LoginPage()
        {
            this.InitializeComponent();
            _viewModel = App.Container.GetService<LoginPageViewModel>();
            this.DataContextChanged += (sender, args) => Bindings.Update();
            
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.PageUnloaded();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //=================Bypass Login========================//
             //To be Deleted
            //_viewModel.LoginButtonPressed();
        }

    }
}
