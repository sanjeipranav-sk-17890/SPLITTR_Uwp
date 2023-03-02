using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase.GetGroupDetails;

public class GroupDetailById : UseCaseBase<GroupDetailByIdResponse>
{
    private readonly GroupDetailByIdRequest _requestObj;
    private readonly IGroupDetailManager _groupDataManager;

    public GroupDetailById(GroupDetailByIdRequest requestObj) : base(requestObj.PresenterCallBack, requestObj.Cts)
    {
        _requestObj = requestObj;
        _groupDataManager = SplittrDependencyService.GetInstance<IGroupDetailManager>();
    }

    public override bool GetIfAvailableFromCache()
    {
        if (!_groupDataManager.IsCacheAvailable(_requestObj.GroupUniqueId))
        {
            return false;
        }
        _groupDataManager.FetchGroupByGroupId(_requestObj.GroupUniqueId,_requestObj.CurrentUser, new GroupDetailUsCallBack(this));
        return true;
    }

    protected override void Action()
    {
        _groupDataManager.FetchGroupByGroupId(_requestObj.GroupUniqueId,_requestObj.CurrentUser, new GroupDetailUsCallBack(this));
    }

    private class GroupDetailUsCallBack : IUseCaseCallBack<GroupDetailByIdResponse>
    {
        private readonly GroupDetailById _useCase;

        public GroupDetailUsCallBack(GroupDetailById useCase)
        {
            _useCase = useCase;
        }

        public void OnSuccess(GroupDetailByIdResponse responseObj)
        {
            _useCase.PresenterCallBack?.OnSuccess(responseObj);
        }
        public void OnError(SplittrException error)
        {
            _useCase?.PresenterCallBack?.OnError(error);
        }
    }

}