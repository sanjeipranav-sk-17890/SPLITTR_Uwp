using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SPLITTR_Uwp.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class ThemeChangerButton : UserControl
    {
        public ThemeChangerButton()
        {
            InitializeComponent();
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
