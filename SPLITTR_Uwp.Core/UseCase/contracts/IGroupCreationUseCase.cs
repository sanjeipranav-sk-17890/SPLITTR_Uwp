using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

namespace SPLITTR_Uwp.Core.UseCase.contracts
{
    
    public class GroupCreationResponseObj
    {
        public GroupCreationResponseObj(GroupBobj createdGroup)
        {
            CreatedGroup = createdGroup;
        }

        public GroupBobj CreatedGroup { get; set; } 
    }

    public interface IRequestObj<in T>
    {
        public CancellationToken Cts { get;}

        public IPresenterCallBack<T> PresenterCallBack { get; }
    }

    /// <summary>
    /// Group Creation Request Object
    /// </summary>
    public class GroupCreationRequestObj : IRequestObj<GroupCreationResponseObj>
    {
        public CancellationToken Cts { get; }

        public IPresenterCallBack<GroupCreationResponseObj> PresenterCallBack { get; }

        public IEnumerable<User> GroupParticiPants { get; set; }

        public UserBobj CurrentUser { get;}

        public string GroupName { get; }

        public GroupCreationRequestObj(CancellationToken cts,IPresenterCallBack<GroupCreationResponseObj> presenterCallBack, UserBobj currentUser, IEnumerable<User> groupParticiPants, string groupName)
        {
            Cts = cts;
            PresenterCallBack = presenterCallBack;
            CurrentUser = currentUser;
            GroupParticiPants = groupParticiPants;
            GroupName = groupName;

        }
    }

    public  interface IUseCaseCallBackBase<in T>
    {
        public void OnSuccess(T responseObj);
        public void OnError(SplittrException error);

    }

    public class GroupCreation :UseCaseBase<GroupCreationResponseObj> ,IUseCaseCallBackBase<GroupCreationResponseObj>
    {
        private readonly GroupCreationRequestObj _requestObj;
        private IGroupCreationDataManager _DataManager;

        public GroupCreation(GroupCreationRequestObj requestObj, IGroupCreationDataManager dataManager) : base(requestObj.PresenterCallBack, requestObj.Cts)
        {
            _requestObj = requestObj;
            // DataManager  Will Be Instantiated By Core Injector
            _DataManager = dataManager;
        }
        public override void Action()
        {
            _DataManager.CreateSplittrGroup(_requestObj.GroupParticiPants,_requestObj.CurrentUser,_requestObj.GroupName,this);
        }

        void IUseCaseCallBackBase<GroupCreationResponseObj>.OnSuccess(GroupCreationResponseObj responseObj)
        {
            PresenterCallBack.OnSuccess(responseObj);
        }
        void IUseCaseCallBackBase<GroupCreationResponseObj>.OnError(SplittrException error)
        {
            PresenterCallBack.OnError(error);
        }
    }

}
