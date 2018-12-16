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

            btreeAdder.Add(keyToAdd);
            var actualModifiedPage = btreeIOInterceptor.WrittenPage[0];
            
            Assert.AreEqual(btreeIOInterceptor.WritePageCalls, 1);
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
            
            btreeAdder.Add(keyToAdd);
            var actualModifiedPage = btreeIOInterceptor.WrittenPage[0];
            
            Assert.AreEqual(btreeIOInterceptor.WritePageCalls, 1);
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
            
            btreeAdder.Add(keyToAdd);
            var actualModifiedPage = btreeIOInterceptor.WrittenPage[0];
            
            Assert.AreEqual(btreeIOInterceptor.WritePageCalls, 1);
            Assert.AreEqual(expectedModifiedPage.Length, actualModifiedPage.Length);
            for(var i = 0; i < expectedModifiedPage.Length; i++)
            {
                Assert.AreEqual(expectedModifiedPage.KeyAt(i).Value, actualModifiedPage.KeyAt(i).Value);
            }
        }
        
        private static BTreeKey<int> preparePageWithOneEmptySpace(int valueToAdd, out BTreeIOTestFixture<int> btreeIOInterceptor,
            out BTreeAdder<int> btreeAdder, out PageTestFixture<int> expectedModifiedPage, params int[] valuesInPage)
        {
            var nullPage = new PageTestFixture<int>();
            nullPage.PagePointer = new BTreePagePointer<int>() {PointsToPageType = PageType.NULL};
            var keyToAdd = new BTreeKey<int>()
            {
                LeftPagePointer = nullPage.PagePointer,
                RightPagePointer = nullPage.PagePointer, Value = valueToAdd
            };
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
            btreeAdder.BTreeSearching.SearchForPair(null).ReturnsForAnyArgs(false);
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