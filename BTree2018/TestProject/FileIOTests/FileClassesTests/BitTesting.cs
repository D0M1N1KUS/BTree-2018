using NUnit.Framework;

namespace UnitTests.FileIOTests.FileClassesTests
{
    [TestFixture]
    public class BitTesting
    {
        [Test]
        [Ignore("This test was just a proof of concept.")]
        public void checkIfBitInByteIsSet_testingSomeonesThought()
        {
            byte expectedByte = 0b11001101;
            byte actualByte = 0;
            for (var i = 7; i >= 0; i--)
            {
                actualByte <<= 1;
                if (isSet(expectedByte, i + 1))
                    actualByte |= 0b00000001;
            }
            
            Assert.AreEqual(expectedByte, actualByte);
        }

        private bool isSet(byte b, int n)
        {
            return (b & (1 << n - 1)) != 0;
        }
    }
}