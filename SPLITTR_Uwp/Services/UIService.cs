using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
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
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        public static Task ShowContentAsync(string content = "",string title= "",XamlRoot rootElement = null,CoreDispatcher dispatcher = null)
        {
            return RunOnUiThread(async () =>
                {
                    rootElement ??= Window.Current.Content.XamlRoot;
                    var msg = new ContentDialog
                    {
                        Title = title,
                        Content = content,
                        CloseButtonText = "close",
                        XamlRoot = rootElement,
                        RequestedTheme = ThemeHelperService.GetPreferenceThemeIfSet()
                    };
                    await msg.ShowAsync();
                },dispatcher);
        }
        
        /// <summary>
        /// Runs assigned Block of code in Ui Thread
        /// </summary>
        /// <param name="function"></param>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        public async static Task RunOnUiThread(Action function, CoreDispatcher dispatcher = null)
        {
            dispatcher ??= CoreApplication.MainView.Dispatcher;

            if (dispatcher != null)
            {
                await dispatcher.RunOnUIContextAsync(() =>
                {
                    function?.Invoke();
                });
            }
        }

    }
}
