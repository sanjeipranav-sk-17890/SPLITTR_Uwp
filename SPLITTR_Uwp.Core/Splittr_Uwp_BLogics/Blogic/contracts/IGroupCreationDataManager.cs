using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CreateGroup;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public interface IGroupCreationDataManager 
    {
        /// <summary>
        /// Creates Group for user and change notification will bw raised 
        /// </summary>
        /// <param name="participants"></param>
        /// <param name="currentUser"></param>
        /// <param name="groupName"></param>
        /// <param name="callBack"></param>
        /// <exception cref="System.ArgumentException">number of particpants must be greater than 1 to form a group </exception>
        public void CreateSplittrGroup(IEnumerable<User> participants, UserBobj currentUser, string groupName,IUseCaseCallBack<GroupCreationResponseObj> callBack);
    }
}
