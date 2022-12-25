using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Services
{
    internal class ExceptionHandlerService : INotifyPropertyChanged
    {
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        private static ExceptionHandlerService _instance;
        private string _errorMessage;
        public ExceptionHandlerService()
        {
            _instance ??= this;
        }

        public static void HandleException(Exception exception)
        {
            if (_instance != null)
            {
             _instance.ErrorMessage = exception?.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
