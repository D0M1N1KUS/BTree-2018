using BTree2018.BTreeIOComponents;
using NUnit.Framework;

namespace UnitTests.FileIOTests
{
    [TestFixture]
    public class GenericArithmeticTests
    {
        [Test]
        public void calculateSum_everySupportedValueWillBeTesed()
        {
            short sA = 1, sB = 2;
            int iA = 3, iB = 4;
            long lA = 5, lB = 6;
            float fA = 7, fB = 8;
            double dA = 9, dB = 10;
            short expectedShort = 3;
            int expectedInt = 7;
            long expectedLong = 11;
            float expectedFloat = 15;
            double expectedDouble = 19;

            var actualShort = GenericArithmetic<short>.Sum(sA, sB);
            var actualInt = GenericArithmetic<int>.Sum(iA, iB);
            var actualLong = GenericArithmetic<long>.Sum(lA, lB);
            var actualFloat = GenericArithmetic<float>.Sum(fA, fB);
            var actualDouble = GenericArithmetic<double>.Sum(dA, dB);
            
            Assert.AreEqual(expectedShort, actualShort);
            Assert.AreEqual(expectedInt, actualInt);
            Assert.AreEqual(expectedLong, actualLong);
            Assert.AreEqual(expectedFloat, actualFloat);
            Assert.AreEqual(expectedDouble, actualDouble);
        }
    }
}