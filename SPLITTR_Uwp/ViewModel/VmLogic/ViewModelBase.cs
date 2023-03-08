using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.ViewModel.Contracts;

namespace SPLITTR_Uwp.ViewModel.VmLogic;

public abstract class ViewModelBase : ObservableObject
{
    public event Action BindingUpdateInvoked;

    protected void InvokeBindingsUpdate()
    {
        BindingUpdateInvoked?.Invoke();
    }
}