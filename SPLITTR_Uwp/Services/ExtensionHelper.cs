using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

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

        public static T CreateInstance<T>(params object[] args)
        {
           return ActivatorUtilities.CreateInstance<T>(App.Container,parameters:args);
        }
    }

    internal static class InstanceHelper
    {
        public static T CreateInstance<T>(params object[] args)
        {
            return ActivatorUtilities.CreateInstance<T>(App.Container, parameters: args);
        }
    }

}
