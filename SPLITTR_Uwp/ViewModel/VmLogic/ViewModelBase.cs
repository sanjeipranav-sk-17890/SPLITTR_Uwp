using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.ViewModel.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
