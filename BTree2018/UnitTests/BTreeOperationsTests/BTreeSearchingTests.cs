using BTree2018.Bisection;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;
using UnitTests.HelperClasses.BTree;

namespace UnitTests.BTreeOperationsTests
{
    [TestFixture]
    public class BTreeSearchingTests
    {
        [Test]
        public void findKeyRecordPair_ValueIsOnRootPage()
        {
            var searchedRecord = new Record<int>() {Value = 8};
            
            var nullPage = new PageTestFixture<int>();
            nullPage.PageType = PageType.NULL;
            nullPage.KeysInPage = -1; //To recognize it better while debugging
            
            var rootPage = new PageTestFixture<int>();
            rootPage.SetUpValues(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            rootPage.PageType = PageType.ROOT;
            rootPage.PageLength = 20;
            
            var btreeSearcher = new BTreeSearcher<int>();
            btreeSearcher.BTreeIO = Substitute.For<IBTreeIO<int>>();
            btreeSearcher.BTreeIO.GetRootPage().Returns(rootPage);
            btreeSearcher.BTreeIO.GetPage(null).ReturnsForAnyArgs(nullPage);
            btreeSearcher.BisectSearch = new BisectSearch<int>();

            var success = btreeSearcher.SearchForPair(rootPage.KeyAt(7), searchedRecord);
            
            Assert.IsTrue(success);
            Assert.AreEqual(searchedRecord.Value, btreeSearcher.FoundKey.Value);
        }

        [Test]
        public void findKeyRecordPair_ValueIsOnChildPage()
        {
            var searchedRecord = new Record<int>() {Value = 15};
            
            var nullPage = new PageTestFixture<int>();
            nullPage.PageType = PageType.NULL;
            nullPage.KeysInPage = -1; //To recognize it better while debugging
            
            var pageNullPointer = Substitute.For<IPagePointer<int>>();
            var childPagePointer = Substitute.For<IPagePointer<int>>();
            
            var rootPage = new PageTestFixture<int>();
            rootPage.SetUpValues(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            rootPage.PageType = PageType.ROOT;
            rootPage.PageLength = 13;
            rootPage.SetUpPointers(pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, 
                pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, 
                childPagePointer);
            
            var childPage = new PageTestFixture<int>();
            childPage.SetUpValues(11, 12, 13, 14, 15, 16, 17, 18, 19);
            childPage.SetUpPointers(pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, 
                pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer, pageNullPointer);
            childPage.PageType = PageType.LEAF;
            childPage.PageLength = 13;
            
            var btreeSearcher = new BTreeSearcher<int>();
            btreeSearcher.BTreeIO = Substitute.For<IBTreeIO<int>>();
            btreeSearcher.BTreeIO.GetRootPage().Returns(rootPage);
            //sadly does not work. Or I'm the on who's not working
//            btreeSearcher.BTreeIO.GetPage(pageNullPointer).Returns(nullPage);
//            btreeSearcher.BTreeIO.GetPage(childPagePointer).Returns(childPage); 
            btreeSearcher.BTreeIO.GetPage(null).ReturnsForAnyArgs(childPage, nullPage);
            btreeSearcher.BisectSearch = new BisectSearch<int>();

            var success = btreeSearcher.SearchForPair(childPage.KeyAt(4), searchedRecord);
            
            Assert.IsTrue(success);
            Assert.AreEqual(searchedRecord.Value, btreeSearcher.FoundKey.Value);
        }

        [Test]
        public void findKeyRecordPair_RecordDoesNotExist_NoRecordShouldBeReturned()
        {
            var searchedRecord = new Record<int>() {Value = 8};
            var nonExistingKey = new BTreeKey<int>() {Value = 8};
            var rootPage = new PageTestFixture<int>();
            rootPage.SetUpValues(1, 2, 3, 4, 5, 6, 7, 9, 10);
            rootPage.PageType = PageType.ROOT;
            rootPage.PageLength = 20;
            var btreeSearcher = new BTreeSearcher<int> {BTreeIO = Substitute.For<IBTreeIO<int>>()};
            btreeSearcher.BTreeIO.GetRootPage().Returns(rootPage);
            btreeSearcher.BisectSearch = new BisectSearch<int>();

            var success = btreeSearcher.SearchForPair(nonExistingKey, searchedRecord);
            
            Assert.IsFalse(success);
            Assert.IsNull(btreeSearcher.FoundKey);
        }
    }
}