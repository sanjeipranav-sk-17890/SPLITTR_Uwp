using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace SPLITTR_Uwp.Services
{
    internal static class UiService
    {
        
        
            /// <summary>
            /// Message Box whixh displays Content and Wait for User To repond
            /// </summary>
            /// <param name="content"></param>
            /// <param name="title"></param>
            /// <returns></returns>
            public async static Task ShowContentAsync(string content = "",string title= "")
            {
                await RunOnUiThread(async () =>
                {
                    MessageDialog msg = new MessageDialog(content, title);

                    msg.Commands.Add(new UICommand("close"));
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
