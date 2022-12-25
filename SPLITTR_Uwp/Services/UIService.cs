using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Popups;
using Windows.UI.Xaml;

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
                await App.Current.Resources.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,()=>
                {
                   function?.Invoke();
                });
            }
    }
}
