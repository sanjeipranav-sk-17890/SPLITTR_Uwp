using System;
using System.Collections.Generic;
using System.Text;

namespace SPLITTR_Uwp.Core.EventArg
{
    public class UserNameInvalidException : Exception
    {
        public UserNameInvalidException(string message) :base(message)
        {

        }
    }
}
