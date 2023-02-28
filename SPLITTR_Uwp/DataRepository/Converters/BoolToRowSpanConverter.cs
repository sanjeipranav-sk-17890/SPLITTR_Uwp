using System;
using Windows.UI.Xaml.Data;

namespace SPLITTR_Uwp.DataRepository.Converters;

public class BoolToRowSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var b = (bool)value;

        return b ?  1: 2;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}