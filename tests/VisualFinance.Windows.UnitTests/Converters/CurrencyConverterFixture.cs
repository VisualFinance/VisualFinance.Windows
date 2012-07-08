using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualFinance.Windows.Converters;

namespace VisualFinance.Windows.UnitTests.Converters
{
    [TestClass]
    public class CurrencyConverterFixture
    {
        [TestInitialize]
        public void Setup()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
        }

        [TestMethod]
        public void CurrencyConverter_Normal_ImplementsIValueConverter()
        {
            CurrencyConverter cc = new CurrencyConverter();
            IValueConverter vc = cc as IValueConverter;

            Assert.IsNotNull(vc, "CurrencyConverter should implement the IValueConverter interface");
        }

        [TestMethod]
        public void Convert_Normal_ConvertsToDollars()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.Convert(10, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$10.00", (string)result, "Should convert 10 to $10.00");

            result = cc.Convert("10", typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$10.00", (string)result, "Should convert \"10\" to $10.00");
        }

        //rounds correctly
        [TestMethod]
        public void Convert_Normal_RoundsCorrectly()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.Convert(9.99, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$9.99", (string)result, "Should convert 9.99 to $9.99");

            result = cc.Convert(9.995, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$10.00", (string)result, "Should convert 9.995 to $10.00");

            result = cc.Convert(9.994, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$9.99", (string)result, "Should convert 9.994 to $9.99");

            result = cc.Convert(.025, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$0.03", (string)result, "Should convert .025 to $0.03");

            result = cc.Convert(1234.567, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("$1,234.57", (string)result, "Should convert 1234.567 to $1,234.57");
        }

        //handles nulls and other invalid input.
        [TestMethod]
        public void Convert_Boundary_ReturnsEmptyStringForNullInputs()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object expected = string.Empty;
            object result = cc.Convert(null, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(expected, result, "Null inputs should become empty strings");

            result = cc.Convert(new int?(), typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(expected, result, "Null inputs should become empty strings");
        }

        [TestMethod]
        [ExpectedException(typeof(System.FormatException))]
        public void Convert_Boundary_ThrowsFormatExceptionForInvalidStrings()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.Convert("not a number", typeof(string), null, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(System.FormatException))]
        public void Convert_Boundary_ThrowsFormatExceptionForInvalidTypes()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.Convert(typeof(string), typeof(string), null, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(System.FormatException))]
        public void Convert_Boundary_ThrowsFormatExceptionForInvalidIFormattable()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.Convert(System.DateTime.Now, typeof(string), null, CultureInfo.CurrentCulture);
        }

        //converts to culture specific
        [TestMethod]
        public void Convert_Normal_RoundsCorrectlyForCulture()
        {
            CurrencyConverter cc = new CurrencyConverter();

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

            object result = cc.Convert(9.99, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("£9.99", (string)result, "Should convert 9.99 to £9.99");

            result = cc.Convert(9.995, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("£10.00", (string)result, "Should convert 9.995 to £10.00");

            result = cc.Convert(9.994, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("£9.99", (string)result, "Should convert 9.994 to £9.99");

            result = cc.Convert(.025, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("£0.03", (string)result, "Should convert .025 to £0.03");

            result = cc.Convert(1234.567, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("£1,234.57", (string)result, "Should convert 1234.567 to £1,234.57");


            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            result = cc.Convert(9.99, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("9,99 €", (string)result, "Should convert 9.99 to 9,99 €");

            result = cc.Convert(9.995, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("10,00 €", (string)result, "Should convert 9.995 to 10,00 €");

            result = cc.Convert(9.994, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("9,99 €", (string)result, "Should convert 9.994 to 9,99 €");

            result = cc.Convert(.025, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("0,03 €", (string)result, "Should convert .025 to 0,03 €");

            result = cc.Convert(1234.567, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual("1 234,57 €", (string)result, "Should convert 1234.567 to 1 234,57 €");
        }

        //converts back
        [TestMethod]
        public void ConvertBack_Normal_ConvertsBack()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.ConvertBack("$9.99", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(9.99, result, "Should convert $9.99 to 9.99");

            result = cc.ConvertBack("$0.03", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0.03, result, "Should convert $0.03 to 0.03");

            result = cc.ConvertBack("$1,234.57", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1234.57, result, "Should convert $1,234.57 to 1234.57");
        }

        //converts back for culture specific
        [TestMethod]
        public void ConvertBack_Normal_ConvertsBackForCulture()
        {
            CurrencyConverter cc = new CurrencyConverter();

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

            object result = cc.ConvertBack("£9.99", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(9.99, result, "Should convert £9.99 to 9.99");

            result = cc.ConvertBack("£0.03", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0.03, result, "Should convert £0.03 to 0.03");

            result = cc.ConvertBack("£1,234.57", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1234.57, result, "Should convert £1,234.57 to 1234.57");

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            result = cc.ConvertBack("9,99 €", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(9.99, result, "Should convert 9,99 € to 9.99");

            result = cc.ConvertBack("0,03 €", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0.03, result, "Should convert 0,03 € to 0.03");

            result = cc.ConvertBack("1 234,57 €", typeof(double), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1234.57, result, "Should convert 1 234,57 € to 1234.57");

            //All known cultures below.
            Helper("");
            Helper("af-ZA");
            Helper("sq-AL");
            Helper("ar-DZ");
            Helper("ar-BH");
            Helper("ar-EG");
            Helper("ar-IQ");
            Helper("ar-JO");
            Helper("ar-KW");
            Helper("ar-LB");
            Helper("ar-LY");
            Helper("ar-MA");
            Helper("ar-OM");
            Helper("ar-QA");
            Helper("ar-SA");
            Helper("ar-SY");
            Helper("ar-TN");
            Helper("ar-AE");
            Helper("ar-YE");
            Helper("hy-AM");
            Helper("az-Cyrl-AZ");
            Helper("az-Latn-AZ");
            Helper("eu-ES");
            Helper("be-BY");
            Helper("bg-BG");
            Helper("ca-ES");
            Helper("zh-HK");
            Helper("zh-MO");
            Helper("zh-CN");
            Helper("zh-SG");
            Helper("zh-TW");
            Helper("hr-HR");
            Helper("cs-CZ");
            Helper("da-DK");
            Helper("dv-MV");
            Helper("nl-BE");
            Helper("nl-NL");
            Helper("en-AU");
            Helper("en-BZ");
            Helper("en-CA");
            Helper("en-029");
            Helper("en-IE");
            Helper("en-JM");
            Helper("en-NZ");
            Helper("en-PH");
            Helper("en-ZA");
            Helper("en-TT");
            Helper("en-GB");
            Helper("en-US");
            Helper("en-ZW");
            Helper("et-EE");
            Helper("fo-FO");
            Helper("fa-IR");
            Helper("fi-FI");
            Helper("fr-BE");
            Helper("fr-CA");
            Helper("fr-FR");
            Helper("fr-LU");
            Helper("fr-MC");
            Helper("fr-CH");
            Helper("gl-ES");
            Helper("ka-GE");
            Helper("de-AT");
            Helper("de-DE");
            Helper("de-LI");
            Helper("de-LU");
            Helper("de-CH");
            Helper("el-GR");
            Helper("gu-IN");
            Helper("he-IL");
            Helper("hi-IN");
            Helper("hu-HU");
            Helper("is-IS");
            Helper("id-ID");
            Helper("it-IT");
            Helper("it-CH");
            Helper("ja-JP");
            Helper("kn-IN");
            Helper("kk-KZ");
            Helper("kok-IN");
            Helper("ko-KR");
            Helper("ky-KG");
            Helper("lv-LV");
            Helper("lt-LT");
            Helper("mk-MK");
            Helper("ms-BN");
            Helper("ms-MY");
            Helper("mr-IN");
            Helper("mn-MN");
            Helper("nb-NO");
            Helper("nn-NO");
            Helper("pl-PL");
            Helper("pt-BR");
            Helper("pt-PT");
            Helper("pa-IN");
            Helper("ro-RO");
            Helper("ru-RU");
            Helper("sa-IN");
            Helper("sr-Cyrl-CS");
            Helper("sr-Latn-CS");
            Helper("sk-SK");
            Helper("sl-SI");
            Helper("es-AR");
            Helper("es-BO");
            Helper("es-CL");
            Helper("es-CO");
            Helper("es-CR");
            Helper("es-DO");
            Helper("es-EC");
            Helper("es-SV");
            Helper("es-GT");
            Helper("es-HN");
            Helper("es-MX");
            Helper("es-NI");
            Helper("es-PA");
            Helper("es-PY");
            Helper("es-PE");
            Helper("es-PR");
            Helper("es-ES");
            Helper("es-ES_tradnl");
            Helper("es-UY");
            Helper("es-VE");
            Helper("sw-KE");
            Helper("sv-FI");
            Helper("sv-SE");
            Helper("syr-SY");
            Helper("ta-IN");
            Helper("tt-RU");
            Helper("te-IN");
            Helper("th-TH");
            Helper("tr-TR");
            Helper("uk-UA");
            Helper("ur-PK");
            Helper("uz-Cyrl-UZ");
            Helper("uz-Latn-UZ");
            Helper("vi-VN");

        }

        [TestMethod]
        public void ConvertBack_Normal_ConvertsBackToGenericTypes()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.ConvertBack("$9.99", typeof(double?), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(9.99, result, "Should convert $9.99 to 9.99");

            result = cc.ConvertBack("", typeof(int?), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(new int?(), result, "Should convert $0.03 to 0.03");

            result = cc.ConvertBack(null, typeof(decimal?), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(new decimal?(), result, "Should convert $1,234.57 to 1234.57");
        }

        [TestMethod]
        public void ConvertBack_Boundary_ReturnsNullForReferenceTargetTypesIfValueIsNull()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.ConvertBack(null, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(null, result, "Should convert null to null for string target type");
        }

        [TestMethod]
        public void ConvertBack_Boundary_ReturnsNullForReferenceTargetTypesIfValueIsEmptyString()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.ConvertBack(string.Empty, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(null, result, "Should convert empty string to null for string target type");
        }

        [TestMethod]
        public void ConvertBack_Boundary_ReturnsNullForNullableDecimalTargetTypesIfValueIsNull()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.ConvertBack(null, typeof(Decimal?), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(null, result, "Should convert null to null for Decimal? target type");
        }

        [TestMethod]
        public void ConvertBack_Boundary_ReturnsNullForNullableDecimalTargetTypesIfValueIsEmptyString()
        {
            CurrencyConverter cc = new CurrencyConverter();

            object result = cc.ConvertBack(string.Empty, typeof(Decimal?), null, CultureInfo.CurrentCulture);
            Assert.AreEqual(null, result, "Should convert empty string to null for Decimal? target type");
        }



        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ConvertBack_Erroneous_ThrowsInvalidcastExceptionForInvalidTargetType()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.ConvertBack("$10.50", typeof(DateTime), null, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertBack_Erroneous_ThrowsFormatExceptionForValuesThatCanBeConvertedToDecimal()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.ConvertBack("Fail string", typeof(DateTime), null, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertBack_Erroneous_ThrowsFormatExceptionForNullValuesAndWithNonNullTargetTypes()
        {
            CurrencyConverter cc = new CurrencyConverter();
            object result = cc.ConvertBack(null, typeof(DateTime), null, CultureInfo.CurrentCulture);
        }

        private void Helper(string culture)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            CurrencyConverter cc = new CurrencyConverter();
            double input = 9.99;
            object result = null;
            object output = null;
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalDigits > 0)
            {
                result = cc.Convert(input, typeof(string), null, CultureInfo.CurrentCulture);
                output = cc.ConvertBack(result, typeof(double), null, CultureInfo.CurrentCulture);
                Assert.AreEqual(input, output, "Input should be Converted and converted back to the same value. Culture = " + culture);

                input = 1234.57;
                result = cc.Convert(input, typeof(string), null, CultureInfo.CurrentCulture);
                output = cc.ConvertBack(result, typeof(double), null, CultureInfo.CurrentCulture);

                Assert.AreEqual(input, output, "Input should be Converted and converted back to the same value. Culture = " + culture);
            }
            else
            {
                input = 999;
                result = cc.Convert(input, typeof(string), null, CultureInfo.CurrentCulture);
                output = cc.ConvertBack(result, typeof(double), null, CultureInfo.CurrentCulture);
                Assert.AreEqual(input, output, "Input should be Converted and converted back to the same value. Culture = " + culture);

                input = 123457;
                result = cc.Convert(input, typeof(string), null, CultureInfo.CurrentCulture);
                output = cc.ConvertBack(result, typeof(double), null, CultureInfo.CurrentCulture);

                Assert.AreEqual(input, output, "Input should be Converted and converted back to the same value. Culture = " + culture);
            }
        }
    }
}
