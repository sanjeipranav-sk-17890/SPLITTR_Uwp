using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Contracts;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.ViewModel;

namespace SPLITTR_Uwp.ViewModel
{
    internal class ExpenseItemVmPresenterCallBack :IPresenterCallBack<RelatedExpenseResponseObj>
    {
        private readonly ExpenseItemViewModel _viewModel;
        public ExpenseItemVmPresenterCallBack(ExpenseItemViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public void OnSuccess(RelatedExpenseResponseObj result)
        {
           _viewModel.OnUseCaseSuccess(result);
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
    internal class ExpenseItemViewModel : ObservableObject,IViewModel
    {
       

        private ExpenseViewModel _expenseVObj;
        private Group _groupObject;
        private string _expenseTotalAmount;
        private string _splitOwnerTitle;


        public bool IsGroupButtonVisible
        {
            get => _expenseVObj?.GroupUniqueId is not null;
        }
        public string GroupName
        {
            get => GetGroupNameByGroupId(_expenseVObj?.GroupUniqueId);
            
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
            get => GetExpenseOwnerTitle();

        }

        public bool OwingAmountTextBlockVisibility
        {
            get => _expenseVObj != null && !IsCurrentUserRaisedExpense(_expenseVObj);
        }

        public string OwingSplitAmount
        {
            get => _expenseVObj is null ? string.Empty : FormatExpenseAmountWithSymbol(_expenseVObj.StrExpenseAmount);
        }

      

        public string OwingSplitTitle
        {
            get=> GetOwingExpenseAmountTitle();
        }

        public Brush OwingExpenseForeground
        {
            get
            {
                if (_expenseVObj is not null && IsCurrentUser(_expenseVObj.SplitRaisedOwner))
                {
                    return new SolidColorBrush(Windows.UI.Colors.DarkSeaGreen);
                }
                return new SolidColorBrush(Windows.UI.Colors.DarkOrange);

            }
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


        private string GetExpenseOwnerTitle()
        {
            if (_expenseVObj is null)
            {
                return string.Empty;
            }
            if (IsCurrentUser(_expenseVObj.SplitRaisedOwner))
            {
                return "You Paid";
            }
            return _expenseVObj.SplitRaisedOwner.UserName + " Paid";

        }

        private bool IsCurrentUser(User user)
        {
            return Store.CurreUserBobj.Equals(user);
        }

        private string GetGroupNameByGroupId(string groupUniqueId)
        {
            if (groupUniqueId is null)
            {
                return string.Empty;
            }

            var groupName = string.Empty;
            foreach (var group in Store.CurreUserBobj.Groups)
            {
                if (!group.GroupUniqueId.Equals(groupUniqueId))
                {
                    continue;
                }
                groupName = group.GroupName;
                GroupObject = group;
                break;
            }
            return groupName;

        }

        private string FormatGroupName(string groupUniqueId)
        {
            var groupName = GetGroupNameByGroupId(groupUniqueId);
            if (string.IsNullOrEmpty(groupName))
            {
                return string.Empty;
            }
            if (groupName.Length > 10)
            {
               return groupName.Substring(0,10) + "....";
            }
            return groupName;
        }


        public string FormatExpenseTitle(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return String.Empty;
            }
            if (expenseObj.GroupUniqueId is not null)
            {
                
                return GetGroupNameByGroupId(expenseObj.GroupUniqueId);
            }
            //If Current user is Owner Showing the Name as You instead of Name
            if (expenseObj.SplitRaisedOwner.Equals(Store.CurreUserBobj))
            {
                return "You";
            }
            return expenseObj.SplitRaisedOwner?.UserName ?? string.Empty;
        }

        public string FormatExpenseAmount(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return string.Empty;
            }
            
            var expenseAmount = expenseObj.ExpenseAmount.ToString();
            if (expenseAmount.Length > 7)
            {
                expenseAmount = expenseAmount.Substring(0, 7);
            }
            if (expenseObj.SplitRaisedOwner.Equals(Store.CurreUserBobj))
            {
                return "+ " + expenseAmount;
            }
            return "- " + expenseAmount;

        }

        public void ExpenseObjLoaded(ExpenseViewModel expenseObj)
        {

            if (expenseObj is null)
            {
                return;
            }
            _expenseVObj = expenseObj;

            _expenseVObj.PropertyChanged += _expenseVObj_PropertyChanged;
            BindingUpdateInvoked?.Invoke();

            CallRelatedExpenseUseCaseCall();
        }

        private void CallRelatedExpenseUseCaseCall()
        {
            var cts = new CancellationTokenSource();

             var relatedExpenseReqObj = new RelatedExpenseRequestObj(_expenseVObj, Store.CurreUserBobj, cts.Token, new ExpenseItemVmPresenterCallBack(this));

             var relatedExpenseUseCase = InstanceBuilder.CreateInstance<RelatedExpense>(relatedExpenseReqObj);

             relatedExpenseUseCase.Execute();
        }

        private void _expenseVObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            BindingUpdateInvoked?.Invoke();
            if (e.PropertyName.Equals(nameof(ExpenseBobj.CurrencyConverter)))
            {
                CallRelatedExpenseUseCaseCall();
            }
        }


        private string FormatExpenseAmountWithSymbol(double expenseAmount)
        {
            //if expense amount is more than 7 digits then trimming it to 7 digits and adding Currency Symbol
            if (expenseAmount.ToString().Length > 7)
            {
                return expenseAmount.ExpenseSymbol(Store.CurreUserBobj) + expenseAmount.ToString().Substring(0, 7);
            }
            return expenseAmount.ExpenseAmount(Store.CurreUserBobj);
        }

        public event Action BindingUpdateInvoked;

        public async void OnUseCaseSuccess(RelatedExpenseResponseObj result)
        {
            var totalAmount = result.RelatedExpenses.Sum(expense => expense.StrExpenseAmount);
            totalAmount += _expenseVObj.StrExpenseAmount;

            var formatedExpenseAmount = FormatExpenseAmountWithSymbol(totalAmount);

            await UiService.RunOnUiThread(() =>
            {
                ExpenseTotalAmount = formatedExpenseAmount;
            });
        }
       
}
