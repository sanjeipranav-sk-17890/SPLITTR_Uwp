using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Microsoft.Extensions.DependencyInjection;

namespace SPLITTR_Uwp.Services
{
    internal static class ExtensionHelper
    {
        public static void ClearAndAdd<T>(this ObservableCollection<T>destination,  IEnumerable<T> source)
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

        public static Task RunOnUIContextAsync(this CoreDispatcher dispatcher,Action function)
        {
            if (!dispatcher.HasThreadAccess)
            {
                return dispatcher?.RunAsync(CoreDispatcherPriority.Normal, (() =>
                {
                    function?.Invoke();
                })).AsTask();
            }
            function?.Invoke();
            return Task.CompletedTask;
        }
    }

    internal static class InstanceBuilder
    {
        public static T CreateInstance<T>(params object[] args)
        {
            return ActivatorUtilities.CreateInstance<T>(App.Container, parameters: args);
        }
    }

}
