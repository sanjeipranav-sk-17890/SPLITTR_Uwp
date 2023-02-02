using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.UpdateUser;

public class UpdateUserResponseObj
{
    public UpdateUserResponseObj(UserBobj updatedUserBobj)
    {
        UpdatedUserBobj = updatedUserBobj;
    }

    public UserBobj UpdatedUserBobj { get;  }
}
