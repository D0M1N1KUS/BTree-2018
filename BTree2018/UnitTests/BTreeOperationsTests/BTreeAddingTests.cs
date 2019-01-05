using System.Linq;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using NSubstitute;
using NUnit.Framework;
using UnitTests.HelperClasses;
using UnitTests.HelperClasses.BTree;
using System.Collections.Generic;
using NSubstitute.ExceptionExtensions;

namespace UnitTests.BTreeOperationsTests
{
    [TestFixture]
    public class BTreeAddingTests
    {
        [Test]
        public void addKey_KeyDoesNotExistInFoundPageAndFitsInPage_KeyNeedsToBeInsertedAtTheBeginningOfThePage()
        {
            var keyToAdd = preparePageWithOneEmptySpace(1, out var btreeIOInterceptor, out var btreeAdder,
                out var expectedModifiedPage, 2, 3, 4);

            var actualModifiedPage = btreeAdder.Add(keyToAdd);
            
            Assert.AreEqual(expectedModifiedPage.Length, actualModifiedPage.Length);
            for(var i = 0; i < expectedModifiedPage.Length; i++)
            {
                Assert.AreEqual(expectedModifiedPage.KeyAt(i).Value, actualModifiedPage.KeyAt(i).Value);
            }
        }

        //sry, for that copy/paste, but testing this is pure cancer
        [Test]
        public void addKey_KeyDoesNotExistInFoundPageAndFitsInPage_KeyNeedsToBeInsertedAtTheEndOfThePage()
        {
            var keyToAdd = preparePageWithOneEmptySpace(4, out var btreeIOInterceptor, out var btreeAdder,
                out var expectedModifiedPage, 1, 2, 3);

            var actualModifiedPage = btreeAdder.Add(keyToAdd);

            Assert.AreEqual(expectedModifiedPage.Length, actualModifiedPage.Length);
            for(var i = 0; i < expectedModifiedPage.Length; i++)
            {
                Assert.AreEqual(expectedModifiedPage.KeyAt(i).Value, actualModifiedPage.KeyAt(i).Value);
            }
        }

        [Test]
        public void addKey_KeyDoesNotExistInFoundPageAndFitsInPage_KeyNeedsToBeInsertedInTheMiddleOfThePage()
        {
            var keyToAdd = preparePageWithOneEmptySpace(3, out var btreeIOInterceptor, out var btreeAdder,
                out var expectedModifiedPage, 1, 2, 4);
            
            var actualModifiedPage = btreeAdder.Add(keyToAdd);

            Assert.AreEqual(expectedModifiedPage.Length, actualModifiedPage.Length);
            for(var i = 0; i < expectedModifiedPage.Length; i++)
            {
                Assert.AreEqual(expectedModifiedPage.KeyAt(i).Value, actualModifiedPage.KeyAt(i).Value);
            }
        }

        [Test]
        public void insertKeyIntoPage_OperationIsUsedByWhileSplitting()
        {
            var pagePointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.LEAF}
            };
            var expectedPagePointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 4, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.LEAF}
            };
            var keyToInsert = new BTreeKey<int>() {Value = 3, RecordPointer = RecordPointer<int>.NullPointer};
            var pointerToInsert = new BTreePagePointer<int>() {Index = 4, PointsToPageType = PageType.LEAF};

            var initialPage = new PageTestFixture<int>();
            initialPage.PageLength = 4;
            initialPage.PageType = PageType.ROOT;
            initialPage.SetUpValues(1, 2, 4);
            initialPage.SetUpPointers(pagePointers);
            var adder = new BTreeAdder<int>();
            var btreeIOInterceptor = new BTreeIOTestFixture<int>();
            adder.BTreeIO = btreeIOInterceptor;
            var expectedPage = new PageTestFixture<int>();
            expectedPage.PageLength = 4;
            expectedPage.PageType = PageType.ROOT;
            expectedPage.SetUpValues(1, 2, 3, 4);
            expectedPage.SetUpPointers(expectedPagePointers);
            
            var actualPage = adder.InsertKeyIntoPage(initialPage, keyToInsert, pointerToInsert);

            Assert.IsTrue(expectedPage.Equals(actualPage));
        }
        
        private static BTreeKey<int> preparePageWithOneEmptySpace(int valueToAdd, out BTreeIOTestFixture<int> btreeIOInterceptor,
            out BTreeAdder<int> btreeAdder, out PageTestFixture<int> expectedModifiedPage, params int[] valuesInPage)
        {
            var nullPage = new PageTestFixture<int>();
            nullPage.PagePointer = new BTreePagePointer<int>() {PointsToPageType = PageType.NULL};
            var keyToAdd = new BTreeKey<int>() { Value = valueToAdd };
            var testPage = new PageTestFixture<int>();
            testPage.PageType = PageType.ROOT;
            testPage.SetUpValues(valuesInPage);
            testPage.SetUpPointers(nullPage.PagePointer, nullPage.PagePointer, nullPage.PagePointer,
                nullPage.PagePointer);
            testPage.PageLength = 4;
            btreeIOInterceptor = new BTreeIOTestFixture<int>();
            btreeAdder = new BTreeAdder<int>();
            btreeAdder.BTreeIO = btreeIOInterceptor;
            btreeAdder.BTreeSearching = Substitute.For<IBTreeSearching<int>>();
            btreeAdder.BTreeSearching.SearchForKey(null).ReturnsForAnyArgs(false);
            btreeAdder.BTreeSearching.FoundPage.Returns(testPage);
            expectedModifiedPage = new PageTestFixture<int>();
            expectedModifiedPage.SetUpValues(addValueToArrayAndSort(valueToAdd, valuesInPage));
            expectedModifiedPage.SetUpPointers(nullPage.PagePointer, nullPage.PagePointer, nullPage.PagePointer,
                nullPage.PagePointer, nullPage.PagePointer);
            return keyToAdd;
        }

        private static int[] addValueToArrayAndSort(int valueToAdd, params int[] valuesInPage)
        {
            var allValues = new int[valuesInPage.Length + 1];
            valuesInPage.CopyTo(allValues, 0);
            allValues[allValues.Length - 1] = valueToAdd;
            var sortList = new List<int>(allValues);
            sortList.Sort();
            return sortList.ToArray();
        }

    }
}