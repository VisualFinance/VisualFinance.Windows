using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualFinance.Windows.Converters;

namespace VisualFinance.Windows.UnitTests.Converters
{
    [TestClass]
    public class FileSizeConverterFixture
    {
        [TestMethod]
        public void Convert_Normal_StringCorrectFormat()
        {
            FileSizeConverter converter = new FileSizeConverter();
            object input = 1024;
            string expected = "1KB";
            object output = converter.Convert(input, input.GetType(), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = 512;
            expected = "512B";
            output = converter.Convert(input, input.GetType(), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = 1024 * 1024;
            expected = "1MB";
            output = converter.Convert(input, input.GetType(), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = 1024 * 1024 * 1024;
            expected = "1GB";
            output = converter.Convert(input, input.GetType(), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = 1024L * 1024L * 1024L * 1024L;
            expected = "1TB";
            output = converter.Convert(input, input.GetType(), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

        }

        [TestMethod]
        public void Convert_Boundary_NullOrEmptyReturnsEmptyString()
        {
            FileSizeConverter converter = new FileSizeConverter();
            object input = null;
            string expected = string.Empty;
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = new Decimal?();
            expected = string.Empty;
            output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        public void Convert_Boundary_HandlesDoubleAndSingleTypes()
        {
            FileSizeConverter converter = new FileSizeConverter();
            object input = 100D;
            string expected = "100B";
            object output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));

            input = (Single)100;
            output = converter.Convert(input, typeof(string), null, Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(expected, output, string.Format("Expected result is '{0}'", expected));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBack_Erroneous_ThrowsNotSupportException()
        {
            FileSizeConverter converter = new FileSizeConverter();
            object output = converter.ConvertBack(null, null, null, null);
        }
    }
}