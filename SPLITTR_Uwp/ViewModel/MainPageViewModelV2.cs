using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.ExtensionMethod;

namespace SPLITTR_Uwp.ViewModel
{
    internal class MainPageViewModelV2 :ObservableObject
    {
        
            private readonly DataStore _store;
            private readonly MainPageVersion2 _mainPage;
            private string _userInitial;
            private bool _isUpdateWalletBalanceTeachingTipOpen;


            public MainPageViewModelV2(DataStore store, MainPageVersion2 mainPage)
            {
                _store = store;
                _mainPage = mainPage;
                UserViewModel = new UserViewModel(_store.UserBobj);
                _store.UserBobj.ValueChanged += UserObjUpdated;

            }

  
  


            public string UserInitial
            {
                get => UserViewModel.UserName.GetUserInitial();
                set => SetProperty(ref _userInitial, value);
            }
            public UserViewModel UserViewModel { get; }


            public bool IsUpdateWalletBalanceTeachingTipOpen
            {
                get => _isUpdateWalletBalanceTeachingTipOpen;
                set => SetProperty(ref _isUpdateWalletBalanceTeachingTipOpen, value);
            }

            public void AppLicationStart(object sender, RoutedEventArgs e)
            {

                if (_store.UserBobj == null)
                {
                    NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());
                    return;
                }

            }

            public async void UserObjUpdated()
            {
                //since this Will be called by Worker thread it needs to invoked by Ui thread so calling dispatcher to user it
                await UiService.RunOnUiThread((() =>
                {
                    OnPropertyChanged(nameof(UserInitial));
                }));

            }

            public void LogOutButtonClicked()
            {
                _store.UserBobj = null;
                NavigationService.Frame = null;
                NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());

            }
            public void PersonProfileClicked()
            {
                
                NavigationService.Frame = _mainPage.InnerFrame;
                NavigationService.Navigate<UserProfilePage>();
            }

        
            public void AddWalletBalanceButtonClicked()
            {
                IsUpdateWalletBalanceTeachingTipOpen = !IsUpdateWalletBalanceTeachingTipOpen;
            }

            public void AddButtonItemSelected(object sender, RoutedEventArgs e)
            {
                var selectedItem = sender as MenuFlyoutItem;
                var title = selectedItem.Text;
                NavigationService.Frame = _mainPage.InnerFrame;
                if (string.Compare(title, "Add Exepense", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    NavigationService.Navigate(typeof(AddExpenseTestPage), new DrillInNavigationTransitionInfo());
                }
                else
                {
                    NavigationService.Navigate(typeof(GroupCreationPage), new DrillInNavigationTransitionInfo());
                }
            }

        
}
}
