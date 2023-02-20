using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SPLITTR_Uwp.DataTemplates.Controls;

namespace SPLITTR_Uwp.Services
{
    internal static class ExceptionHandlerService 
    {
        private static InAppNotification _notificationControl;

        private static InAppNotification NotificationControl
        {
            get
            {
                return _notificationControl ??= Window.Current?.Content.FindDescendant("InAppNotification") as InAppNotification;

            }
        }

        public async static void HandleException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            await UiService.RunOnUiThread((() =>
            {
                NotificationControl?.Show(exception.Message,2500);
            })).ConfigureAwait(false);
            if (exception.InnerException is not null)
            {
                Debug.WriteLine(exception.Message,exception.InnerException.InnerException.StackTrace);
            }
          
        }

    }
}
