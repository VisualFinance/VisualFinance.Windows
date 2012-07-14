using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    public class IndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var leftMargin = Converters.NumericConverterBase.ConvertToNumeric(value, culture) * 5;
            return new Thickness(leftMargin, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
