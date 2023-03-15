using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SPLITTR_Uwp.Core.SplittrExceptions
{
    public class InvalidExpenseUpdationException : SplittrException
    {
        public InvalidExpenseUpdationException() : base(default,"User Has No Access To Updation")
        {
           
        }
        public InvalidExpenseUpdationException(string message) : base(default,message) 
        {
        }
    }
}
