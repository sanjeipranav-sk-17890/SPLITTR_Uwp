using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.Core.UseCase.GetUserExpenses;
using SQLite;

namespace SPLITTR_Uwp.Core.DataManager;

internal class ExpenseFetchDataManager : IRelatedExpenseDataManager,IExpenseFetchDataManager
{
    private readonly ICurrencyCalcFactory _currencyCalcFactory;
    private readonly IUserDataManager _userDataManager;
    private readonly IExpenseDbHandler _dbHandler;

    public ExpenseFetchDataManager(IExpenseDbHandler dbHandler, ICurrencyCalcFactory currencyCalcFactory,IUserDataManager userDataManager)
    {
        _currencyCalcFactory = currencyCalcFactory;
        _userDataManager = userDataManager;
        _dbHandler = dbHandler;

    }

    /// <exception cref="ArgumentException">Expense passed cannot be null</exception>
    /// <exception cref="ArgumentNullException"><paramref name="source">source</paramref>
    public async void GetRelatedExpenses(ExpenseBobj referenceExpense, UserBobj currentUser, IUseCaseCallBack<RelatedExpenseResponseObj> callBack)
    {
        var relatedExpenses = await GetRelatedExpenses(referenceExpense, currentUser).ConfigureAwait(false);

        var filteredExpense = relatedExpenses.Where(ex => !ex.ExpenseUniqueId.Equals(referenceExpense.ExpenseUniqueId));

        callBack?.OnSuccess(new RelatedExpenseResponseObj(filteredExpense));
    }

    public async Task<IEnumerable<ExpenseBobj>> GetRelatedExpenses(ExpenseBobj referenceExpense, UserBobj currentUser)
    {

        var key = referenceExpense.ParentExpenseId ?? referenceExpense.ExpenseUniqueId;

        var relatedExpenseList = await _dbHandler.SelectRelatedExpenses(key).ConfigureAwait(false);

        var relatedExpenses = await InitializeExpenseBobjs(relatedExpenseList, currentUser).ConfigureAwait(false);
        return relatedExpenses;
    }

    private async Task<IEnumerable<ExpenseBobj>> InitializeExpenseBobjs(IEnumerable<Expense> expenses, User currentUser)
    {
        //Fetching Current User's Currency Preference
        var userCurrencyPreference = (Currency)currentUser.CurrencyIndex;


        //getting ICurrencyConverter Based on Currency PReference
        ExpenseBobj.CurrencyConverter = _currencyCalcFactory.GetCurrencyCalculator(userCurrencyPreference);


        var outputList = new List<ExpenseBobj>();

        //For Each Expense Constructs ExpenseBobj in Worker Thread 
        var tasksOfExpense = expenses.Select(expense => Task.Run(() => ConstructExpenseBobj(expense)));

        var userExpenseBobjs = await Task.WhenAll(tasksOfExpense).ConfigureAwait(false);

        return userExpenseBobjs;


        //Local Function Which Builds ExpenseBobj 
        async Task<ExpenseBobj> ConstructExpenseBobj(Expense expense)
        {

            User respectiveUserObj = null;
            User requestedOwnerUserObj = null;

            if (expense.UserEmailId == currentUser.EmailId)
            {
                respectiveUserObj = currentUser;
            }
            if (expense.RequestedOwner == currentUser.EmailId)
            {
                requestedOwnerUserObj = currentUser;
            }

            requestedOwnerUserObj ??= await _userDataManager.FetchUserUsingMailId(expense.RequestedOwner).ConfigureAwait(false);

            respectiveUserObj ??= await _userDataManager.FetchUserUsingMailId(expense.UserEmailId).ConfigureAwait(false);

            return new ExpenseBobj(respectiveUserObj, requestedOwnerUserObj, expense: expense);
        }

    }

    public async void GetUserExpensesAsync(User user, IUseCaseCallBack<GetExpensesByIdResponse> callBack)
    {
        try
        {
            //fetching Expense entity obj from db  
            var userExpenses = await _dbHandler.SelectUserExpensesAsync(user.EmailId).ConfigureAwait(false);

            var convertedExpenseBobj = await InitializeExpenseBobjs(userExpenses, user).ConfigureAwait(false);

            callBack?.OnSuccess(new GetExpensesByIdResponse(convertedExpenseBobj));
        }
        catch (Exception e)
        {
            callBack?.OnError(new SplittrException(e));
        }
    }
}
