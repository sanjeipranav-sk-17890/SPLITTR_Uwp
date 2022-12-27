using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Services
{
    internal static class ExtensionHelper
    {
        public static void ClearAndAdd<T>(this ObservableCollection<T>destination,  ICollection<T> source)
        {
            destination?.Clear();
            if(source is null)
            {
                return;
            }
            foreach (var obj in source)
            {
             destination?.Add(obj);   
            }
        }
    }
}
