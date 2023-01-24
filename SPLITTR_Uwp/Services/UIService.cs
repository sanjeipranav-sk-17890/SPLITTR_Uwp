using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
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
                    var msg = new ContentDialog();
                    msg.Title = title;
                    msg.Content = content;
                    msg.CloseButtonText = "close";
                    await msg.ShowAsync();
                });
            }

            /// <summary>
            /// Runs assigned Block of code in Ui Thread
            /// </summary>
            /// <param name="function"></param>
            /// <returns></returns>
            public async static Task RunOnUiThread(Action function)
            {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,()=>
                { 
                   function?.Invoke();
                });
            }
    }
}
