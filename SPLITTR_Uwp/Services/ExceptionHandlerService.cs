using System;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Services
{
    internal class ExceptionHandlerService 
    {
 
        public event Action<string> NotifyErrorMessage;


        private static ExceptionHandlerService _instance;


        public ExceptionHandlerService()
        {
            _instance ??= this;
        }

        public static void HandleException(Exception exception)
        {
            if (_instance != null)
            {
             _instance?.OnErrorNotifyMessage(exception?.Message);
            }
        }


        protected async virtual Task OnErrorNotifyMessage(string obj)
        {
            await UiService.RunOnUiThread((() =>
            {

                NotifyErrorMessage?.Invoke(obj);
            }));
        }
    }
}
