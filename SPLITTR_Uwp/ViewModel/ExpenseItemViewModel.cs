using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.ChangeExpenseCategory;
using SPLITTR_Uwp.Core.UseCase.GetCategoryById;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Vobj;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel;

internal class ExpenseItemViewModel : ObservableObject
{

    #region NotifiablePRoperties

    private ExpenseVobj _expenseVObj;
    private Group _groupObject;
    private string _expenseTotalAmount;
    private string _groupName;
    private bool _isGroupButtonVisible;
    private string _splitOwnerTitle;
    private bool _owingAmountTextBlockVisibility;
    private string _owingSplitAmount;
    private string _owingSplitTitle;
    private Brush _owingExpenseForeground;
    private bool _isCategoryChangeAllowed;
    private string _categoryName;

    public bool IsCategoryChangeAllowed
    {
        get => _isCategoryChangeAllowed;
        set => SetProperty(ref _isCategoryChangeAllowed, value);
    }

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

    public string CategoryName
    {
        get => _categoryName;
        set => SetProperty(ref _categoryName, value);
    }

    #endregion

    #region ViewDataCalCulationgMethods

        

       

    private Brush SetOwingExpenseForeGround()
    {

        if (_expenseVObj is not null && IsCurrentUser(_expenseVObj.SplitRaisedOwner))
        {
            return new SolidColorBrush(Colors.DarkSeaGreen);
        }
        return new SolidColorBrush(Colors.DarkOrange);
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

    private bool IsCurrentUserRaisedExpense(ExpenseVobj expenseVObj)
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
        return Store.CurrentUserBobj.Equals(user);
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
            Store.CurrentUserBobj);

        var getGroupDetailUseCaseObj = InstanceBuilder.CreateInstance<GroupDetailById>(getGroupDetail);

        getGroupDetailUseCaseObj.Execute();
            
    }

    private void SetViewDataBasesdOnExpense()
    {

        //Changing visibility for Group Name Indicator
        IsGroupButtonVisible = _expenseVObj?.GroupUniqueId is not null;

        //Allowing Category Change Only if Current User is OWer of that Expense
        IsCategoryChangeAllowed = _expenseVObj?.SplitRaisedOwner.Equals(Store.CurrentUserBobj) ?? false;

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
            return expenseAmount.ExpenseSymbol(Store.CurrentUserBobj) + expenseAmount.ToString().Substring(0, 7);
        }
        return expenseAmount.ExpenseAmount(Store.CurrentUserBobj);
    }

    #endregion

    #region UseCaseCallRegion

    public void ChangeExpenseCategory(ExpenseCategory expenseCategory)
    {
        //No UseCase Call If Same Category Selected Again  
        if (expenseCategory == null || IsCurrentCategoryAndPreviousCategorySame(expenseCategory))
        {
            return;
        }
        var changeCategoryReq = new ChangeExpenseCategoryReq(CancellationToken.None, new ExpenseItemVmPresenterCallBack(this), _expenseVObj, expenseCategory, Store.CurrentUserBobj);

        var changeCategoryUseCase = InstanceBuilder.CreateInstance<ChangeExpenseCategory>(changeCategoryReq);

        changeCategoryUseCase.Execute();

        bool IsCurrentCategoryAndPreviousCategorySame(ExpenseCategory newCategory)
        {
            return _expenseVObj.CategoryId == newCategory.Id;
        }
    }

    private void CallRelatedExpenseUseCaseCall()
    {
        var cts = new CancellationTokenSource();

        var relatedExpenseReqObj = new RelatedExpenseRequestObj(_expenseVObj, Store.CurrentUserBobj, cts.Token, new ExpenseItemVmPresenterCallBack(this));

        var relatedExpenseUseCase = InstanceBuilder.CreateInstance<RelatedExpense>(relatedExpenseReqObj);

        relatedExpenseUseCase.Execute();
    }
    #endregion

    public void ExpenseObjLoaded(ExpenseVobj expenseObj)
    {

        if (expenseObj is null)
        {
            return;
        }
        _expenseVObj = expenseObj;

        //Subscribing For Currency Preference Changed Notification
        SplittrNotification.CurrencyPreferenceChanged += SplittrNotification_CurrencyPreferenceChanged;

        SetViewDataBasesdOnExpense();

        CallGroupNameByGroupIdUseCase(expenseObj?.GroupUniqueId);

        CallRelatedExpenseUseCaseCall();

    }

    private async void SplittrNotification_CurrencyPreferenceChanged(CurrencyPreferenceChangedEventArgs obj)
    {
        //Recalculating Expense Total Based on new Currency Preference
        CallRelatedExpenseUseCaseCall();

        await RunOnUiThread(() =>
        {
            //Reassign Owing Amount Based On New Index
            OwingSplitAmount = _expenseVObj is null ? string.Empty : FormatExpenseAmountWithSymbol(_expenseVObj.StrExpenseAmount);

        }).ConfigureAwait(false);
    }

    public void ViewDisposed()
    {
        SplittrNotification.CurrencyPreferenceChanged -= SplittrNotification_CurrencyPreferenceChanged;
    }

    private async void OnRelatedExpensesRecievedSuccess(RelatedExpenseResponseObj result)
    {
        var totalAmount = result.RelatedExpenses.Sum(expense => expense.StrExpenseAmount);
        totalAmount += _expenseVObj.StrExpenseAmount;

        var formatedExpenseAmount = FormatExpenseAmountWithSymbol(totalAmount);

        await RunOnUiThread(() =>
        {
            ExpenseTotalAmount = formatedExpenseAmount;
        }).ConfigureAwait(false);
    }


    private class ExpenseItemVmPresenterCallBack : IPresenterCallBack<RelatedExpenseResponseObj>,IPresenterCallBack<GroupDetailByIdResponse>,IPresenterCallBack<ChangeExpenseCategoryResponse>
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
            await RunOnUiThread(() =>
            {

                _viewModel.GroupName = result?.RequestedGroup?.GroupName;
                _viewModel.GroupObject = result?.RequestedGroup;

            }).ConfigureAwait(false);
        }
        public async void OnSuccess(ChangeExpenseCategoryResponse result)
        {
            if (result is { ChangedExpenseCategory: { } })
            {
               await RunOnUiThread((() =>
                {
                    _viewModel._expenseVObj.IconSource = result.ChangedExpenseCategory.Icon;
                    _viewModel._expenseVObj.CategoryName = result.ChangedExpenseCategory.Name;
                    _viewModel.CategoryName = result.ChangedExpenseCategory.Name;
                }));
                   
            }
        }
        public void OnError(SplittrException ex)
        {
            if (ex.InnerException is SqlException)
            {
                //Code to Notify sql db access failed
            }
            ExceptionHandlerService.HandleException(ex);
        }
    }
   
}