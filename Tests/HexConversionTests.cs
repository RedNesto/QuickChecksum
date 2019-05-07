using System;
using NUnit.Framework;

namespace QuickChecksum.Tests
{
    [TestFixture]
    public class HexConversionTests
    {
        [Test]
        public void TestConversion()
        {
            const string originalHash = "012eab4c54886ca3fa52039afe03dcdf";
            var byteHash = HexConvert.FromString(originalHash);
            var convertedHash = HexConvert.ToString(byteHash);
            Assert.AreEqual(originalHash, convertedHash);
        }

        [Test]
        public void TestInvalidConversion()
        {
            Assert.Throws<ArgumentException>(() => {
                const string invalidHash = "blahb";
                HexConvert.FromString(invalidHash);
            });
        }
    }
}
