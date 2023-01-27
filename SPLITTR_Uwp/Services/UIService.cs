using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SPLITTR_Uwp.Services
{
    internal static class UiService
    {


        /// <summary>
        /// Message Box whixh displays Content and Wait for User To repond
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static Task ShowContentAsync(string content = "",string title= "",XamlRoot rootElement = null)
            {
                return RunOnUiThread(async () =>
                {
                    rootElement ??= Window.Current.Content.XamlRoot;
                    var msg = new ContentDialog
                    {
                        Title = title,
                        Content = content,
                        CloseButtonText = "close",
                        XamlRoot = rootElement
                    };
                    await msg.ShowAsync();
                });
            }

        /// <summary>
        /// Runs assigned Block of code in Ui Thread
        /// </summary>
        /// <param name="function"></param>
        /// <param name="applicationViewId"></param>
        /// <returns></returns>
        public async static Task RunOnUiThread(Action function,int applicationViewId=default)
            {
                var dispatcher = CoreApplication.MainView.Dispatcher;
                if (applicationViewId != default)
                {
                   dispatcher = CoreApplication.Views.FirstOrDefault(v => ApplicationView.GetApplicationViewIdForWindow(v.CoreWindow) == applicationViewId)?.Dispatcher;
                }

                if (dispatcher != null)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        function?.Invoke();
                    });
                    
                }
            }
            
    }
}
