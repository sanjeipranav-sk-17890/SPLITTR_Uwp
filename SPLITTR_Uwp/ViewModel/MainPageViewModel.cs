using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Utility.Blogic;
using SPLITTR_Uwp.ViewModel.Models;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;

namespace SPLITTR_Uwp.ViewModel
{
    public class MainPageViewModel : ObservableObject
    {
        private readonly DataStore _store;
        private readonly MainPage _mainPage;
        private readonly IStringManipulator _manipulator;
        private readonly IUserUtility _userUtility;
        private bool _backButtonVisibility = false;
        private bool _isPaneOpen = false;
        private string _mainPageTitleBarText;
        private int _splitPaneSelectedIndex = 0;
        private string _userInitial;
        private int _previousSelectedIndex=-1;
        //private string _moneyTextBoxText;
        //private bool _invalidInputTextBlockVisibility;
        private bool _isUpdateWalletBalanceTeachingTipOpen;


        public MainPageViewModel(DataStore store, MainPage mainPage, IStringManipulator manipulator,IUserUtility userUtility)
        {
            _store = store;
            _mainPage = mainPage;
            _manipulator = manipulator;
            _userUtility = userUtility;
            UserViewModel = new UserViewModel(_store.UserBobj);
            _store.UserBobj.ValueChanged += UserObjUpdated;

        }

        public bool BackButtonVisibility
        {
            get => _backButtonVisibility;
            set => SetProperty(ref _backButtonVisibility, value);
        }

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => SetProperty(ref _isPaneOpen, value);
        }

        public string MainPageTitleBarText
        {
            get => _mainPageTitleBarText;
            set => SetProperty(ref _mainPageTitleBarText, value);
        }

        public int SplitPaneSelectedIndex
        {
            get => _splitPaneSelectedIndex;
            set => SetProperty(ref _splitPaneSelectedIndex, value);
        }

        
        public string UserInitial
        {
            get => _manipulator.GetUserInitial(UserViewModel.UserName);
            set => SetProperty(ref _userInitial, value);
        }
        public UserViewModel UserViewModel { get; }

       /* public string MoneyTextBoxText
        {
            get => _moneyTextBoxText;
            set => SetProperty(ref _moneyTextBoxText, value);
        }

        public bool InvalidInputTextBlockVisibility
        {
            get => _invalidInputTextBlockVisibility;
            set => SetProperty(ref _invalidInputTextBlockVisibility, value);
        }
*/
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
            await  Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    OnPropertyChanged(nameof(UserInitial));
                }
                );
            
        }
        public void PaneControlButtonOnClick()
        {
            IsPaneOpen = !IsPaneOpen;

        }
        public void ListBoxItemSelected(object sender, SelectionChangedEventArgs e)
        {
            
            

            if (SplitPaneSelectedIndex != -1)
            {
                //Recording the selected index for backButton Usage
                 _previousSelectedIndex = SplitPaneSelectedIndex;


                // setting the Title bar of the mainPage from the selected listbox item
                var listbox = sender as ListBox;
                var listBoxItem = listbox?.SelectedItem as ListBoxItem;
                var textBlock = (listBoxItem?.Content as StackPanel)?.Children[1] as TextBlock;
                MainPageTitleBarText = textBlock?.Text;

                // To Navigated to the respective page on the user choice 
                NavigationService.Frame = _mainPage.InnerFrameobjref;
                NavigationService.Navigate<TestPage>();
            }
            
        }

        public void LogOutButtonClicked()
        {
            _store.UserBobj = null;
            NavigationService.Frame = null;
            NavigationService.Navigate(typeof(LoginPage), new DrillInNavigationTransitionInfo());

        }
        public void PersonProfileClicked()
        {
            //Clearing the Selected tab in Hamburger Navigation and storing its previous selected index
            _previousSelectedIndex = SplitPaneSelectedIndex;
            SplitPaneSelectedIndex = -1;


            NavigationService.Frame = _mainPage.InnerFrameobjref;
            NavigationService.Navigate<UserProfilePage>();
            BackButtonVisibility = true;
            MainPageTitleBarText = "Me ";

        }

        public void BackButtonClicked()
        {
            BackButtonVisibility = false;
            SplitPaneSelectedIndex = _previousSelectedIndex;
        }
        public void AddWalletBalanceButtonClicked()
        {
            IsUpdateWalletBalanceTeachingTipOpen = !IsUpdateWalletBalanceTeachingTipOpen;
        }
        //public async void AddMoneyToWalletButtonClicked()
        //{
        //    //Wallet adding money should be non negative and a number
        //    if (double.TryParse(MoneyTextBoxText, out var newWalletBalance) && newWalletBalance > -1)
        //    {
        //        InvalidInputTextBlockVisibility = false;
        //       await _userUtility.UpdateUserObjAsync(_store.UserBobj, newWalletBalance);
        //       await ShowMessageBoxAsync("Amount Added to Wallet SuccessFully", "Payment SuccessFull!!");
        //       AddWalletBalanceButtonClicked();
        //    }
        //    else
        //    {
        //        InvalidInputTextBlockVisibility = true;
        //    }

        //}
        private async Task ShowMessageBoxAsync(string content,string title)
        {

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog msg = new MessageDialog(content, title);

                    msg.Commands.Add(new UICommand("close"));
                    await msg.ShowAsync();

                });

        }
        public async void AddButtonItemSelected(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as MenuFlyoutItem;
            var title=selectedItem.Text;
            if (string.Compare(title, "Add Exepense", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                NavigationService.Frame = _mainPage.InnerFrameobjref;
                NavigationService.Navigate(typeof(AddExpenseTestPage));
            }
            else
            {
                await ShowMessageBoxAsync("Add group Selected", "Group");
            }
        }
        
    }
}
