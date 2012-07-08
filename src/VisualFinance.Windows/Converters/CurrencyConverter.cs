using System.Globalization;
using System.Windows.Data;

namespace VisualFinance.Windows.Converters
{
    /// <summary>
    /// Converts a string or <see cref="T:System.IFormattable"/> object to a formatted currency string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will usually be used to convert numeric (<see cref="T:System.Int32"/>, 
    /// <see cref="T:System.Decimal"/> etc) to currency strings and back to the original type.
    /// </para>
    /// <para>
    /// As this is an explicit method of converting, <see cref="M:IValueConverter.ConvertBack"/> method 
    /// is implemented allowing full two-way binding. This converter does utilise <see cref="CultureInfo"/>
    /// to provide a localized conversion.
    /// </para>
    /// </remarks>
    /// <seealso cref="T:System.Windows.Data.IValueConverter"/>
    [ValueConversion(typeof(object), typeof(string))]
    public class CurrencyConverter : NumericConverterBase //IValueConverter
    {
        public override string Format
        {
            get { return "C"; }
        }

        public override NumberStyles Style
        {
            get { return NumberStyles.Currency; }
        }
    }
}