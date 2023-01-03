using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.DataRepository;

namespace SPLITTR_Uwp.ViewModel
{
    internal class ExpenseItemViewModel
    {
        private readonly DataStore _store;

        public ExpenseItemViewModel(DataStore store )
        {
            _store = store;

        }

        public string GetGroupNameByGroupId(string groupUniqueId)
        {
            if (groupUniqueId is null)
            {
                return String.Empty;
            }

            string groupName = String.Empty;
            foreach (var group in _store.UserBobj.Groups)
            {
                if (group.GroupUniqueId.Equals(groupUniqueId))
                {
                    groupName = group.GroupName;
                    break;
                }
            }
            return groupName;

        }

    }
}
