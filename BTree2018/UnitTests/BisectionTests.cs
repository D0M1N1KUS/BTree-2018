using System.Threading;
using BTree2018.Bisection;
using NUnit.Framework;
using UnitTests.HelperClasses;

namespace UnitTests
{
    [TestFixture]
    public class BisectionTests
    {
        [Test]
        public void bisectSearch_EvenNumberOfElementsInCollectionSearchedValueIsInCollection_ExactIndexShouldBeReturned()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            var expectedIndex = 7;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 8);

            Assert.AreEqual(expectedIndex, bisectSearch.LastIndex);
        }
        
        [Test]
        public void bisectSearch_UnEvenNumberOfElementsInCollectionSearchedValueIsInCollection_ExactIndexShouldBeReturned()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var expectedIndex = 7;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 8);

            Assert.AreEqual(expectedIndex, bisectSearch.LastIndex);
        }
        
        [Test]
        public void bisectSearch_SearchedValueIsAtTheLowerEndOfEvenCollection_ExactIndexShouldBeReturned()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            var expectedIndex = 0;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 1);

            Assert.AreEqual(expectedIndex, bisectSearch.LastIndex);
        }
        
        [Test]
        public void bisectSearch_SearchedValueIsAtTheLowerEndOfUnEvenCollection_ExactIndexShouldBeReturned()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var expectedIndex = 0;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 1);

            Assert.AreEqual(expectedIndex, bisectSearch.LastIndex);
        }
        
        [Test]
        public void bisectSearch_SearchedValueIsAtTheHigherEndOfEvenCollection_ExactIndexShouldBeReturned()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            var expectedIndex = 9;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 10);

            Assert.AreEqual(expectedIndex, bisectSearch.LastIndex);
        }
        
        [Test]
        public void bisectSearch_SearchedValueIsAtTheHigherEndOfUnEvenCollection_ExactIndexShouldBeReturned()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var expectedIndex = 10;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 11);

            Assert.AreEqual(expectedIndex, bisectSearch.LastIndex);
        }
        
        [Test]
        public void bisectSearch_SearchedValueIsNotInCollection_ExpectedIndexShouldBeCloseToValue()
        {
            var collection = new CustomCollection<int>(1, 2, 3, 4, 5, 6, 7, 9, 10, 11);
            var expectedIndex = 6;
            var bisectSearch = new BisectSearch<int>();

            bisectSearch.GetClosestIndexTo(collection, 8);

            Assert.AreEqual(bisectSearch.LastIndex, 7, 1);
            
        }
    }
}