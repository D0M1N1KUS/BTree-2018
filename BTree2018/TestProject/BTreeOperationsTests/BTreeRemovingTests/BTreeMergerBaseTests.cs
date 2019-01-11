using System;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.BTreeOperationsTests.BTreeRemovingTests
{
    [TestFixture]
    public class BTreeMergerBaseTests
    {
        [Test]
        public void mergePagesAndParentKey()
        {
            getInitialTestPagesForMerging(out var leftPage, out var parentKey, out var rightPage);
            var expectedMergedPage = getExpectedMergedPage();

            var actualMergedPage = BTreePageMergerBase<int>.MergePagesAndKey(leftPage, parentKey, rightPage);
            
            Console.WriteLine("Expected merged page\n{0}\n\nActual merged page\n{1}", expectedMergedPage, 
                actualMergedPage);
            Assert.AreEqual(expectedMergedPage, actualMergedPage);
        }

        #region mergePageAndParentKeySubroutines

        private void getInitialTestPagesForMerging(out IPage<int> leftPage, out IKey<int> parentKey, out IPage<int> rightPage)
        {
            leftPage = new BTreePageBuilder<int>(4)
                .SetPagePointer(new BTreePagePointer<int>(){Index = 10, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 1, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .AddPointer(new BTreePagePointer<int>(){Index = 11, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 12, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 13, PointsToPageType = PageType.LEAF})
                .Build();

            parentKey = new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3};
            
            rightPage = new BTreePageBuilder<int>(4)
                .SetPagePointer(new BTreePagePointer<int>(){Index = 20, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 2, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 4})
                .AddPointer(new BTreePagePointer<int>(){Index = 14, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 15, PointsToPageType = PageType.LEAF})
                .Build();
        }

        private IPage<int> getExpectedMergedPage()
        {
            return new BTreePageBuilder<int>()
                .SetPagePointer(new BTreePagePointer<int>(){Index = 10, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 1, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 3})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 4})
                .AddPointer(new BTreePagePointer<int>(){Index = 11, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 12, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 13, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 14, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>(){Index = 15, PointsToPageType = PageType.LEAF})
                .Build();
        }

        #endregion

        [Test]
        public void removeParentPageKeyAndInsetNewPointer()
        {
            getInitialTestPagesForNewPointerInsertion(out var parentPage, out var newPagePointer);
            var expectedPage = getExpectedPageAfterInsertion();

            var actualPage =
                BTreePageMergerBase<int>.RemoveParentPageKeyAndInsetNewPointer(parentPage, 1, newPagePointer);
            
            Console.WriteLine("Expected merged page\n{0}\n\nActual merged page\n{1}", expectedPage, actualPage);
            Assert.AreEqual(expectedPage, actualPage);
        }

        #region removeParentPageKeyAndInsetNewPointerSubroutines

        private void getInitialTestPagesForNewPointerInsertion(out IPage<int> parentPage,
            out IPagePointer<int> newPagePointer)
        {
            parentPage = new BTreePageBuilder<int>(4)
                .SetPagePointer(new BTreePagePointer<int>(){Index = 100, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 10, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 3})
                .AddPointer(new BTreePagePointer<int>(){Index = 1, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 2, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 3, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 4, PointsToPageType = PageType.BRANCH})
                .Build();

            newPagePointer = new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.BRANCH};
        }

        private IPage<int> getExpectedPageAfterInsertion()
        {
            return new BTreePageBuilder<int>(4)
                .SetPagePointer(new BTreePagePointer<int>(){Index = 100, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 10, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 3})
                .AddPointer(new BTreePagePointer<int>(){Index = 1, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 5, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 4, PointsToPageType = PageType.BRANCH})
                .Build();
        }

        #endregion

        [Test]
        public void removeParentPageKeyAndInsetNewPointer_RemoveSmallestKey()
        {
            getInitialTestPagesForNewPointerInsertion(out var parentPage, out var newPagePointer);
            var expectedPage = getExpectedPageAfterSmallestInsertion();

            var actualPage =
                BTreePageMergerBase<int>.RemoveParentPageKeyAndInsetNewPointer(parentPage, 0, newPagePointer);
            
            Console.WriteLine("Expected merged page\n{0}\n\nActual merged page\n{1}", expectedPage, actualPage);
            Assert.AreEqual(expectedPage, actualPage);
        }

        #region removeParentPageKeyAndInsetNewPointer_RemoveSmallestKey_Subroutines

        private IPage<int> getExpectedPageAfterSmallestInsertion()
        {
            return new BTreePageBuilder<int>(4)
                .SetPagePointer(new BTreePagePointer<int>(){Index = 100, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 10, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 3})
                .AddPointer(new BTreePagePointer<int>(){Index = 5, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 3, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 4, PointsToPageType = PageType.BRANCH})
                .Build();
        }

        #endregion

        [Test]
        public void removeParentPageKeyAndInsetNewPointer_RemoveBiggestKey()
        {
            getInitialTestPagesForNewPointerInsertion(out var parentPage, out var newPagePointer);
            var expectedPage = getExpectedPageAfterBiggestInsertion();

            var actualPage =
                BTreePageMergerBase<int>.RemoveParentPageKeyAndInsetNewPointer(parentPage, 2, newPagePointer);
            
            Console.WriteLine("Expected merged page\n{0}\n\nActual merged page\n{1}", expectedPage, actualPage);
            Assert.AreEqual(expectedPage, actualPage);
        }

        #region removeParentPageKeyAndInsetNewPointer_RemoveBiggestKey_Subroutines

        private IPage<int> getExpectedPageAfterBiggestInsertion()
        {
            return new BTreePageBuilder<int>(4)
                .SetPagePointer(new BTreePagePointer<int>(){Index = 100, PointsToPageType = PageType.BRANCH})
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(new BTreePagePointer<int>(){Index = 10, PointsToPageType = PageType.ROOT})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>(){RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .AddPointer(new BTreePagePointer<int>(){Index = 1, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 2, PointsToPageType = PageType.BRANCH})
                .AddPointer(new BTreePagePointer<int>(){Index = 5, PointsToPageType = PageType.BRANCH})
                .Build();
        }

        #endregion
        
    }
}