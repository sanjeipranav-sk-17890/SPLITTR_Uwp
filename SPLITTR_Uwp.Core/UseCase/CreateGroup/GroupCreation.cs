using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

namespace SPLITTR_Uwp.Core.UseCase.CreateGroup;

public class GroupCreation : UseCaseBase<GroupCreationResponseObj>
{
    private readonly GroupCreationRequestObj _requestObj;
    private IGroupCreationDataManager _dataManager;

    public GroupCreation(GroupCreationRequestObj requestObj, IGroupCreationDataManager dataManager) : base(requestObj.PresenterCallBack, requestObj.Cts)
    {
        _requestObj = requestObj;
        // DataManager  Will Be Instantiated By Core Injector
        _dataManager = dataManager;
    }
    public override void Action()
    {
        _dataManager.CreateSplittrGroup(_requestObj.GroupParticiPants, _requestObj.CurrentUser, _requestObj.GroupName,new UseCaseCallBackBase<GroupCreationResponseObj>(this));
    }
}
