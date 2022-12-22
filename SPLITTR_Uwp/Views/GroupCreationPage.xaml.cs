using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
