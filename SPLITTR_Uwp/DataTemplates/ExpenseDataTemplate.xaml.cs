using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;
using System.Reflection;
using Windows.UI.Text;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class ExpenseDataTemplate : UserControl
    {
        private readonly ExpenseItemViewModel _viewModel;

        public ExpenseViewModel ExpenseObj
        {
            get => this.DataContext as ExpenseViewModel;
        }

        public ExpenseDataTemplate()
        {
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseItemViewModel>(App.Container);
            this.InitializeComponent();
            this.DataContextChanged += ExpenseDataTemplateV2_DataContextChanged;
            Unloaded += ExpenseDataTemplate_Unloaded;
        }

        private void ExpenseDataTemplate_Unloaded(object sender, RoutedEventArgs e)
        {
           _viewModel.ViewDisposed();
        }

        private void ExpenseDataTemplateV2_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, nameof(OnPointerLeaved), false);
        }
        private void ExpenseDataTemplateV2_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, nameof(OnPointerOver), false);
        }

        private void ExpenseDataTemplateV2_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ExpenseObj is null)
            {
                return;
            }
            _viewModel.ExpenseObjLoaded(ExpenseObj);
            Bindings.Update();
        }
       
        

    }
}
