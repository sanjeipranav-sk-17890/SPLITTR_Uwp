using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;
using SPLITTR_Uwp.Core.UseCase.SplitExpenses;
using SPLITTR_Uwp.Core.UseCase.UserSuggestion;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace SPLITTR_Uwp.ViewModel
{



    public class SplitExpenseViewModel : ObservableObject, IViewModel
    {

        private ISplitExpenseView View { get; set; }

        private UserVobj User { get; }

        /// <summary>
        /// Contains Expenses obj which will be processed and inserted into db
        /// </summary>
        public readonly ObservableCollection<ExpenseBobj> _expensesToBeSplitted = new ObservableCollection<ExpenseBobj>();




        #region UserAutoSUggestionBox Logic Region
        private bool _isUserSuggestionListOpen;
        private string _splittingUsersName = string.Empty;

        public string SplittingUsersName
        {
            get => _splittingUsersName;
            set => SetProperty(ref _splittingUsersName, value);
        }

        public ObservableCollection<User> UsersList { get; } = new ObservableCollection<User>();


        public void TextBoxLostFocus()
        {
            ExpenseTextBoxValueChanged();
            if (_selectedUser == _dummyUser || _selectedUser == null && !_usersToBeSplitted.Any())
            {
                _isInnerInvokationOfTextChanged = true;
                SplittingUsersName = "";
                return;
            }

        }


        public bool IsUserSuggestionListOpen
        {
            get => _isUserSuggestionListOpen;
            set
            {
                _isUserSuggestionListOpen = value;
                OnPropertyChanged(nameof(IsUserSuggestionListOpen));
            }
        }



        private User _selectedUser;
        private readonly IList<User> _usersToBeSplitted = new List<User>();
        private int _selectedGroupIndex = 0;
        private string _selectedGroupName;
        private bool _isNameTextBoxReadOnly;
        private bool _isInnerInvokationOfTextChanged = false;


        public void TextBoxTextChanged()
        {
            //no Recommendation operation Should be done If TextBox is non editable for user interaction
            if (IsNameTextBoxReadOnly || _isInnerInvokationOfTextChanged)
            {
                //Changing ExpenseViewModels in Unequal Split Taeaching Tip ListView
                SplittingUserPreferenceChanged();
                _isInnerInvokationOfTextChanged = false;
                IsUserSuggestionListOpen = false;
                return;
            }
            if (SplittingUsersName.Trim().Length < 1)
            {
                _selectedUser = null;
                IsUserSuggestionListOpen = false;
                return;
            }
            UsersList.Clear();

            //to be Made static Cancel previous Request if Another Made 
            var cts = new CancellationTokenSource().Token;
            var fetchSuggestionReqObj = new UserSuggestionRequestObject(new SplitExpenseVmPresenterCallBack(this), cts, SplittingUsersName.Trim().ToLower());

            var suggestionFetchUseCase = InstanceBuilder.CreateInstance<UserSuggestion>(fetchSuggestionReqObj);

            suggestionFetchUseCase.Execute();

        }
        public async void InvokeOnUserSuggestionReceived(UserSuggestionResponseObject result)
        {
            await UiService.RunOnUiThread(() =>
            {
                foreach (var user in result.UserSuggestions)
                {
                    UsersList.Add(user);
                }
                if (!UsersList.Any())
                {
                    UsersList.Add(_dummyUser);
                }

                IsUserSuggestionListOpen = true;

            }, View.Dispatcher);
        }

        //if the splitting is successfull showing split completed text box and reset the page 
        public async void InvokeOnSplitExpenseCompleted(SplitExpenseResponseObj result)
        {

            await UiService.RunOnUiThread((() =>
            {
                UiService.ShowContentAsync("Spliting SuccessFull", "Expenses Splitted Successfully");
                ResetPage();
            }), View.Dispatcher);
        }

        private User _dummyUser = new User()
        {
            UserName = "No Results Found"
        };

        private int _selectedUserIndex = -1;



        public void ListViewOnSelected()
        {
            if (SelectedUserIndex == -1) return;

            _selectedUser = UsersList[SelectedUserIndex];

            //checking whether the selcted user obj is dummy obj for showing no results found and clearing text box
            if (string.IsNullOrEmpty(_selectedUser.EmailId))
            {
                _selectedUser = null;
                SplittingUsersName = "";
                return;

            }
            //clearing splitting user's list when single user is selected
            _usersToBeSplitted.Clear();

            _isInnerInvokationOfTextChanged = true;
            SplittingUsersName = _selectedUser?.UserName;
            UsersList.Clear();
            IsUserSuggestionListOpen = false;
        }



        #endregion

        #region GroupSelection logic region

        public int SelectedGroupIndex
        {
            get => _selectedGroupIndex;
            set => SetProperty(ref _selectedGroupIndex, value);
        }


        private ObservableCollection<GroupBobj> _userParticipatingGroups;
        public ObservableCollection<GroupBobj> UserParticipatingGroup
        {
            get
            {
                return _userParticipatingGroups ??= GetUserParticipatingGroup();
            }
        }


        public string SelectedGroupName
        {
            get => _selectedGroupName;
            set => SetProperty(ref _selectedGroupName, value);
        }

        public bool IsNameTextBoxReadOnly
        {
            get => _isNameTextBoxReadOnly;
            set => SetProperty(ref _isNameTextBoxReadOnly, value);
        }


        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public int SelectedUserIndex
        {
            get => _selectedUserIndex;
            set => SetProperty(ref _selectedUserIndex, value);
        }

        private GroupBobj _dummyGroup = new GroupBobj()
        {
            GroupName = "Non Group Expense"
        };



        private ObservableCollection<GroupBobj> GetUserParticipatingGroup()
        {
            //dummy GroupBobj to display default value

            var observableGroupObj = new ObservableCollection<GroupBobj>
            {
                _dummyGroup
            };

            //Calling UseCase To PopulateGroups
            CallUserGroupsUseCase();

            return observableGroupObj;

            void CallUserGroupsUseCase()
            {
                var getUSerGroupReqObj = new GetUserGroupReq(CancellationToken.None, new SplitExpenseVmPresenterCallBack(this), Store.CurreUserBobj);

                var getGroupsUseCase = InstanceBuilder.CreateInstance<GetUserGroups>(getUSerGroupReqObj);

                getGroupsUseCase.Execute();
            }
        }


        public void GroupItemSelectionChanged()
        {
            if (SelectedGroupIndex < 0)
            {
                return;
            }
            SelectedGroupName = UserParticipatingGroup[SelectedGroupIndex].GroupName;

            //if dummy is selected Clearing Users to be Splitted list so other UI Usecases work Properly

            SelectedGroupName = SelectedGroupName;

            if (SelectedGroupIndex == 0)
            {
                _isInnerInvokationOfTextChanged = true;
                SplittingUsersName = String.Empty;
                IsNameTextBoxReadOnly = false;
                return;
            }

            IsNameTextBoxReadOnly = true;
            _selectedUser = null;
            var splittingGroup = UserParticipatingGroup[SelectedGroupIndex];
            SplittingUsersName = String.Empty;
            _usersToBeSplitted.Clear();
            foreach (var user in splittingGroup.GroupParticipants)
            {
                _isInnerInvokationOfTextChanged = true;

                _usersToBeSplitted.Add(user);

                //ignoring current users name in the GROUP MEMBERS SPLITTING NAME
                if (user.EmailId == User.EmailId)
                    continue;

                //CONCATING REMAINING USERS NAME TO THE USERS NAME TEXT BOX
                SplittingUsersName += ("," + user.UserName);
            }
        }




        #endregion

        #region ExpenseAmountLogicRegion Single/Multiple  User


        private string _singleUserExpenseShareAmount;
        private bool _amountFormatIncorrectIndicatorVisibility = false;

        private bool _splitEquallyPanelVisibility;

        //By default the Selected option is SplitEqully
        private int _selectedSplitPreferenceIndex = 0;
        /*       private bool _singleUserSelectionComboBoxVisibility;*/

        public string SingleUserExpenseShareAmount
        {
            get => _singleUserExpenseShareAmount;
            set => SetProperty(ref _singleUserExpenseShareAmount, value);
        }

        public bool AmountFormatIncorrectIndicatorVisibility
        {
            get => _amountFormatIncorrectIndicatorVisibility;
            set => SetProperty(ref _amountFormatIncorrectIndicatorVisibility, value);
        }

        public bool SplitEquallyPanelVisibility
        {
            get => _splitEquallyPanelVisibility;
            set => SetProperty(ref _splitEquallyPanelVisibility, value);
        }

        public int SelectedSplitPreferenceIndex
        {
            get => _selectedSplitPreferenceIndex;
            set => SetProperty(ref _selectedSplitPreferenceIndex, value);
        }

        /*
                public bool SingleUserSelectionComboBoxVisibility
                {
                    get => _singleUserSelectionComboBoxVisibility;
                    set => SetProperty(ref _singleUserSelectionComboBoxVisibility, value);
                }


                public List<string> SingleUserSplitComboBoxItems()
                {
                    if (_selectedUser is null)
                    {
                        return new List<string>();
                    }
                    return new List<string>()
                    {
                        "Split This Bill",
                        $"{_selectedUser.UserName} owe the full Total ",
                        " You owe the Full Total"
                    };
                }
        */
        /*if (SelectedUser != null)
                        {
                            //calling iNotify to populate combo box items 
                            OnPropertyChanged(nameof(SingleUserSplitComboBoxItems));
                            SingleUserSelectionComboBoxVisibility = true;
                        }
                        else
                        {

                            SingleUserSelectionComboBoxVisibility = false;
                        }*/
        private double _equalSplitAmount;

        public void ExpenseTextBoxValueChanged()
        {
            if (string.IsNullOrEmpty(SingleUserExpenseShareAmount))
            {
                return;
            }
            if (double.TryParse(SingleUserExpenseShareAmount, out _equalSplitAmount) && _equalSplitAmount > -1)
            {
                //not showing indicator if parsing is Successfull 
                AmountFormatIncorrectIndicatorVisibility = false;

            }
            else
            {
                //showing indicator if parsing is Successfull 
                AmountFormatIncorrectIndicatorVisibility = true;
            }
        }
        public void SplitPreferenceChanged()
        {
            if (SelectedSplitPreferenceIndex == 0)
            {
                //NOt showing teaching tip if unequal split option is selected
                UneqaulSplitPopUpVisibility = false;

                SplitEquallyPanelVisibility = true;
                return;
            }
            //showing teaching tip if unequal split option is selected
            UneqaulSplitPopUpVisibility = true;
            SplitEquallyPanelVisibility = false;

        }




        #endregion

        #region ExpenseDateSelectionRegion

        private DateTime _expenditureDate = DateTime.Now;

        public DateTimeOffset ExpenditureDate
        {
            get => _expenditureDate;
            set => SetProperty(ref _expenditureDate, value.DateTime);
        }

        public DateTimeOffset MaxYearDisplayLimit
        {
            get => DateTimeOffset.Now;
        }


        #endregion

        #region UnEqualSplitPopUpLogicRegion
        private bool _uneqaulSplitPopUpVisibility;

        public bool UneqaulSplitPopUpVisibility
        {
            get => _uneqaulSplitPopUpVisibility;
            set => SetProperty(ref _uneqaulSplitPopUpVisibility, value);
        }




        public void UnequalSplitTeachingSplitClosed()
        {
            SelectedSplitPreferenceIndex = 0;
        }



        //Manupulates ExpenseViewModels for Unequal Splitting in teaching tip 
        private void SplittingUserPreferenceChanged()
        {
            _expensesToBeSplitted.Clear();

            //cheching whether it is dummy groupobj
            if (_selectedGroupIndex <= 0)
            {
                if (_selectedUser != null)//Individual Split
                {

                    _expensesToBeSplitted.Add(GenerateExpenseViewModel(Store.CurreUserBobj, null));//current User
                    _expensesToBeSplitted.Add(GenerateExpenseViewModel(_selectedUser, null));//Spiltting user

                }
            }
            else//Group Split 
            {

                var group = UserParticipatingGroup[_selectedGroupIndex];

                foreach (var participant in group.GroupParticipants)
                {
                    _expensesToBeSplitted.Add(GenerateExpenseViewModel(participant, group.GroupUniqueId));
                }
            }

        }

        private ExpenseBobj GenerateExpenseViewModel(User user, string groupUid)
        {
            return new ExpenseBobj(Store.CurreUserBobj.CurrencyConverter)
            {
                RequestedOwner = Store.CurreUserBobj.EmailId,
                SplitRaisedOwner = Store.CurreUserBobj, //Currently by default current user is splitRaiseOwner , 
                UserEmailId = user.EmailId,
                CorrespondingUserObj = user,
                StrExpenseAmount = 0.0,
                GroupUniqueId = groupUid
            };

        }

        #endregion

        #region ExpensesSplitFunctionality region

        private bool _isSplitButtonEnabled;
        private string _expenseNote;
        private string _expenseDescription;


        public bool IsSplitButtonEnabled
        {
            get => _isSplitButtonEnabled;
            set => SetProperty(ref _isSplitButtonEnabled, value);
        }

        public string ExpenseNote
        {
            get => _expenseNote;
            set => SetProperty(ref _expenseNote, value);
        }

        public string ExpenseDescription
        {
            get => _expenseDescription;
            set => SetProperty(ref _expenseDescription, value);
        }

        //event raises when collection item changes
        private void ExpensesToBeSplittedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var expensesBobjCount = _expensesToBeSplitted.Count;

            if (expensesBobjCount < 1)
            {
                IsSplitButtonEnabled = false;
                return;
            }

            //split button works if split is done for more than person 
            IsSplitButtonEnabled = true;

        }

        public void SplitButtonOnClick()
        {
            var expenseNote = ExpenseNote != null ? ExpenseNote.Trim() : string.Empty;
            var dateOfExpense = ExpenditureDate.DateTime;
            var expenseDescription = ExpenseDescription?.Trim() ?? string.Empty;

            var splittingType = SelectedSplitPreferenceIndex;// 0 if equal split or >0 for unequal split

            var ctk = new CancellationTokenSource().Token;

            var splitExpenseRequestObj = new SplitExpenseRequestObj(expenseDescription, Store.CurreUserBobj, _expensesToBeSplitted, expenseNote, dateOfExpense, _equalSplitAmount, splittingType, ctk, new SplitExpenseVmPresenterCallBack(this));

            var splitExpenseUseCaseObj = InstanceBuilder.CreateInstance<SplitExpenses>(splitExpenseRequestObj);

            splitExpenseUseCaseObj.Execute();
        }

        private void ResetPage()//resets UserControl to initial stage
        {
            SplittingUsersName = string.Empty;
            _isInnerInvokationOfTextChanged = true;
            ExpenseNote = string.Empty;
            SelectedGroupIndex = 0;
            ExpenseDescription = string.Empty;
            SingleUserExpenseShareAmount = string.Empty;
            ExpenditureDate = new DateTimeOffset(DateTime.Today);
            SelectedSplitPreferenceIndex = 0;

        }



        #endregion


        public string GetUserCurrencyPreference()
        {
            var preference = User.CurrencyPreference;
            return preference.ToString().ToUpper();
        }


        public SplitExpenseViewModel(ISplitExpenseView view)
        {
            View = view;
            User = new UserVobj(Store.CurreUserBobj);
            _expensesToBeSplitted.CollectionChanged += ExpensesToBeSplittedOnCollectionChanged;

        }

        private async void OnUserValueChanged(string property)
        {
            await UiService.RunOnUiThread(
                () =>
                {
                    BindingUpdateInvoked?.Invoke();
                }, View.Dispatcher).ConfigureAwait(false);
        }

        /// <summary>
        /// Updating expense objects Currency Converter if the currency preference Changes 
        /// </summary>
        public event Action BindingUpdateInvoked;

        class SplitExpenseVmPresenterCallBack : IPresenterCallBack<UserSuggestionResponseObject>, IPresenterCallBack<SplitExpenseResponseObj>, IPresenterCallBack<GetUserGroupResponse>
        {
            private readonly SplitExpenseViewModel _viewModel;
            public SplitExpenseVmPresenterCallBack(SplitExpenseViewModel viewModel)
            {
                _viewModel = viewModel;
            }
            public void OnSuccess(UserSuggestionResponseObject result)
            {
                _viewModel.InvokeOnUserSuggestionReceived(result);
            }
            public void OnSuccess(SplitExpenseResponseObj result)
            {
                _viewModel.InvokeOnSplitExpenseCompleted(result);
            }
            public async void OnSuccess(GetUserGroupResponse result)
            {
                await UiService.RunOnUiThread(() =>
                {
                    _viewModel.UserParticipatingGroup?.AddRange(result.UserParticipatingGroups);

                }, _viewModel.View.Dispatcher).ConfigureAwait(false);
            }


            #region ErrorHAndlingRegion

            public void OnError(SplittrException ex)
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

            #endregion


        }
    }

}
