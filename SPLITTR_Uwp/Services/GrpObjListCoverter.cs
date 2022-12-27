using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Services
{
    internal class GrpObjListCoverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<GroupBobj> collection)
            {
                return collection;
            }
            throw new NotSupportedException("Suppose to Be Coverted to Observable Collection");
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
