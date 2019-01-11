using System;
using BTree2018.BTreeOperations.BTreeSplitting;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;
using UnitTests.HelperClasses;

namespace UnitTests.BTreeOperationsTests
{
    [TestFixture]
    public class BTreeSplittingTests
    {
        [Test]
        public void splitOverfilledLeafPage_ParentPageIsNotFull()
        {
            try
            {
                var expectedLeftPagePointer = new BTreePagePointer<int>() {Index = 100, PointsToPageType = PageType.LEAF};
                var expectedRightPagePointer = new BTreePagePointer<int>() {Index = 101, PointsToPageType = PageType.LEAF};
                getExpectedOutcomePages(out var expectedParentPage, out var expectedLeftPage, out var expectedRightPage, 
                    expectedLeftPagePointer, expectedRightPagePointer);
                var bTreeIO = getTestPages(out var initialParentPage, out var initialFilledPage,
                    expectedLeftPagePointer, expectedRightPagePointer);
                var bTreeAdding = Substitute.For<IBTreeAdding<int>>();
                var bTreeSplitter = new BTreePageSplitter<int>() {BTreeAdding = bTreeAdding, BTreeIO = bTreeIO};
                
                bTreeSplitter.Split(initialFilledPage);

                var actualLeftPage = bTreeIO.WrittenPage[0];
                var actualRightPage = bTreeIO.WrittenPage[1];
                Assert.AreEqual(expectedLeftPage, actualLeftPage);
                Assert.AreEqual(expectedRightPage, actualRightPage);
                bTreeAdding.Received().InsertKeyIntoPage(initialParentPage, 
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3}, 
                    expectedRightPagePointer);
            }
            catch(Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail();
            }
        }

        private BTreeIOTestFixture<int> getTestPages(out IPage<int> parentPage, out IPage<int> filledPage,
            IPagePointer<int> expectedLeftPagePointer, IPagePointer<int> expectedRightPagePointer)
        {
            parentPage = new BTreePageBuilder<int>()
                .AddPointer(expectedLeftPagePointer)
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 6})
                .SetPagePointer(BTreePagePointer<int>.NullPointer)
                .SetPageType(PageType.ROOT)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            filledPage = new BTreePage<int>()
            {
                Keys = new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5}
                },
                KeysInPage = 5, OverFlown = true, PageType = PageType.LEAF, PagePointer = expectedLeftPagePointer,
                PageLength = 4, ParentPage = parentPage.PagePointer,
                Pointers = new IPagePointer<int>[]
                {
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer, BTreePagePointer<int>.NullPointer,
                    BTreePagePointer<int>.NullPointer,
                }
            };

            var bTreeIoSubstitute = new BTreeIOTestFixture<int>();
            bTreeIoSubstitute.AddToGetPage(parentPage);
            bTreeIoSubstitute.AddToWritePage(null);
            bTreeIoSubstitute.AddToWritePage(expectedRightPagePointer);
            return bTreeIoSubstitute;
        }

        private void getExpectedOutcomePages(out IPage<int> expectedParentPage, out IPage<int> expectedLeftPage,
            out IPage<int> expectedRightPage,IPagePointer<int> expectedLeftPagePointer, 
            IPagePointer<int> expectedRightPagePointer)
        {
            expectedParentPage = new BTreePageBuilder<int>()
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3})
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 6})
                .AddPointer(expectedLeftPagePointer)
                .AddPointer(expectedRightPagePointer)
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .SetPagePointer(BTreePagePointer<int>.NullPointer)
                .SetPageType(PageType.ROOT)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();

            expectedLeftPage = new BTreePageBuilder<int>(4)
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .SetPageType(PageType.LEAF)
                .SetPagePointer(expectedLeftPagePointer)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();

            expectedRightPage = new BTreePageBuilder<int>(4)
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4})
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5})
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .SetPageType(PageType.LEAF)
                .SetPagePointer(expectedRightPagePointer)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
        }
    }
}