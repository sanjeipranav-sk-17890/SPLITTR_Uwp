using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel.Models.ExpenseListObject;

namespace SPLITTR_Uwp.ViewModel.Contracts;

internal interface IMainPageViewModel : INotifyPropertyChanged
{
    string UserInitial { get; set; }

    UserViewModel UserViewModel { get; }

    bool IsUpdateWalletBalanceTeachingTipOpen { get; set; }

    ObservableCollection<User> RelatedUsers { get; }

    ObservableCollection<ExpenseGroupingList> ExpensesList { get; }

    ObservableCollection<GroupBobj> UserGroups { get; }

    bool PaneVisibility { get; set; }

    void ViewLoaded();
    void PopulateAllExpense();
    void PopulateSpecificGroupExpenses(Group selectedGroup);
    void PopulateUserRelatedExpenses(User selectedUser);
    void PopulateUserRecievedExpenses();
    void PopulateUserRaisedExpenses();
    void UserObjUpdated();
    void LogOutButtonClicked();
    void PersonProfileClicked();
    void AddButtonItemSelected(object sender, RoutedEventArgs e);
    void AddWalletBalanceButtonClicked();

    event PropertyChangedEventHandler PropertyChanged;

    event PropertyChangingEventHandler PropertyChanging;
}
