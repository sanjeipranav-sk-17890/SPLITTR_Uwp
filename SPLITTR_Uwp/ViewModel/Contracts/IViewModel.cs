using System;
using System.ComponentModel;

namespace SPLITTR_Uwp.ViewModel.Contracts;

internal interface IViewModel : INotifyPropertyChanged
{
    event Action BindingUpdateInvoked;
}