using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.ViewModel.Models
{
    public class GroupViewModel : GroupBobj,INotifyPropertyChanged
    {
        private readonly GroupBobj _group;


        public string GroupCreatedDate
        {
            get => CreateDateTime.ToString("D");
        }

        

        public  GroupViewModel(GroupBobj group) : base(group)
        {
            _group = group;
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

 
    }
}
