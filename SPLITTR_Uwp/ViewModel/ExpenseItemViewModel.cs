using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel;

namespace SPLITTR_Uwp.ViewModel
{
  
    internal class ExpenseItemViewModel : ObservableObject
    {

        #region NotifiablePRoperties

        private ExpenseViewModel _expenseVObj;
        private Group _groupObject;
        private string _expenseTotalAmount;
        private string _groupName;
        private bool _isGroupButtonVisible;
        private string _splitOwnerTitle;
        private bool _owingAmountTextBlockVisibility;
        private string _owingSplitAmount;
        private string _owingSplitTitle;
        private Brush _owingExpenseForeground;




        public bool IsGroupButtonVisible
        {
            get => _isGroupButtonVisible;
            set => SetProperty(ref _isGroupButtonVisible, value);
        }


        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);

        }

        public Group GroupObject
        {
            get => _groupObject;
            set => SetProperty(ref _groupObject, value);
        }

        public string ExpenseTotalAmount
        {
            get => _expenseTotalAmount;
            set => SetProperty(ref _expenseTotalAmount, value);
        }

        public string SplitOwnerTitle
        {
            get => _splitOwnerTitle;
            set => SetProperty(ref _splitOwnerTitle, value);
        }

        public bool OwingAmountTextBlockVisibility
        {
            get => _owingAmountTextBlockVisibility;
            set => SetProperty(ref _owingAmountTextBlockVisibility, value);
        }

        public string OwingSplitAmount
        {
            get => _owingSplitAmount;
            set => SetProperty(ref _owingSplitAmount, value);
        }


        public string OwingSplitTitle
        {
            get => _owingSplitTitle;
            set => SetProperty(ref _owingSplitTitle, value);
        }


        public Brush OwingExpenseForeground
        {
            get => _owingExpenseForeground;
            set => SetProperty(ref _owingExpenseForeground, value);
        }
        #endregion

        #region ViewDataCalCulationgMethods

        

       

        private Brush SetOwingExpenseForeGround()
        {

            if (_expenseVObj is not null && IsCurrentUser(_expenseVObj.SplitRaisedOwner))
            {
                return new SolidColorBrush(Windows.UI.Colors.DarkSeaGreen);
            }
            return new SolidColorBrush(Windows.UI.Colors.DarkOrange);
        }

        private string GetOwingExpenseAmountTitle()
        {
            if (_expenseVObj is null)
            {
                return string.Empty;
            }
            if (IsCurrentUserRaisedExpense(_expenseVObj))
            {
                return "You borrow Nothing";
            }
            return IsCurrentUser(_expenseVObj.SplitRaisedOwner) ? $"You lent {_expenseVObj.CorrespondingUserObj.UserName}" : $"{_expenseVObj.SplitRaisedOwner.UserName} lent you";
        }

        private bool IsCurrentUserRaisedExpense(ExpenseViewModel expenseVObj)
        {
            return expenseVObj.SplitRaisedOwner.Equals(expenseVObj.CorrespondingUserObj);
        }


        private void SetExpenseOwnerTitle()
        {
            if (_expenseVObj is null)
            {
                SplitOwnerTitle = string.Empty;
                return;
            }
            if (IsCurrentUser(_expenseVObj.SplitRaisedOwner))
            {
                SplitOwnerTitle = "You Paid";
                return;
            }
            SplitOwnerTitle = _expenseVObj.SplitRaisedOwner.UserName + " Paid";
        }

        private bool IsCurrentUser(User user)
        {
            return Store.CurreUserBobj.Equals(user);
        }

        private void CallGroupNameByGroupIdUseCase(string groupUniqueId)
        {
            if (groupUniqueId is null)
            {
                GroupName = string.Empty;
                return;
            }

            var getGroupDetail = new GroupDetailByIdRequest(groupUniqueId,
                CancellationToken.None,
                new ExpenseItemVmPresenterCallBack(this),
                Store.CurreUserBobj);

            var getGroupDetailUseCaseObj = InstanceBuilder.CreateInstance<GroupDetailById>(getGroupDetail);

            getGroupDetailUseCaseObj.Execute();
            
        }

        private void SetViewDataBasesOnExpense()
        {

            //Changing visibility for Group Name Indicator
            IsGroupButtonVisible = _expenseVObj?.GroupUniqueId is not null;

            OwingAmountTextBlockVisibility = !IsCurrentUserRaisedExpense(_expenseVObj);

            OwingSplitAmount = _expenseVObj is null ? string.Empty : FormatExpenseAmountWithSymbol(_expenseVObj.StrExpenseAmount);

            OwingSplitTitle = GetOwingExpenseAmountTitle();

            OwingExpenseForeground = SetOwingExpenseForeGround();

            SetExpenseOwnerTitle();
        }

        private string FormatExpenseAmountWithSymbol(double expenseAmount)
        {
            //if expense amount is more than 7 digits then trimming it to 7 digits and adding Currency Symbol
            if (expenseAmount.ToString(CultureInfo.InvariantCulture).Length > 7)
            {
                return expenseAmount.ExpenseSymbol(Store.CurreUserBobj) + expenseAmount.ToString().Substring(0, 7);
            }
            return expenseAmount.ExpenseAmount(Store.CurreUserBobj);
        }

        #endregion

        public void ExpenseObjLoaded(ExpenseViewModel expenseObj)
        {

            if (expenseObj is null)
            {
                return;
            }
            _expenseVObj = expenseObj;

            //Subscribing For Currency Preference Changed Notification
            SplittrNotification.CurrencyPreferenceChanged += SplittrNotification_CurrencyPreferenceChanged;

            SetViewDataBasesOnExpense();

            CallGroupNameByGroupIdUseCase(expenseObj?.GroupUniqueId);

            CallRelatedExpenseUseCaseCall();
        }

      

        private async void SplittrNotification_CurrencyPreferenceChanged(CurrencyPreferenceChangedEventArgs obj)
        {
            //Recalculating Expense Total Based on new Currency Preference
            CallRelatedExpenseUseCaseCall();

            await UiService.RunOnUiThread((() =>
            {
                //Reassign Owing Amount Based On New Index
                OwingSplitAmount = _expenseVObj is null ? string.Empty : FormatExpenseAmountWithSymbol(_expenseVObj.StrExpenseAmount);

            })).ConfigureAwait(false);
        }

        public void ViewDisposed()
        {
            SplittrNotification.CurrencyPreferenceChanged -= SplittrNotification_CurrencyPreferenceChanged;
        }

        private void CallRelatedExpenseUseCaseCall()
        {
            var cts = new CancellationTokenSource();

             var relatedExpenseReqObj = new RelatedExpenseRequestObj(_expenseVObj, Store.CurreUserBobj, cts.Token, new ExpenseItemVmPresenterCallBack(this));

             var relatedExpenseUseCase = InstanceBuilder.CreateInstance<RelatedExpense>(relatedExpenseReqObj);

             relatedExpenseUseCase.Execute();
        }



        private async void OnRelatedExpensesRecievedSuccess(RelatedExpenseResponseObj result)
        {
            var totalAmount = result.RelatedExpenses.Sum(expense => expense.StrExpenseAmount);
            totalAmount += _expenseVObj.StrExpenseAmount;

            var formatedExpenseAmount = FormatExpenseAmountWithSymbol(totalAmount);

            await UiService.RunOnUiThread(() =>
            {
                ExpenseTotalAmount = formatedExpenseAmount;
            }).ConfigureAwait(false);
        }


        private class ExpenseItemVmPresenterCallBack : IPresenterCallBack<RelatedExpenseResponseObj>,IPresenterCallBack<GroupDetailByIdResponse>
        {
            private readonly ExpenseItemViewModel _viewModel;
            public ExpenseItemVmPresenterCallBack(ExpenseItemViewModel viewModel)
            {
                _viewModel = viewModel;

            }
            public void OnSuccess(RelatedExpenseResponseObj result)
            {
                _viewModel.OnRelatedExpensesRecievedSuccess(result);
            }
            public async void OnSuccess(GroupDetailByIdResponse result)
            {
               await UiService.RunOnUiThread((() =>
               {

                   _viewModel.GroupName = result?.RequestedGroup?.GroupName;
                   _viewModel.GroupObject = result?.RequestedGroup;

               })).ConfigureAwait(false);
            }
            public void OnError(SplittrException ex)
            {
                if (ex.InnerException is SqlException)
                {
                    //Code to Notify sql db access failed
                }
            }
        }
    }
}
