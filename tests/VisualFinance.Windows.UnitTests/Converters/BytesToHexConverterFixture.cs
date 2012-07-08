using System;
using System.Threading;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualFinance.Windows.Converters;

namespace VisualFinance.Windows.UnitTests.Converters
{
    [TestClass]
    public class BytesToHexConverterFixture
    {
        [TestMethod]
        public void Convert_Normal_StringCorrectFormat()
        {
            IValueConverter converter = new BytesToHexConverter();
            byte[] input = new byte[] { 2, 4, 8, 16, 32, 64, 128, 255 };
            string expected = "0x02040810204080FF";
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        public void Convert_Normal_AcceptsVariousArrayLengths()
        {
            IValueConverter converter = new BytesToHexConverter();
            byte[] input = new byte[] { 2 };
            string expected = "0x02";
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 32, 64, 128, 255 };
            expected = "0x0102030405060708090A0B0C0D0E0F10204080FF";
            output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        public void Convert_Boundary_NullInputReturnsEmptyString()
        {
            IValueConverter converter = new BytesToHexConverter();
            byte[] input = null;
            string expected = "";
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        public void Convert_Boundary_EmptyInputReturns0x()
        {
            IValueConverter converter = new BytesToHexConverter();
            byte[] input = new byte[] { };
            object expected = "0x";
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_Erroneous_InvalidTargetTypeThrowsArgumentException()
        {
            IValueConverter converter = new BytesToHexConverter();
            object input = "0x";
            object output = converter.Convert(input, typeof(DateTime), null, Thread.CurrentThread.CurrentCulture);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Convert_Erroneous_InvalidValueTypeThrowsArgumentException()
        {
            IValueConverter converter = new BytesToHexConverter();
            object input = "Some string";
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);
        }


        [TestMethod]
        public void ConvertBack_Normal_StringToHex()
        {
            IValueConverter converter = new BytesToHexConverter();
            object input = "0x0102030405060708090A0B0C0D0E0F10204080FF";
            byte[] expected = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 32, 64, 128, 255 };
            byte[] output = (byte[])converter.ConvertBack(input, typeof(byte[]), null, Thread.CurrentThread.CurrentCulture);
            Assert.AreEqual(expected.Length, output.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], output[i]);
            }
        }

        [TestMethod]
        public void ConvertBack_Normal_NullInputReturnsNull()
        {
            IValueConverter converter = new BytesToHexConverter();
            object input = null;
            byte[] expected = null;
            byte[] output = (byte[])converter.ConvertBack(input, typeof(byte[]), null, Thread.CurrentThread.CurrentCulture);
            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertBack_Erroneous_InvalidFormatThrowsFormatException()
        {
            BytesToHexConverter converter = new BytesToHexConverter();
            object input = "Invalid string";
            object output = converter.ConvertBack(input, typeof(byte[]), null, Thread.CurrentThread.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertBack_Erroneous_PartiallyValidFormatThrowsFormatException()
        {
            BytesToHexConverter converter = new BytesToHexConverter();
            object input = "0x020408_FAIL_";
            object output = converter.ConvertBack(input, typeof(byte[]), null, Thread.CurrentThread.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertBack_Erroneous_NoLeading0xFormatException()
        {
            BytesToHexConverter converter = new BytesToHexConverter();
            object input = "020408FF";
            object output = converter.ConvertBack(input, typeof(byte[]), null, Thread.CurrentThread.CurrentCulture);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertBack_Erroneous_InvalidTargetTypeThrowsArgumentException()
        {
            BytesToHexConverter converter = new BytesToHexConverter();
            object input = "0x02";
            object output = converter.ConvertBack(input, typeof(DateTime), null, Thread.CurrentThread.CurrentCulture);
        }
    }
}