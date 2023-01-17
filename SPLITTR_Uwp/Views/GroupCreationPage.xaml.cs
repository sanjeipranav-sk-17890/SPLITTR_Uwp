using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupCreationPage : Page
    {
        GroupCreationPageViewModel _viewModel;
        public GroupCreationPage()
        {
            _viewModel = ActivatorUtilities.CreateInstance<GroupCreationPageViewModel>(App.Container);
            this.InitializeComponent();
            _viewModel.GroupParticipants.CollectionChanged += GroupParticipants_CollectionChanged;
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;

        }

        private void _viewModel_BindingUpdateInvoked()
        {
            Bindings.Update();
        }

        private void GroupParticipants_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var groupParticipants = sender as ObservableCollection<User>;
            GroupCreateButton.IsEnabled = !(groupParticipants?.Count < 1); // allowing group creation when participant count is atleat 2
        }

        private void GroupNameTextBlock_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var groupNameTextBlock = sender as TextBox;
            if (groupNameTextBlock?.Text.Length > 3)
            {
                GroupMembersStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                GroupMembersStackPanel.Visibility = Visibility.Collapsed;
            }

        }
        
        private void RemovePersonButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var objToBeRemoved = button?.DataContext as User;

            _viewModel.GroupParticipants.Remove(objToBeRemoved);
        }

       
        private  void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //clearing previous suggestions
            _viewModel.UserSuggestionList.Clear();

            var inputText = sender.Text.Trim();
            if (inputText.Length <= 1)// no call for suggestion if no text
            {
                return;
            }
            _viewModel.PopulateSuggestionList(inputText);

            

        }
        private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selectedPerson = (User)args.SelectedItem;
            if ( string.IsNullOrEmpty(selectedPerson.EmailId))
            {
                UserSearchAutoSuggestbox.Text = string.Empty;//clearing Suggestion box after person selection made
                return;
            }
            _viewModel.GroupParticipants.Add(selectedPerson);
            UserSearchAutoSuggestbox.Text = string.Empty;//clearing Suggestion box after person selection made

        }

        private string GetUserInitial(string username) => username.GetUserInitial();
    }
}
