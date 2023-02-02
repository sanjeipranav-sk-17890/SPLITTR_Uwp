using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;

namespace SPLITTR_Uwp.Services
{
    internal class AccentColorService
    {
        private readonly static UISettings UiSettings;

        private readonly static Dictionary<UIContext,CoreWindow> AppWindows= new Dictionary<UIContext, CoreWindow>();

        static AccentColorService()
        {
            UiSettings = new UISettings();
            UiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        private async static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            //Changing Registered Windows Title Bar Color If System Accent Changes 
            foreach (var window in AppWindows)
            {
               await ChangeTitleBarColor(window.Value);
            }
            // var titleBarBackGroundColorChangeTasks = AppWindows.Select(v => ChangeTitleBarColor(v.Value)).ToList();

            //Task.WhenAll(titleBarBackGroundColorChangeTasks);
        }

        public async static void Register(CoreWindow appWindow)
        {
            if (appWindow == null)
            {
                return;
            }
            //Changing Title Bar to Current Accent Color
            await ChangeTitleBarColor(appWindow);

            AppWindows.Add(appWindow.UIContext,appWindow);
        }

        private async static Task ChangeTitleBarColor(ICoreWindow appWindow)
        {
           await appWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                var view = ApplicationView.GetForCurrentView();
                var applicationThemeColor = (Color)Application.Current.Resources["SystemAccentColorLight1"];
                view.TitleBar.BackgroundColor = applicationThemeColor;
                view.TitleBar.InactiveBackgroundColor = applicationThemeColor;
                view.TitleBar.ButtonInactiveBackgroundColor = applicationThemeColor;
                view.TitleBar.ButtonBackgroundColor = applicationThemeColor;
            }));

        }
        public static void UnRegister(CoreWindow appWindow)
        {
            if (appWindow != null)
            {
                AppWindows.Remove(appWindow.UIContext);
            }
        }
    }


}
