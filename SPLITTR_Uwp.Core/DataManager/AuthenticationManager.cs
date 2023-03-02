using System;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.LoginUser;
using SPLITTR_Uwp.Core.UseCase.SignUpUser;
using SQLite;

namespace SPLITTR_Uwp.Core.DataManager;

public class AuthenticationManager : IAuthenticationManager,ISignUpDataManager
{
    private readonly IUserDbHandler _userDbHandler;
    private readonly IUserDataManager _dataManager;

    public AuthenticationManager(IUserDbHandler dbHandler, IUserDataManager dataManager)
    {
        _userDbHandler = dbHandler;
        _dataManager = dataManager;

    }

    public async Task<bool> IsUserAlreadyExist(string emailId)
    {
        var userObj = await _userDbHandler.SelectUserObjByEmailId(emailId).ConfigureAwait(false);

        return userObj is not null;
    }


    public async void Authenticate(string emailId, IUseCaseCallBack<LoginResponseObj> callBack)
    {

        try
        {
            var isExistingUser = await IsUserAlreadyExist(emailId).ConfigureAwait(false);

            if (!isExistingUser)
            {
                callBack?.OnSuccess(new LoginResponseObj(null, false));
                return;
            }
            var currentUser = await _dataManager.FetchCurrentUserDetails(emailId).ConfigureAwait(false);

            callBack?.OnSuccess(new LoginResponseObj(currentUser, true));

        }
        catch (ArgumentException ex)
        {
            callBack?.OnError(new SplittrException.SplittrException(ex, ex.Message));
        }
        catch (Exception ex)
        {
            callBack?.OnError(new SplittrException.SplittrException(ex, ex.Message));
        }

    }
    public async void CreateNewUser(string userName, string emailId, int currencyPreference, IUseCaseCallBack<SignUpUserResponseObj> callBack)
    {
        try
        {
            var isExistingUser = await IsUserAlreadyExist(emailId).ConfigureAwait(false);

            if (isExistingUser)
            {
                throw new UserAlreadyExistException();
            }
            var newUser = new User(emailId, userName, 0.0, currencyPreference, 0.0, 0.0);
            await _userDbHandler.InsertUserObjAsync(newUser).ConfigureAwait(false);

            callBack.OnSuccess(new SignUpUserResponseObj(newUser));
        }
        catch (SQLiteException e)
        {
            callBack.OnError(new SplittrException.SplittrException(e, "Db Insertion error"));
        }
        catch (Exception ex)
        {
            callBack?.OnError(new SplittrException.SplittrException(ex,ex.Message));
        }
    }
}
