using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;

namespace SPLITTR_Uwp.ViewModel.Contracts;

internal interface IMainPageViewModel : INotifyPropertyChanged,IViewModel
{
    string UserInitial { get; set; }

    UserVobj UserVobj { get; }

    bool IsUpdateWalletBalanceTeachingTipOpen { get; set; }

    ObservableCollection<User> RelatedUsers { get; }


    ObservableCollection<GroupBobj> UserGroups { get; }

    bool PaneVisibility { get; set; }

    void ViewLoaded();
    void LogOutButtonClicked();
    void PersonProfileClicked();
    void AddButtonItemSelected(object sender, RoutedEventArgs e);
    void AddWalletBalanceButtonClicked();
    void ViewDisposed();
}
