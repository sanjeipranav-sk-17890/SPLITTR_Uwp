#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
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
