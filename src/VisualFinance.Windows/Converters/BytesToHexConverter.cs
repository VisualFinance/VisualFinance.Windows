using System;
using System.Text;
using System.Windows.Data;
using VisualFinance.Windows.Properties;

namespace VisualFinance.Windows.Converters
{
    /// <summary>
    /// Converts byte arrays to a hexidecimal formatted string.
    /// </summary>
    [ValueConversion(typeof(byte[]), typeof(string))]
    public sealed class BytesToHexConverter : IValueConverter
    {
        private const string Prefix = "0x";
        #region IValueConverter Members

        /// <summary>
        /// Converts a byte array to a hexidecimal formatted string.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// The formatted string .
        /// </returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// IValueConverter converter = new BytesToHexConverter();
        /// 
        /// byte[] input = new byte[] { 2 };
        /// Console.WriteLine(converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentUICulture);
        /// 
        /// input = new byte[] { 2, 4, 8, 16, 32, 64, 128, 255 };
        /// Console.WriteLine(converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentUICulture);
        /// 
        /// //outputs the following to the console:
        /// //  0x02
        /// //  0x0102030405060708090A0B0C0D0E0F10204080FF
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="targetType"/> is not string or when the type of <paramref name="value"/> is not a <see cref="T:byte"/> array.</exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new ArgumentException(Resources.Converter_TargetType_must_be_string, "targetType");
            }
            if (value == null)
                return string.Empty;

            var input = value as byte[];
            if (input == null)
            {
                throw new ArgumentException(Resources.Converter_Value_cant_be_parsed_to_byte_array, "value");
            }

            int c = input.Length;
            var sb = new StringBuilder((c / 4) + 2);
            sb.Append(Prefix);
            for (int i = 0; i < c; i++)
            {
                sb.Append(input[i].ToString("X2", culture));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a <see cref="string"/> back to a <see cref="byte"/> array.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="targetType"/> is not a typeof(byte[]).</exception>
        /// <exception cref="FormatException">Thrown when the string does not begin with "0x", or when the string can not be parsed as a byte array.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(byte[]))
            {
                throw new ArgumentException(Resources.Converter_TargetType_must_be_byte_array, "targetType");
            }

            if (value == null)
                return null;

            var input = value.ToString();
            if (!input.StartsWith(Prefix, StringComparison.Ordinal))
            {
                throw new FormatException(string.Format(Resources.Converter_Value_has_invalid_prefix, Prefix));
            }

            var byteLength = (input.Length - 2) / 2;
            var bytes = new byte[byteLength];
            var j = Prefix.Length;  //skip the prefix (leading "0x");
            try
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    var hex = new string(new[] { input[j], input[j + 1] });
                    bytes[i] = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    j = j + 2;
                }
                return bytes;
            }
            catch (FormatException fe)
            {
                throw new FormatException(Resources.Converter_Value_cant_be_parsed_to_byte_array, fe);
            }
        }

        #endregion
    }
}