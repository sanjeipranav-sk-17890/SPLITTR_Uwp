using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace SPLITTR_Uwp.Services;

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

    public static void HandleException(Exception exception)
    {
        if (exception == null)
        {
            return;
        }
        _ = UiService.RunOnUiThread(() =>
        {
            if (NotificationControl is not null)
            {
                NotificationControl.Dismiss();
                NotificationControl.Content = exception.Message;
                NotificationControl?.Show(2000);
            }
               
        }).ConfigureAwait(false);

        /*/
         *If Exception is Internet not available exception show Network connection failed symbol and retry option template
         *Log Exception And its stack trace in a class File
         */
        if (exception.InnerException is not null)
        {
            Debug.WriteLine(exception.Message,exception.InnerException.StackTrace);
        }
          
    }

}