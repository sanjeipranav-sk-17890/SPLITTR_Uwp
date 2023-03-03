using System;

namespace SPLITTR_Uwp.Core.SplittrExceptions;

public class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException() : base("User With that Email Already Exist")
    {
            
    }
}