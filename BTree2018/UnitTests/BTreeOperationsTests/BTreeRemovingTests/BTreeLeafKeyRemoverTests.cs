using System;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;
using NSubstitute;
using NUnit.Framework;
using UnitTests.HelperClasses;

namespace UnitTests.BTreeOperationsTests.BTreeRemovingTests
{
    [TestFixture]
    public class BTreeLeafKeyRemoverTests
    {
        private IKey<int> expectedBiggestKey = new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1000};
        [Test]
        public void removeBiggestKey()
        {
            try
            {
                var bTreeIO = prepareTestTree(out var beginningPage);
                var bTreeLeafKeyRemover = new BTreeLeafKeyRemover<int>();
                bTreeLeafKeyRemover.BTreeIO = bTreeIO;
                var expectedModifiedLeafPage = getExpectedModifiedLeafPage();

                var actualBiggestKey =
                    bTreeLeafKeyRemover.RemoveBiggestKey(beginningPage, out var actualModifiedLeafPage);

                Assert.AreEqual(expectedBiggestKey, actualBiggestKey);
                Assert.AreEqual(expectedModifiedLeafPage, actualModifiedLeafPage);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.Write(Logger.GetLog());
                Assert.Fail();
            }
        }

        #region removeBiggestKey_Subroutines

        private IBTreeIO<int> prepareTestTree(out IPage<int> beginningPage)
        {
            var pointerToBranch = new BTreePagePointer<int>() {Index = 10, PointsToPageType = PageType.BRANCH};
            var pointerToLeaf =  new BTreePagePointer<int>() {Index = 20, PointsToPageType = PageType.LEAF};
            
            beginningPage = new BTreePage<int>()
            {
                Keys = new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3},
                    null 
                },
                Pointers = new[]
                {
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer, pointerToBranch
                },
                KeysInPage = 3, PageLength = 4, PageType = PageType.ROOT, ParentPage = BTreePagePointer<int>.NullPointer
            };
            
            var branchPage = new BTreePage<int>()
            {
                Keys = new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 6},
                    null 
                },
                Pointers = new[]
                {
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer, pointerToLeaf
                },
                KeysInPage = 3, PageLength = 4, PageType = PageType.BRANCH, ParentPage = BTreePagePointer<int>.NullPointer
            };
            
            var leafPage = new BTreePage<int>()
            {
                Keys = new[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 7},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 8},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 9},
                    expectedBiggestKey
                },
                Pointers = new[]
                {
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer
                },
                KeysInPage = 4, PageLength = 4, PageType = PageType.LEAF, ParentPage = BTreePagePointer<int>.NullPointer
            };

            var bTreeIO = Substitute.For<IBTreeIO<int>>();
            bTreeIO.GetPage(pointerToBranch).Returns(branchPage);
            bTreeIO.GetPage(pointerToLeaf).Returns(leafPage);

            return bTreeIO;
        }

        private IPage<int> getExpectedModifiedLeafPage()
        {
            return new BTreePage<int>()
            {
                Keys = new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 7},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 8},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 9},
                    null
                },
                Pointers = new[]
                {
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer
                },
                KeysInPage = 3, PageLength = 4, PageType = PageType.LEAF, ParentPage = BTreePagePointer<int>.NullPointer
            };
        }

        #endregion
        
        
    }
}