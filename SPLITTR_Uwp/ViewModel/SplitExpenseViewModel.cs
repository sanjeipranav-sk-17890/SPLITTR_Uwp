using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using System.ComponentModel;
using System.Net;
using Windows.UI.Core;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;

namespace SPLITTR_Uwp.ViewModel
{
    public class SplitExpenseViewModel : ObservableObject
    {
        private readonly IUserUtility _userUtility;
        private readonly IExpenseUtility _expenseUtility;
        private readonly DataStore _store;
        

        public UserViewModel User { get;}

        /// <summary>
        /// Contains Expenses obj which will be processed and inserted into db
        /// </summary>
        public readonly ObservableCollection<ExpenseBobj> ExpensesToBeSplitted = new ObservableCollection<ExpenseBobj>();




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
            if (_selectedUser == _dummyUser|| _selectedUser == null && !_usersToBeSplitted.Any())
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
                _isUserSuggestionListOpen= value;
                 OnPropertyChanged(nameof(IsUserSuggestionListOpen));
            }
        }

       

        private User _selectedUser;
        private readonly IList<User> _usersToBeSplitted = new List<User>();
        private int _selectedGroupIndex = 0;
        private string _selectedGroupName;
        private bool _isNameTextBoxReadOnly;
        private bool _isInnerInvokationOfTextChanged = false;
        

        public async void TextBoxTextChanged()
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
            await _userUtility.GetUsersSuggestionAsync(SplittingUsersName.Trim().ToLower(), async suggestions =>
            {//remainCode will be run if data fetching from use case is finished
               await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        foreach (var user in suggestions)
                        {
                            UsersList.Add(user);
                        }
                        if (!UsersList.Any())
                        {
                            UsersList.Add(_dummyUser);
                        }

                        IsUserSuggestionListOpen = true;
                       
                    });
               

            });

            
        }



        private User _dummyUser = new User()
        {
            UserName = "No Results Found"
        };

        private int _selectedUserIndex= -1;


        // public void ListViewOnSelected(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        public void ListViewOnSelected()
        {
            if(SelectedUserIndex == -1) return;

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



        public ObservableCollection<GroupBobj> UserParticipatingGroup
        {
            get
            {
                return GetUserParticipatingGroup();
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
            
            var observableGroupObj = new ObservableCollection<GroupBobj>();
            observableGroupObj.Add(_dummyGroup);
            foreach (var grp in _store.UserBobj.Groups)
            {
                observableGroupObj.Add(grp);
            }
            return observableGroupObj;

        }


        public void GroupItemSelectionChanged()
        {
            SelectedGroupName = UserParticipatingGroup[SelectedGroupIndex].GroupName;

            //if dummy is selected Clearing Users to be Splitted list so other UI Usecases work Properly

            SelectedGroupName = SelectedGroupName;

            if (SelectedGroupIndex == 0)
            {
                _isInnerInvokationOfTextChanged=true;
                SplittingUsersName = String.Empty;
                IsNameTextBoxReadOnly = false;
                return;
            }

            IsNameTextBoxReadOnly = true;
            _selectedUser = null;
            var splitingGroup = UserParticipatingGroup[SelectedGroupIndex];
            SplittingUsersName = String.Empty;
            _usersToBeSplitted.Clear();
            foreach (var user in splitingGroup.GroupParticipants)
            {
                _isInnerInvokationOfTextChanged = true;

                _usersToBeSplitted.Add(user);

                //ingoring current users name in the GROUP MEMBERS SPLITTING NAME
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
        private int _selectedSplitPreferenceIndex=0;
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
            get=>DateTimeOffset.Now;
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
            ExpensesToBeSplitted.Clear();

            //cheching whether it is dummy groupobj
            if (_selectedGroupIndex == 0)
            {
                if (_selectedUser != null)//Individual Split
                {

                    ExpensesToBeSplitted.Add(GenerateExpenseViewModel(_store.UserBobj,null));//current User
                    ExpensesToBeSplitted.Add(GenerateExpenseViewModel(_selectedUser,null));//Spiltting user
                   
                }
            }
            else//Group Split 
            {
                
                var group = UserParticipatingGroup[_selectedGroupIndex];

                foreach (var participant in group.GroupParticipants)
                {
                   ExpensesToBeSplitted.Add(GenerateExpenseViewModel(participant,group.GroupUniqueId)); 
                }
            }

        }

        private ExpenseBobj GenerateExpenseViewModel(User user,string groupUid)
        {
            return new ExpenseBobj(_store.UserBobj.CurrencyConverter)
            {
                RequestedOwner = _store.UserBobj.EmailId,
                UserEmailId = user.EmailId,
                UserDetails = user,
                StrExpenseAmount = 0.0,
                GroupUniqueId = groupUid
            };

        }

        #endregion

        #region ExpensesSplitFunctionality region

        private bool _isSplitButtonEnabled;
        private string _expenseNote;


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

        //event raises when collection item changes
        private void ExpensesToBeSplittedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var expensesBobjCount = ExpensesToBeSplitted.Count;

            if (expensesBobjCount < 1 )
            {
                IsSplitButtonEnabled = false;
                return;
            }

            //split button works if split is done for more than person 
            IsSplitButtonEnabled = true;

        }

        public async void SplitButtonOnClick()
        {
            var expenseNote = ExpenseNote.Trim();
            var dateOfExpense = ExpenditureDate.DateTime;

           
                var splittingType = SelectedSplitPreferenceIndex;// 0 if equal split or >0 for unequal split


                //on expense Splitting success Assigend Callback will be called
            _expenseUtility.PresenterCallBackOnSuccess += _expenseUtility_PresenterCallBackOnSuccess;
            
              //on Error Call back 
              if (_expenseUtility is IUseCase expenseUtility)
              {
                expenseUtility.OnError += ExpenseSplitting_OnError;
              }


            await _expenseUtility.SplitNewExpensesAsync(_store.UserBobj, ExpensesToBeSplitted, expenseNote, dateOfExpense, _equalSplitAmount, splittingType);

        }

        private void ExpenseSplitting_OnError(Exception arg1, string arg2)
        {
            
        }

        //if the splitting is successfull showing split completed text box and reset the page 
        private async void _expenseUtility_PresenterCallBackOnSuccess(EventArgs args)
        {
            
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        MessageDialog msg = new MessageDialog("Spliting SuccessFull","Expenses Splitted Successfully");

                        msg.Commands.Add(new UICommand("close"));
                        await msg.ShowAsync();

                        ResetPage();
                    });
                
            
        }
        private void ResetPage()//resets UserControl to initial stage
        {
            SplittingUsersName = String.Empty;
            _isInnerInvokationOfTextChanged = true;
            ExpenseNote = String.Empty;
            SelectedGroupIndex = 0;
            SingleUserExpenseShareAmount = String.Empty;
            ExpenditureDate = new DateTimeOffset(DateTime.Today);
            SelectedSplitPreferenceIndex = 0;

        }



        #endregion
        public string GetUserCurrencyPreference()
        {
          var preference=  (Currency)User.CurrencyPreference;
          return  preference.ToString().ToUpper();
        }

       
        public SplitExpenseViewModel(IUserUtility userUtility,IExpenseUtility expenseUtility,DataStore store)
        {
            _userUtility = userUtility;
            _expenseUtility = expenseUtility;
            _store = store;
            _store.UserBobj.ValueChanged += OnUserValueChanged;
            User = new UserViewModel(_store.UserBobj);
            ExpensesToBeSplitted.CollectionChanged += ExpensesToBeSplittedOnCollectionChanged;

        }
        

        public async void OnUserValueChanged()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    OnPropertyChanged(nameof(GetUserCurrencyPreference));
                });
        }



        
    }


}
