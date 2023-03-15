using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class UserBobjUpdatedEventArgs : SplittrEventArgs
{
    public UserBobj UpdatedUser { get; }

    public UserBobjUpdatedEventArgs(UserBobj updatedUser)
    {
        UpdatedUser = updatedUser;
    }
}
