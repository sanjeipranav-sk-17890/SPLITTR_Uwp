using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    internal class GroupCreationPageViewModel : ObservableObject,IValueConverter
    {
        private readonly IGroupUtility _groupUtility;
        private readonly DataStore _store;
        private readonly IUserUtility _userUtility;
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

        public ObservableCollection<User> UserSuggestionList = new ObservableCollection<User>();

        public ObservableCollection<User> GroupParticipants = new ObservableCollection<User>();

        public GroupCreationPageViewModel(DataStore store,IUserUtility userUtility,IGroupUtility groupUtility)
        {
            _store = store;
            _userUtility = userUtility;
            _groupUtility = groupUtility;
            _store.UserBobj.ValueChanged += UserBobj_ValueChanged;
            User = new UserViewModel(_store.UserBobj);

        }


        private User _dummyUser = new User()
        {
            UserName = "No results Found"
        };
        public Task PopulateSuggestionList(string userName)
        {
           return _userUtility.GetUsersSuggestionAsync(userName.ToLower(), async (suggestions) =>
            {
                await  UiService.RunOnUiThread((() =>
                {
                    foreach (var suggestedUser in suggestions)
                    {
                        if(GroupParticipants.Contains(suggestedUser)) continue; //suggestion is not showed if the user is already added to group participants
                        UserSuggestionList.Add(suggestedUser);
                    }
                    if (!UserSuggestionList.Any())
                    {
                        UserSuggestionList.Add(_dummyUser);
                    }

                }));
            });
        }
        public void GroupCreateButtonClicked(object sender, RoutedEventArgs e)
        {
            var groupName = _groupName.Trim();

            if (_groupUtility is IUseCase useCase) //On Failed doing Respective Actions
            {
                useCase.OnError += UseCase_OnError;
            }

            _groupUtility.CreateSplittrGroup(GroupParticipants,_store.UserBobj,groupName, async () =>
            {
                await UiService.RunOnUiThread(() =>
                {
                    Debug.WriteLine("******************************Group Created Successfully *******************************************************");
                });
            });
        }

        private void UseCase_OnError(Exception arg1, string arg2)
        {
            throw new NotImplementedException();
        }

      
        

        private async void UserBobj_ValueChanged()
        {
            //since this Will be called by Worker thread it needs to invoked by Ui thread so calling dispatcher to user it
            
          await UiService.RunOnUiThread((() =>
            {
                        OnPropertyChanged(nameof(GetCurrentUserInitial));

            }));

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
