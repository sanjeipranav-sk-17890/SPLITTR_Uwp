using SPLITTR_Uwp.Services;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class ThemeChangerButton : UserControl
    {
        public ThemeChangerButton()
        {
            this.InitializeComponent();
        }
        private async void ApplicationThemeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is not Frame rootFrame)
            {
                return;
            }
            if (rootFrame.ActualTheme == ElementTheme.Light)
            {

              await  ThemeHelperService.ChangeTheme(ElementTheme.Dark);
                return;
            }
            // ApplicationThemeButton.Content = LightModeIcon;
              await ThemeHelperService.ChangeTheme(ElementTheme.Light);

        }
       
    }
}
