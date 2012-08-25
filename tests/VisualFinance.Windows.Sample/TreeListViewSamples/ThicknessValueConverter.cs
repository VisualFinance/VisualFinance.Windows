using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    public class ThicknessValueConverter : IValueConverter
    {
        public double TopScale { get; set; }
        public double BottomScale { get; set; }
        public double LeftScale { get; set; }
        public double RightScale { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var factor = Converters.NumericConverterBase.ConvertToNumeric(value, culture);
            var topValue = factor * TopScale;
            var bottomValue = factor * BottomScale;
            var leftValue = factor * LeftScale;
            var rightValue = factor * RightScale;

            return new Thickness(leftValue, topValue, rightValue, bottomValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
