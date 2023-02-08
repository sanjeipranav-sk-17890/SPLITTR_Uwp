using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models;
using SQLite;
using SPLITTR_Uwp.Core.UseCase.CreateGroup;
using SPLITTR_Uwp.Core.UseCase.UserSuggestion;
using SPLITTR_Uwp.Core.DataManager;

namespace SPLITTR_Uwp.ViewModel
{
   
    internal class GroupCreationPageViewModel : ObservableObject,IViewModel
    {
        
       
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

        public ObservableCollection<User> UserSuggestionList { get; } = new ObservableCollection<User>();

        public ObservableCollection<User> GroupParticipants { get; } = new ObservableCollection<User>();

        public GroupCreationPageViewModel()
        {
            Store.CurreUserBobj.ValueChanged += UserBobj_ValueChanged;
            User = new UserViewModel(Store.CurreUserBobj);
        }


        private User _dummyUser = new User()
        {
            UserName = "No results Found"
        };
        public void PopulateSuggestionList(string userName)
        { 
            //to be Made static Cancel previous Request if Another Made 
            var cts = new CancellationTokenSource().Token;
            var fetchSuggestionReqObj = new UserSuggestionRequestObject(new GroupCreationPageVmPresenterCallBack(this), cts, userName);

            var suggestionFetchUseCase = InstanceBuilder.CreateInstance<UserSuggestion>(fetchSuggestionReqObj);

            suggestionFetchUseCase.Execute();
        }

        //User Suggestion Call BAck
        public async void OnSuggestionRecievd(UserSuggestionResponseObject response)
        {
            await UiService.RunOnUiThread((() =>
            {
                foreach (var suggestedUser in response.UserSuggestions)
                {
                    if (GroupParticipants.Contains(suggestedUser)) continue; //suggestion is not showed if the user is already added to group participants
                    UserSuggestionList.Add(suggestedUser);
                }
                if (!UserSuggestionList.Any())
                {
                    UserSuggestionList.Add(_dummyUser);
                }

            }));
        }

        //Group Creation Success Call Back
        public void GroupCreateButtonClicked(object sender, RoutedEventArgs e)
        {
            var groupName = _groupName.Trim();

            var token = new CancellationTokenSource().Token;

            var groupCreationRequestObject = new GroupCreationRequestObj(token, new GroupCreationPageVmPresenterCallBack(this), Store.CurreUserBobj, GroupParticipants, groupName);

            var groupCreationUseCase = InstanceBuilder.CreateInstance<GroupCreation>(groupCreationRequestObject);

            groupCreationUseCase.Execute();
        }
        
        private async void UserBobj_ValueChanged(string property)
        {
            //since this Will be called by Worker thread it needs to invoked by Ui thread so calling dispatcher to user it
            
          await UiService.RunOnUiThread((() =>
            {
                      BindingUpdateInvoked?.Invoke();

            }));

        }

        public event Action BindingUpdateInvoked;


        class GroupCreationPageVmPresenterCallBack : IPresenterCallBack<GroupCreationResponseObj>, IPresenterCallBack<UserSuggestionResponseObject>
        {
            private readonly GroupCreationPageViewModel _viewModel;
            public GroupCreationPageVmPresenterCallBack(GroupCreationPageViewModel viewModel)
            {
                _viewModel = viewModel;

            }
            public async void OnSuccess(GroupCreationResponseObj result)
            {
                await UiService.RunOnUiThread(async () =>
                {
                    await UiService.ShowContentAsync($"{result.CreatedGroup.GroupName} Group Created SuccessFull", "SuccessFully Created !! ");

                    //clearing groupAdding PAge controls 
                    _viewModel.GroupParticipants.Clear();
                    _viewModel.GroupName = string.Empty;
                });
            }
            void IPresenterCallBack<GroupCreationResponseObj>.OnError(SplittrException ex)
            {
                HandleError(ex);
            }
            public void OnSuccess(UserSuggestionResponseObject result)
            {
                _viewModel.OnSuggestionRecievd(result);
            }
            void IPresenterCallBack<UserSuggestionResponseObject>.OnError(SplittrException ex)
            {
                HandleError(ex);
            }
            private void HandleError(SplittrException ex)
            {
                switch (ex.InnerException)
                {
                    case ArgumentException or ArgumentNullException:
                        ExceptionHandlerService.HandleException(ex.InnerException);
                        break;
                    case SQLiteException:
                        //Retry Code Logic Here
                        break;
                }
            }

        }
    }
}
