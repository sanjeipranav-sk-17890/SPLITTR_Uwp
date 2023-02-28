using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.CreateGroup;

public class GroupCreation : UseCaseBase<GroupCreationResponseObj>
{
    private readonly GroupCreationRequestObj _requestObj;
    private IGroupCreationDataManager _dataManager;

    public GroupCreation(GroupCreationRequestObj requestObj) : base(requestObj.PresenterCallBack, requestObj.Cts)
    {
        _requestObj = requestObj;
   
        _dataManager =SplittrDependencyService.GetInstance<IGroupCreationDataManager>();
    }
   protected override void Action()
    {
        _dataManager.CreateSplittrGroup(_requestObj.GroupParticiPants, _requestObj.CurrentUser, _requestObj.GroupName,new UseCaseCallBackBase<GroupCreationResponseObj>(this));
    }
}
