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
using Microsoft.UI.Xaml.Controls;
using SPLITTR_Uwp.Core.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubcategoryIconControl : UserControl
    {
        public ExpenseCategory ExpenseCategory
        {
            get => DataContext as ExpenseCategory;
        }
        public SubcategoryIconControl()
        {
            this.InitializeComponent();
            DataContextChanged += (sender, args) => Bindings.Update();
        }

        public readonly static DependencyProperty TeachingTipContentProperty = DependencyProperty.Register(
            nameof(TeachingTipContent), typeof(string), typeof(SubcategoryIconControl), new PropertyMetadata(default(string)));

        public string TeachingTipContent
        {
            get => (string)GetValue(TeachingTipContentProperty);
            set => SetValue(TeachingTipContentProperty, value);
        }

        public readonly static DependencyProperty IsInFoTipOpenProperty = DependencyProperty.Register(
            nameof(IsInFoTipOpen), typeof(bool), typeof(SubcategoryIconControl), new PropertyMetadata(default(bool)));

        public bool IsInFoTipOpen
        {
            get => (bool)GetValue(IsInFoTipOpenProperty);
            set => SetValue(IsInFoTipOpenProperty, value);
        }

        private static TeachingTip _singletonTeachingTip;
        private static TeachingTip SingletonTeachingTip
        {
            get => _singletonTeachingTip ??= CreateTeachingTip();
        }

        private static TeachingTip CreateTeachingTip()
        {
            return new TeachingTip()
            {
                Style = App.Current.Resources["NoCloseButtonTeachingTipstyle"] as Style,

            };
        }

        private void SubcategoryIconControl_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // IsInFoTipOpen=true;
            if (sender is FrameworkElement element)
            {
                SingletonTeachingTip.Content = ExpenseCategory.Name;
                SingletonTeachingTip.Target = element;
            }
            SingletonTeachingTip.IsOpen= true;

        }

        private void UIElement_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (SingletonTeachingTip == null)
            {
                return;
            }
            SingletonTeachingTip.IsOpen = false; 
            SingletonTeachingTip.Content = SingletonTeachingTip.Target = null;
        }

        private void InfoTeachingTip_OnClosed(TeachingTip sender, TeachingTipClosedEventArgs args)
        {

            Debug.WriteLine($"{ExpenseCategory.Name} Teaching Tip Closed Event is  Fired ");

        }
        private void InfoTeachingTip_OnClosing(TeachingTip sender, TeachingTipClosingEventArgs args)
        {
            Debug.WriteLine($"{ExpenseCategory.Name} Teaching Tip Closing Closing Event is  Fired ");
        }
    }
}
