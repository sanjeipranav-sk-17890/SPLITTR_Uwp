using System;

namespace SPLITTR_Uwp.Core.EventArg;

public class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException() : base("User With that Email Already Exist")
    {
            
    }
}