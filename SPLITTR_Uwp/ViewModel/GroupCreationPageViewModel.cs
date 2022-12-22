using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    internal class GroupCreationPageViewModel : ObservableObject,IValueConverter
    {
        private readonly DataStore _store;
        private string _groupName;


        public UserViewModel User { get;}
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        public string GetCurrentUserInitial
        {
            get => User.UserName.GetUserInitial();
        }

        public ObservableCollection<User> GroupParticipants = new ObservableCollection<User>();

        public GroupCreationPageViewModel(DataStore store)
        {
            _store = store;
            _store.UserBobj.ValueChanged += UserBobj_ValueChanged;
            User = new UserViewModel(_store.UserBobj);

        }

        private async void UserBobj_ValueChanged()
        {
            //since this Will be called by Worker thread it needs to invoked by Ui thread so calling dispatcher to user it
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                        OnPropertyChanged(nameof(GetCurrentUserInitial));
                }
            );

        }



        #region UserInitialConvertRegion
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var name = (string)value;
            return name.GetUserInitial();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
