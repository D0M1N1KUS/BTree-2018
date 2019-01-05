using System;
using System.Collections.Generic;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.BTreeOperationsTests.CompensationTests
{
    [TestFixture]
    public class BTreePageNeighboursTests
    {
        [Test]
        public void getNeighbours_pageHasTwoNeighbours()
        {
            try
            {
                var bTreeIO = getBTreeIO(out var parentPage, out var leafPages, out var leafPagePointers);
                var bTreePageNeighbours = new BTreePageNeighbours<int> {BTreeIO = bTreeIO};
                var expectedLeftNeighbourPointer = leafPagePointers[0];
                var expectedRightNeighbourPointer = leafPagePointers[2];
                var expectedParentKeyIndex = 1;
                var expectedParentKey = parentPage.KeyAt(expectedParentKeyIndex);


                var success = bTreePageNeighbours.GetNeighbours(leafPages[1], out var leftNeighbourPtr,
                    out var rightNeighbourPtr, out var parentKey, out var parentKeyIndex);

                Assert.IsTrue(expectedLeftNeighbourPointer.Equals(leftNeighbourPtr));
                Assert.IsTrue(expectedRightNeighbourPointer.Equals(rightNeighbourPtr));
                Assert.AreEqual(expectedParentKeyIndex, parentKeyIndex);
                Assert.IsTrue(expectedParentKey.Equals(parentKey));
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail("Exception thrown or Assert failed.");
            }
        }

        [Test]
        public void getNeighbours_pageOnlyHasRightNeighbour()
        {
            try
            {
                var bTreeIO = getBTreeIO(out var parentPage, out var leafPages, out var leafPagePointers);
                var bTreePageNeighbours = new BTreePageNeighbours<int> {BTreeIO = bTreeIO};
                var expectedRightNeighbourPointer = leafPagePointers[1];
                var expectedParentKeyIndex = 0;
                var expectedParentKey = parentPage.KeyAt(expectedParentKeyIndex);


                var success = bTreePageNeighbours.GetNeighbours(leafPages[0], out var leftNeighbourPtr,
                    out var rightNeighbourPtr, out var parentKey, out var parentKeyIndex);

                Assert.AreEqual(BTreePagePointer<int>.NullPointer, leftNeighbourPtr);
                Assert.IsTrue(expectedRightNeighbourPointer.Equals(rightNeighbourPtr));
                Assert.AreEqual(expectedParentKeyIndex, parentKeyIndex);
                Assert.IsTrue(expectedParentKey.Equals(parentKey));
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail("Exception thrown or Assert failed.");
            }
        }

        [Test]
        public void getNeighbours_pageOnlyHasLeftNeighbour()
        {
            try
            {
                var bTreeIO = getBTreeIO(out var parentPage, out var leafPages, out var leafPagePointers);
                var bTreePageNeighbours = new BTreePageNeighbours<int> {BTreeIO = bTreeIO};
                var expectedLeftNeighbourPointer = leafPagePointers[1];
                var expectedParentKeyIndex = 1;
                var expectedParentKey = parentPage.KeyAt(expectedParentKeyIndex);


                var success = bTreePageNeighbours.GetNeighbours(leafPages[2], out var leftNeighbourPtr,
                    out var rightNeighbourPtr, out var parentKey, out var parentKeyIndex);

                Assert.IsTrue(expectedLeftNeighbourPointer.Equals(leftNeighbourPtr));
                Assert.AreEqual(BTreePagePointer<int>.NullPointer, rightNeighbourPtr);
                Assert.AreEqual(expectedParentKeyIndex, parentKeyIndex);
                Assert.IsTrue(expectedParentKey.Equals(parentKey));
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail("Exception thrown or Assert failed.");
            }
        }
        
        
        private IBTreeIO<int> getBTreeIO(out IPage<int> parentPage, out IPage<int>[] leafPages, 
            out IPagePointer<int>[] leafPagePointers)
        {
            var rootPagePointer = new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.ROOT};
            leafPagePointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF}
            };
            
            parentPage = new BTreePageBuilder<int>()
                .AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(leafPagePointers))
                .SetPageType(PageType.ROOT)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .SetPagePointer(new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.ROOT})
                .Build();
            
            leafPages = new IPage<int>[]
            {
                new BTreePageBuilder<int>()
                    .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 0})
                    .AddPointer(new BTreePagePointer<int>() {Index = 4, PointsToPageType = PageType.NULL})
                    .AddPointer(new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.NULL})
                    .SetPageType(PageType.LEAF)
                    .SetPagePointer(leafPagePointers[0])
                    .SetParentPagePointer(rootPagePointer)
                    .Build(),
                new BTreePageBuilder<int>()
                    .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                    .AddPointer(new BTreePagePointer<int>() {Index = 6, PointsToPageType = PageType.NULL})
                    .AddPointer(new BTreePagePointer<int>() {Index = 7, PointsToPageType = PageType.NULL})
                    .SetPageType(PageType.LEAF)
                    .SetPagePointer(leafPagePointers[1])
                    .SetParentPagePointer(rootPagePointer)
                    .Build(),
                new BTreePageBuilder<int>()
                    .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4})
                    .AddPointer(new BTreePagePointer<int>() {Index = 8, PointsToPageType = PageType.NULL})
                    .AddPointer(new BTreePagePointer<int>() {Index = 9, PointsToPageType = PageType.NULL})
                    .SetPageType(PageType.LEAF)
                    .SetPagePointer(leafPagePointers[2])
                    .SetParentPagePointer(rootPagePointer)
                    .Build()
            };
            
            var bTreeIO = Substitute.For<IBTreeIO<int>>();
            bTreeIO.GetPage(Arg.Any<IPagePointer<int>>()).ReturnsForAnyArgs(parentPage);
            return bTreeIO;
        }
    }
}