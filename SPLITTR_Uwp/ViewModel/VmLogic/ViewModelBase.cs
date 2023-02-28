using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.ViewModel.Contracts;

namespace SPLITTR_Uwp.ViewModel.VmLogic
{
    public abstract class ViewModelBase : ObservableObject, IViewModel
    {
        public event Action BindingUpdateInvoked;

        public void InvokeBindingsUpdate()
        {
            BindingUpdateInvoked?.Invoke();
        }
    }
}
