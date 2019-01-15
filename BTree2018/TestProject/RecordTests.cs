using BTree2018.BTreeStructure;
using BTree2018.Enums;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class RecordTests
    {
        [Test]
        public void compareRecordsTest_RecordsShouldBeEqual()
        {
            var record1 = new Record<int>()
            {
                Value = 10, ValueComponents = new int[] {1, 2, 3, 4, 5, 10}
            };
            var record2 = new Record<int>()
            {
                Value = 10, ValueComponents = new int[] {1, 1, 1, 1, 1, 10}
            };

            var comparison = record1.CompareTo(record2);
            
            Assert.IsTrue(comparison == 0);
        }
        
        [Test]
        public void compareSlightlyDifferentRecordsTest_OneRecordHaseOneValuecomponentLess_ShouldNotBeEqual()
        {
            var record1 = new Record<int>()
            {
                Value = 10, ValueComponents = new int[] {1, 2, 3, 4, 10}
            };
            var record2 = new Record<int>()
            {
                Value = 10, ValueComponents = new int[] {1, 2, 3, 4, 5, 10}
            };

            var comparison = record1.CompareTo(record2);
            
            Assert.IsTrue(comparison == (int)Comparison.LESS);
        }
        
        [Test]
        public void compareRecordsWithDifferentValues_TheNumberOfValueComponentsIsTheSame_ShouldNotBeEqual()
        {
            var record1 = new Record<int>()
            {
                Value = 6, ValueComponents = new int[] {1, 2, 3, 4, 5, 6}
            };
            var record2 = new Record<int>()
            {
                Value = 10, ValueComponents = new int[] {1, 2, 3, 4, 5, 10}
            };

            var comparison = record1.CompareTo(record2);
            
            Assert.IsTrue(comparison == 1);
        }

        [Test]
        public void recordValueCalculationTest()
        {
            var expectedValue = 10;
            
            var actualValue = new Record<int>(new []{-10,2,3,4,5,6,10}, 
                RecordPointer<int>.NullPointer).Value;
            
            
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}