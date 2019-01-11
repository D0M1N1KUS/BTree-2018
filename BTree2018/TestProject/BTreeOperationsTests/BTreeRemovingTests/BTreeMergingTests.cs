using System.Collections.Generic;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using UnitTests.HelperClasses;

namespace UnitTests.BTreeOperationsTests.BTreeRemovingTests
{
    [TestFixture]
    public class BTreeMergingTests
    {
        private readonly IRecordPointer<int> recNullptr = RecordPointer<int>.NullPointer;
        
        private List<IPagePointer<int>> listOfPagePointers;
        private List<IPage<int>> listOfPages;

        [Test]
        public void mergeTwoLeafPagesIntoRootPage()
        {
            preparePointers();
            preparePages();
            var expectedRootPage = getExpectedRootPage();
            var bTreeMerger = new BTreeMerger<int>();
            bTreeMerger.BTreeIO = getBTreeIO();
            bTreeMerger.BTreePageNeighbours = getBTreeNeighbours();

            bTreeMerger.Merge(listOfPages[2]);

            bTreeMerger.BTreeIO.Received().FreePage(listOfPages[0]);
            bTreeMerger.BTreeIO.Received().FreePage(listOfPages[2]);
            bTreeMerger.BTreeIO.Received().WriteNewRootPage(expectedRootPage);
        }

        #region mergeTwoLeafPagesIntoRootPageSubroutines

