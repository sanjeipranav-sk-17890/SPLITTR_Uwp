using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SPLITTR_Uwp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SPLITTR_Uwp.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.Views
{
    public sealed partial class SplitExpenseUserControl : UserControl,ISplitExpenseView
    {
        private readonly SplitExpenseViewModel _viewModel;
        public SplitExpenseUserControl()
        {
            _viewModel = ActivatorUtilities.CreateInstance<SplitExpenseViewModel>(App.Container,this);
            this.InitializeComponent();
            this.DataContext = _viewModel;
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;
            Loaded += SplitExpenseUserControl_Loaded;
        }

        private void SplitExpenseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Bindings.Update();
        }

        private void _viewModel_BindingUpdateInvoked()
        {
           Bindings.Update();
        }

        private void UnEqualSplitTeachingTip_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Pointer entered UnequalTEaching tip");
        }

        public XamlRoot VisualRoot
        {
            get
            {
                return XamlRoot.Content.XamlRoot;
            }
        }

        public int ApplicationViewId
        {
            get
            {
                return ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow);
            }
        }

        public SplitExpenseUserControl(SplitExpenseUserControl control)
        {
            _viewModel = control._viewModel;
            _viewModel.BindingUpdateInvoked +=  _viewModel_BindingUpdateInvoked;
            this.InitializeComponent();
            Loaded += SplitExpenseUserControl_Loaded;
        }

    }



    public interface ISplitExpenseView
    {
        public XamlRoot VisualRoot { get;}

        public int ApplicationViewId { get;}
    }
}
