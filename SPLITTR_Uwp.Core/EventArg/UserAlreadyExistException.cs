using System;
using System.Collections.Generic;
using System.Text;

namespace SPLITTR_Uwp.Core.EventArg
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException() : base("User With that Email Already Exist")
        {
            
        }
    }
}
