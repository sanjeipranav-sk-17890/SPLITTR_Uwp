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
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class ExpenseDataTemplateV2 : UserControl
    {
        private ExpenseItemViewModel _viewModel;

        public ExpenseViewModel ExpenseObj
        {
            get => this.DataContext as ExpenseViewModel;
        }

        public ExpenseDataTemplateV2()
        {
            this.InitializeComponent();
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseItemViewModel>(App.Container);
            this.DataContextChanged += ExpenseDataTemplateV2_DataContextChanged;
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;
          
        }

        private void _viewModel_BindingUpdateInvoked()
        {
           Bindings.Update();
        }

        private void ExpenseDataTemplateV2_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ExpenseObj is null)
            {
                return;
            }
            Bindings.Update();
            _viewModel.ExpenseObjLoaded(ExpenseObj);
        }
    }
}
