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

namespace UnitTests.BTreeOperationsTests
{
    [TestFixture]
    public class BTreeAddingTests
    {
        [Test]
        public void addKey_KeyDoesNotExistInFoundPageAndFitsInPage_KeyNeedsToBeInsertedAtTheBeginningOfThePage()
        {
            var nullPage = new PageTestFixture<int>();
            nullPage.PagePointer = new BTreePagePointer<int>() {PointsToPageType = PageType.NULL};
            var keyToAdd = new BTreeKey<int>()
            {
                LeftPagePointer = nullPage.PagePointer,
                RightPagePointer = nullPage.PagePointer, Value = 1
            };
            var testPage = new PageTestFixture<int>();
            testPage.PageType = PageType.ROOT;
            testPage.SetUpValues(2, 3, 4);
            testPage.SetUpPointers(nullPage.PagePointer, nullPage.PagePointer, nullPage.PagePointer, 
                nullPage.PagePointer);
            testPage.PageLength = 4;
            var btreeIOInterceptor = new BTreeIOTestFixture<int>();
            var btreeAdder = new BTreeAdder<int>();
            btreeAdder.BTreeIO = btreeIOInterceptor;
            btreeAdder.BTreeSearching = Substitute.For<IBTreeSearching<int>>();
            btreeAdder.BTreeSearching.SearchForPair(null).ReturnsForAnyArgs(false);
            btreeAdder.BTreeSearching.FoundPage.Returns(testPage);
            var expectedModifiedPage = new PageTestFixture<int>();
            expectedModifiedPage.SetUpValues(1, 2, 3, 4);
            expectedModifiedPage.SetUpPointers(nullPage.PagePointer, nullPage.PagePointer, nullPage.PagePointer, 
                nullPage.PagePointer, nullPage.PagePointer);
            
            btreeAdder.Add(keyToAdd);
            var actualModifiedPage = btreeIOInterceptor.WrittenPage[0];
            
            Assert.AreEqual(btreeIOInterceptor.WritePageCalls, 1);
            Assert.AreEqual(expectedModifiedPage.Length, actualModifiedPage.Length);
            for(var i = 0; i < expectedModifiedPage.Length; i++)
            {
                Assert.AreEqual(expectedModifiedPage.KeyAt(i).Value, actualModifiedPage.KeyAt(i).Value);
            }
        }
    }
}