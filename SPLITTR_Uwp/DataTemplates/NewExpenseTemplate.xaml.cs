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
using SPLITTR_Uwp.ViewModel.Models;
using System.ServiceModel.Channels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class NewExpenseTemplate : UserControl
    {
        //private ExpenseViewModel ExpenseViewModel
        //{
        //    get => this.DataContext as ExpenseViewModel;
        //}

        public NewExpenseTemplate()
        {
            this.InitializeComponent();
          //  this.DataContextChanged += (s, e) => Bindings.Update();

        }





    }
}
