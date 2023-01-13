using System.Collections.Generic;
using System.Collections.ObjectModel;

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