        private void preparePointers()
        {
            listOfPagePointers = new List<IPagePointer<int>>(3)
            {
                new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.ROOT},
                new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF}
            };
        }
        
        private void preparePages()
        {
            listOfPages = new List<IPage<int>>(3)
            {
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[0])
                    .SetPageType(PageType.ROOT)
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 3})
                    .AddPointer(listOfPagePointers[1])
                    .AddPointer(listOfPagePointers[2])
                    .Build(),
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[1])
                    .SetPageType(PageType.LEAF)
                    .SetParentPagePointer(listOfPagePointers[0])
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 1})
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 2})
                    .Build(),
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[1])
                    .SetPageType(PageType.LEAF)
                    .SetParentPagePointer(listOfPagePointers[0])
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 4})
                    .Build()
            };


        }

        private IBTreeIO<int> getBTreeIO()
        {
            var bTreeIOSubstitute = Substitute.For<IBTreeIO<int>>();
            for (var i = 0; i < listOfPagePointers.Count; i++)
            {
                bTreeIOSubstitute.GetPage(listOfPagePointers[i]).Returns(listOfPages[i]);
            }

            return bTreeIOSubstitute;
        }

        private IBTreePageNeighbours<int> getBTreeNeighbours()
        {
            var bTreeNeighbours = Substitute.For<IBTreePageNeighbours<int>>();
            bTreeNeighbours.GetNeighbours(listOfPages[2], out Arg.Any<IPagePointer<int>>(), 
                out Arg.Any<IPagePointer<int>>(),
                out Arg.Any<IKey<int>>(), 
                out Arg.Any<int>()).Returns(x =>
            {
                x[1] = listOfPagePointers[1];
                x[2] = BTreePagePointer<int>.NullPointer;
                x[3] = new BTreeKey<int>() {RecordPointer = recNullptr, Value = 3};
                x[4] = 0;
                return true;
            });
            bTreeNeighbours.ParentPage.Returns(listOfPages[0]);
            
            return bTreeNeighbours;
        }

        private IPage<int> getExpectedRootPage()
        {
            return new BTreePageBuilder<int>(4)
                .SetPagePointer(listOfPagePointers[0])
                .SetPageType(PageType.ROOT)
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 1})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 2})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 3})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 4})
                .Build();
        }

        #endregion

        [Test]
        public void mergeBranchPages_TheParentPageIsAlsoABranchPage()
        {
            preparePointers2();
            preparePages2();
            getExpectedModifiedPages(out var parentPage, out var mergedPage);
            var bTreeMerger = new BTreeMerger<int>();
            bTreeMerger.BTreeIO = getBTreeIO2();
            bTreeMerger.BTreePageNeighbours = getBTreeNeighbours2();
            
            bTreeMerger.Merge(listOfPages[2]);
            
            bTreeMerger.BTreeIO.Received().FreePage(listOfPages[2]);
            bTreeMerger.BTreeIO.Received().WritePages(parentPage, mergedPage);
        }

        #region mergeBranchPagesSubroutines

        private void preparePointers2()
        {
            listOfPagePointers = new List<IPagePointer<int>>(13);
            for (var i = 0; i < 13; i++)
            {
                listOfPagePointers.Add(new BTreePagePointer<int>() {Index = i + 1, PointsToPageType = PageType.BRANCH});
            }
        }
        
        private void preparePages2()
        {
            listOfPages = new List<IPage<int>>(3)
            {
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[0])
                    .SetPageType(PageType.BRANCH)
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 1})
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 4})
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 6})
                    .AddPointer(BTreePagePointer<int>.NullPointer)
                    .AddPointer(listOfPagePointers[1])
                    .AddPointer(listOfPagePointers[2])
                    .AddPointer(listOfPagePointers[3])
                    .Build(),
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[1])
                    .SetPageType(PageType.BRANCH)
                    .SetParentPagePointer(listOfPagePointers[0])
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 2})
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 3})
                    .AddPointer(listOfPagePointers[4])
                    .AddPointer(listOfPagePointers[5])
                    .AddPointer(listOfPagePointers[6])
                    .Build(),
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[2])
                    .SetPageType(PageType.BRANCH)
                    .SetParentPagePointer(listOfPagePointers[0])
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 5})
                    .AddPointer(listOfPagePointers[7])
                    .AddPointer(listOfPagePointers[8])
                    .Build(),
                new BTreePageBuilder<int>(4)
                    .SetPagePointer(listOfPagePointers[2])
                    .SetPageType(PageType.BRANCH)
                    .SetParentPagePointer(listOfPagePointers[0])
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 7})
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 8})
                    .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 9})
                    .AddPointer(listOfPagePointers[9])
                    .AddPointer(listOfPagePointers[10])
                    .AddPointer(listOfPagePointers[11])
                    .AddPointer(listOfPagePointers[12])
                    .Build()
            };


        }

        private IBTreeIO<int> getBTreeIO2()
        {
            var bTreeIOSubstitute = Substitute.For<IBTreeIO<int>>();
            for (var i = 0; i < 3; i++)
            {
                bTreeIOSubstitute.GetPage(listOfPagePointers[i]).Returns(listOfPages[i]);
            }

            return bTreeIOSubstitute;
        }

        private IBTreePageNeighbours<int> getBTreeNeighbours2()
        {
            var bTreeNeighbours = Substitute.For<IBTreePageNeighbours<int>>();
            bTreeNeighbours.GetNeighbours(listOfPages[2], out Arg.Any<IPagePointer<int>>(), 
                out Arg.Any<IPagePointer<int>>(),
                out Arg.Any<IKey<int>>(), 
                out Arg.Any<int>()).Returns(x =>
            {
                x[1] = listOfPagePointers[1];
                x[2] = listOfPagePointers[3];
                x[3] = new BTreeKey<int>() {RecordPointer = recNullptr, Value = 4};
                x[4] = 1;
                return true;
            });
            bTreeNeighbours.ParentPage.Returns(listOfPages[0]);
            
            return bTreeNeighbours;
        }

        private void getExpectedModifiedPages(out IPage<int> parentPage, out IPage<int> mergedPage)
        {
            parentPage = new BTreePageBuilder<int>(4)
                .SetPagePointer(listOfPagePointers[0])
                .SetPageType(PageType.BRANCH)
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 1})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 6})
                .AddPointer(BTreePagePointer<int>.NullPointer)
                .AddPointer(listOfPagePointers[1])
                .AddPointer(listOfPagePointers[3])
                .Build();

            mergedPage = new BTreePageBuilder<int>(4)
                .SetPagePointer(listOfPagePointers[1])
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(listOfPagePointers[0])
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 2})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 3})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 4})
                .AddKey(new BTreeKey<int>() {RecordPointer = recNullptr, Value = 5})
                .AddPointer(listOfPagePointers[4])
                .AddPointer(listOfPagePointers[5])
                .AddPointer(listOfPagePointers[6])
                .AddPointer(listOfPagePointers[7])
                .AddPointer(listOfPagePointers[8])
                .Build();
        }

        #endregion
        
    }
}