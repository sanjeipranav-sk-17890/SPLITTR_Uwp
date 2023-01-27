using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SPLITTR_Uwp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IMainView,INotifyPropertyChanged
    {
        IMainPageViewModel _viewModel;

        public MainPage()
        {
            _viewModel = ActivatorUtilities.CreateInstance<MainPageViewModel>(App.Container, this);
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;
            this.InitializeComponent();

        }

        private void _viewModel_BindingUpdateInvoked()
        {
            Bindings.Update();
        }


        #region Error Handling MEchanisam

        DispatcherTimer _timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        private void ErrorCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            //force stoping the timer if Cross button is clicked on error message
            if (_timer.IsEnabled)
            {

                _timer?.Stop();
                ErrorShowingContent.Visibility = Visibility.Collapsed;

            }

        }
        private void ExceptionHandlerService_OnNotifyErrorMessage(string message)
        {
            //assigning value to error showing pop up box 
            ErrorMEssageTExtBox.Text = message ?? string.Empty;

            //Enabling visibility of the error box
            ErrorShowingContent.Opacity = 5;
            ErrorShowingContent.Visibility = Visibility.Visible;

            _timer.Tick += (sender, e) =>
            {
                //using dispatch timer to slowly fading the error box
                ErrorShowingContent.Opacity -= .2;
                if (ErrorShowingContent.Opacity < 0)
                {
                    _timer.Stop();
                    ErrorShowingContent.Visibility = Visibility.Collapsed;
                }
            };
            _timer.Start();
        }



        #endregion


        private string _innerPageTitle = nameof(AllExpense);

        private void AppIcon_OnClick(object sender, TappedRoutedEventArgs e)
        {
            MainPageNavigationView.IsPaneOpen = true;
            NavigationService.Frame = InnerFrame;
            NavigationService.Navigated += NavigationService_Navigated;
            NavigationService.Navigate(typeof(ExpensesListAndDetailViewPage));
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            
            if (InnerFrame.Content is not ExpensesListAndDetailViewPage page)
            {
                return;
            }
            page.ItemsSource = _viewModel.ExpensesList;
            page.PaneButtonOnClick += (PageOnPaneButtonOnClick);
            //setting binding manually  for title in that Page 
            Binding binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(InnerPageTitle)),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(page, ExpensesListAndDetailViewPage.TitleTextProperty, binding);

            //Unsubscribing NavigationServices
            NavigationService.Navigated -= NavigationService_Navigated;
        }
        

        private void PageOnPaneButtonOnClick()
        {
            var isMainPaneOpen = MainPageNavigationView.IsPaneOpen;
            MainPageNavigationView.IsPaneOpen = !isMainPaneOpen;
        }

        private void UserSelectedFromIndividualSplitList(User selectedUser)
        {
            if (selectedUser is null)
            {
                return;
            }
            //Setting Title
            InnerPageTitle = "Individual Split"+$" : {selectedUser.UserName}" ;


            _viewModel.PopulateUserRelatedExpenses(selectedUser);
        }
        private void MainPageNavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not StackPanel stackpanel)
            {
                return;
            }
            //setting Title and Calling respective viewModel To Polulate the Respective Expenses
            switch (stackpanel.Name)
            {
                case nameof(AllExpense):
                    InnerPageTitle = nameof(AllExpense);
                    _viewModel.PopulateAllExpense();
                    break;
                case nameof(RequestToMe):
                    InnerPageTitle = nameof(RequestToMe);
                    _viewModel.PopulateUserRecievedExpenses();
                    break;
                case nameof(RequestedByMe):
                    InnerPageTitle = nameof(RequestedByMe);
                    _viewModel.PopulateUserRaisedExpenses();
                    break;

            }

        }
        private void GroupList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var groupObject = e.AddedItems[0] as GroupBobj;
                //setting title with Group Name
                InnerPageTitle = groupObject.GroupName + " Expense";


                //Calling ViewModel to Populate Group objects
                _viewModel.PopulateSpecificGroupExpenses(groupObject);
            }
        }


        public Frame ChildFrame
        {
            get => InnerFrame;
        }

        public string InnerPageTitle
        {
            get => _innerPageTitle;
            set => SetField(ref _innerPageTitle, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
