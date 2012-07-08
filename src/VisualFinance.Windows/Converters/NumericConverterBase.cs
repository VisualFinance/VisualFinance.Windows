using System;
using System.Globalization;
using System.Windows.Data;
using VisualFinance.Windows.Properties;

namespace VisualFinance.Windows.Converters
{
    /// <summary>
    /// A base class for Numeric Converters.
    /// </summary>
    /// <remarks>
    /// Provides an implementation to convert objects to numeric values.
    /// </remarks>
    public abstract class NumericConverterBase : IValueConverter
    {
        /// <summary>
        /// The format to use when the type to convert from is either a <see cref="float"/> or a <see cref="double"/>.
        /// </summary>
        protected const string RoundtripFormatter = "R";

        /// <summary>
        /// The general format to use. Applicable to most data types.
        /// </summary>
        protected const string GeneralFormatter = "G";

        /// <summary>
        /// Gets the .NET numeric format string.
        /// </summary>
        /// <value>The format.</value>
        /// <remarks>
        /// Implementation values should conform to the .NET numeric format strings.
        /// <example>
        /// "C"
        /// </example>
        /// <example>
        /// "D6"
        /// </example>
        /// <example>
        /// "E03"
        /// </example>
        /// <example>
        /// "P02"
        /// </example>
        /// <example>
        /// "X"
        /// </example>
        /// </remarks>
        public abstract string Format { get; }

        public abstract NumberStyles Style { get; }

        /// <summary>
        /// Attempts to convert the object to a numeric value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="provider">The format provider to use for conversion. </param>
        /// <returns>Returns a <see cref="double"/> with a value if the conversion was successfull.</returns>
        /// <exception cref="FormatException">Thrown when the value can not be formatted as a <see cref="double"/>.</exception>
        public static Double ConvertToNumeric(object value, IFormatProvider provider)
        {
            return ConvertToNumeric(value, provider, NumberStyles.Any);
        }

        /// <summary>
        /// Attempts to convert the object to a numeric value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="provider">The format provider to use for conversion. </param>
        /// <returns>Returns a <see cref="Nullable&lt;Double&gt;"/> with a value if the conversion was successfull.</returns>
        /// <exception cref="FormatException">Thrown when the value can not be formatted as a <see cref="Double"/>.</exception>
        public static Double? ConvertToNullableNumeric(object value, IFormatProvider provider)
        {
            return ConvertToNullableNumeric(value, provider, NumberStyles.Any);
        }

        public static Double? ConvertToNullableNumeric(object value, IFormatProvider provider, NumberStyles style)
        {
            if (value == null)
                return null;
            return ConvertToNumeric(value, provider, style);
        }

        public static Double ConvertToNumeric(object value, IFormatProvider provider, NumberStyles style)
        {
            var parsableString = value as String;
            if (parsableString == null)
            {
                var formatter = value as IFormattable;
                if (formatter != null)
                {
                    string format = GeneralFormatter;
                    if (formatter is Single || formatter is Double)
                    {
                        format = RoundtripFormatter;
                    }
                    parsableString = formatter.ToString(format, provider);
                }
            }
            if (parsableString != null)
            {
                Double number;
                if (Double.TryParse(parsableString, style, provider, out number))
                {
                    return number;
                }
            }
            throw new FormatException(Resources.Converter_Value_cant_be_parsed_to_numeric);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new ArgumentException(Resources.Converter_TargetType_must_be_string, "targetType");

            if (value == null)
            {
                return string.Empty;
            }
            IFormattable formattable;
            var strValue = value as string;
            if (strValue != null)
            {
                decimal decValue;
                if (Decimal.TryParse(strValue, out decValue))
                {
                    formattable = decValue;
                }
                else
                {
                    throw new FormatException(Resources.Converter_Value_cant_be_parsed_to_numeric);
                }
            }
            else
            {
                formattable = value as IFormattable;
            }
            if (formattable == null)
            {
                throw new FormatException(Resources.Converter_Value_cant_be_parsed_to_IFormattable);
            }
            return formattable.ToString(Format, culture);	//Format argument is the abstract property
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsNullOrEmpty(value))
            {
                if (AcceptsNulls(targetType))
                {
                    return null;
                }
                throw new FormatException(string.Format(Resources.Converter_TargetType_cant_be_null, targetType.Name));

            }
            var result = ConvertToNumeric(value, culture, Style);

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var types = targetType.GetGenericArguments();
                var baseTargetType = types[0];
                return ((IConvertible)result).ToType(baseTargetType, culture);

            }
            return ((IConvertible)result).ToType(targetType, culture);
        }

        /// <summary>
        /// Determines whether the object is null or an empty string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true is the value is null or an empty string.</returns>
        private static bool IsNullOrEmpty(object value)
        {
            return value == null || value.ToString().Length == 0;
        }

        /// <summary>
        /// Returns true if the <paramref name="targetType"/> accepts nulls. ie. either a reference type a <see cref="Nullable&lt;T&gt;"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>true if the <paramref name="targetType"/> is a reference type or <see cref="Nullable&lt;T&gt;"/> type.</returns>
        private static bool AcceptsNulls(Type targetType)
        {
            if (!targetType.IsValueType
                ||
                (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                )
            {
                return true;
            }
            return false;
        }
    }
}
