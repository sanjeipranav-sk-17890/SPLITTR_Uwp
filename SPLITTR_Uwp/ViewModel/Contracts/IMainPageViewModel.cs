using System.Collections.ObjectModel;
using System.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel.Contracts;

internal interface IMainPageViewModel : INotifyPropertyChanged
{
    string UserInitial { get; set; }

    UserVobj UserVobj { get; }

    ObservableCollection<User> RelatedUsers { get; }

    ObservableCollection<GroupBobj> UserGroups { get; }

    void ViewLoaded();
    void LogOutRequested();
    void ViewDisposed();
}
