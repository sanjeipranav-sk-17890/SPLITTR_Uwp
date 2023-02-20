using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class MainPageButtonControl : UserControl
    {

        private readonly UserViewModel _currentUserViewModel;

        public MainPageButtonControl()
        {
            this.InitializeComponent();
            // fetching current user details to avoid showing user as participants in each and every group
            _currentUserViewModel = new UserViewModel(Store.CurreUserBobj);
          //  OnArrowButtonClicked += MainPageButtonControl_OnArrowButtonClicked;
        }

        #region SingularityOPenLogic

        private void PerformOpenListView()
        {
            if (UserListView.Visibility == Visibility.Collapsed)
            {
                ShowListViewButton.Visibility = Visibility.Collapsed;
                HideListViewButton.Visibility = Visibility.Visible;
                UserListView.Visibility = Visibility.Visible;
            }
            else
            {
                PerformCloseListView();
            }
        }
        private void PerformCloseListView()
        {
            ShowListViewButton.Visibility = Visibility.Visible;
            HideListViewButton.Visibility = Visibility.Collapsed;
            UserListView.Visibility = Visibility.Collapsed;

        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            PerformOpenListView();
        }


        #endregion


        public readonly static DependencyProperty TitleNameProperty = DependencyProperty.Register(
            nameof(TitleName), typeof(string), typeof(MainPageButtonControl), new PropertyMetadata(default(string)));

        public string TitleName
        {
            get => (string)GetValue(TitleNameProperty);
            set => SetValue(TitleNameProperty, value);
        }

        //public readonly static DependencyProperty UserListSourceProperty = DependencyProperty.Register(
        //    nameof(UserListSource), typeof(IList<User>), typeof(MainPageButtonControl), new PropertyMetadata(default(IList<User>)));


        public readonly static DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            nameof(SelectionMode), typeof(ListViewSelectionMode), typeof(MainPageButtonControl), new PropertyMetadata(default(ListViewSelectionMode)));

        public ListViewSelectionMode SelectionMode
        {
            get => (ListViewSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set
            {   
                var listWithCurrentUserBsObject = AlterInputListWithCurrentUserBobj(value);
                SetValue(ItemsSourceProperty, listWithCurrentUserBsObject);
            }
        }

      

        //replacing ordinary current  user obj with userVm so Support Change 
        private IEnumerable AlterInputListWithCurrentUserBobj(IEnumerable value)
        {
            if (value is  ObservableCollection<User> )
            {
                return value;
            }
            
            if (value is IList<User> users)
            {
                return users.Select(((user) =>
                {
                    if (user.Equals(_currentUserViewModel))
                    {
                        return (User)_currentUserViewModel;
                    }
                    return user;
                }));
            }
            return value;
        }

        public readonly static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
                typeof(MainPageButtonControl), new PropertyMetadata(null));

        public event Action<User> UserSelectedFromTheList;
        private void UserListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if(e.AddedItems.Any())
             {
                 var selectedUser = (User)e.AddedItems[0];
                 UserListView.SelectedIndex = -1;// resetting selection index , selection the same user may raise Selection changed again 
                 UserSelectedFromTheList?.Invoke(selectedUser);
             }
        }
    }
}
