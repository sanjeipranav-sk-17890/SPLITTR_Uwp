using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.SignUpUser;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface ISignUpDataManager
{
    void CreateNewUser(string userName, string emailId,int currencyPreference,IUseCaseCallBack<SignUpUserResponseObj> callBack);
}
