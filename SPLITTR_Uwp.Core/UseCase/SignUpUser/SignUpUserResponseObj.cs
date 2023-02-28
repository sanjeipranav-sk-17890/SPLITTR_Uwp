#nullable enable
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.UseCase.SignUpUser
{
    public class SignUpUserResponseObj
    {
        
        public User? NewUser { get; }

        public SignUpUserResponseObj(User newUser)
        {
            NewUser = newUser;
        }

    }
}
