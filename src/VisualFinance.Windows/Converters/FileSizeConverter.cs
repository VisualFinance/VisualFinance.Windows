using System;
using System.Windows.Data;

namespace VisualFinance.Windows.Converters
{
    /// <summary>
    /// Converts a string or <see cref="T:System.IFormattable"/> object to a formatted "file size" string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attempts to copy the windows exloper rules for choosing the file size measurement.
    /// </para>
    /// <para>
    /// <see cref="T:FileSizeConverter"/>.<see cref="M:Convert"/> returns a string representaion fit for UI display of 
    /// the file size passed in as measured by bytes. File sizes are automatically assigned to the most significant 
    /// measurement. Once a value is 0.9 or greater than the size of a significant measurement, then it is 
    /// represented as that measurement.
    /// </para>
    /// <para>File sizes are calculated up to TB (terrabytes).</para>
    /// <example>
    /// console.write(fileSizeConverter.Convert(920));   //Writes out "920B"
    /// console.write(fileSizeConverter.Convert(921));   //Writes out "0.9KB"
    /// </example>
    /// </remarks>
    /// <seealso cref="T:System.Windows.Data.IValueConverter"/>
    /// <seealso cref="T:NumericConverterBase"/>
    [ValueConversion(typeof(object), typeof(string))]
    public sealed class FileSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a string or <see cref="T:System.IFormattable"/> object to a formatted "file size" string.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A file size measurement or <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Double? number = NumericConverterBase.ConvertToNullableNumeric(value, culture);
            if (!number.HasValue)
            {
                return string.Empty;
            }

            double contentLength = number.Value;
            string retval;
            if (contentLength < 921)                //less than 0.9KB
            {
                retval = contentLength.ToString(culture);
                retval += "B";
            }
            else if (contentLength < 943718)        //less than 0.9MB
            {
                retval = (Math.Ceiling(contentLength / 1024d).ToString(culture));
                retval += "KB";
            }
            else if (contentLength < 966367641)     //less than 0.9GB
            {
                retval = (Math.Ceiling(contentLength / 1048576d).ToString(culture));        //1048576 = 1024^2;
                retval += ("MB");
            }
            else if (contentLength < 989560464998)  //less than 0.9TB
            {
                retval = (Math.Ceiling(contentLength / 1073741824d).ToString(culture));     //1073741824 = 1024^3;
                retval += ("GB");
            }
            else
            {
                retval = (Math.Ceiling(contentLength / 1099511627776d).ToString(culture));     //1099511627776 = 1024^4;
                retval += ("TB");
            }
            return retval;
        }

        /// <summary>
        /// Currently not supported. Could implement a losy convert-back.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}