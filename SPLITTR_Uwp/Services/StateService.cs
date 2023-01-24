using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Services
{
    internal class StateService : IStateService
    {
        private static Action<UserBobj>  _onLogoutAction;

        public static event Action<UserBobj> OnUserLoggedOut
        {
            add => _onLogoutAction += value;
            remove => _onLogoutAction -= value;
        }


        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public void InvokeCurrentUserLoggedOut(UserBobj user)
        {
            _onLogoutAction?.Invoke(user);
        }
    }
    internal interface IStateService
    {
        void InvokeCurrentUserLoggedOut(UserBobj user);
    }
}
