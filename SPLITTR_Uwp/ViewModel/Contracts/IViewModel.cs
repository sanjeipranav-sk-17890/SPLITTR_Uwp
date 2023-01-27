using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.ViewModel.Contracts
{
    internal interface IViewModel : INotifyPropertyChanged
    {
        event Action BindingUpdateInvoked;
    }
}
