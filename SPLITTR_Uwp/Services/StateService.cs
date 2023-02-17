using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Views;

namespace SPLITTR_Uwp.Services
{
    public class StateService : IStateService
    {
        private static Action<UserBobj>  _onLogoutAction;

        public static event Action<UserBobj> OnUserLoggedOut
        {
            add => _onLogoutAction += value;
            remove => _onLogoutAction -= value;
        }


        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        private void InvokeCurrentUserLoggedOut(UserBobj user)
        {
            _onLogoutAction?.Invoke(user);
        }

        public bool RequestUserLogout()
        { 
            InvokeCurrentUserLoggedOut(Store.CurreUserBobj);

            //Clearing Reference to Current USer Cache
            Store.CurreUserBobj= null;

            NavigationService.Frame = null;

            NavigationService.Navigate(typeof(LoginPage),null,new SuppressNavigationTransitionInfo());
            

            return true;
        }
    }
    public interface IStateService
    {
        bool RequestUserLogout();
    }
}
