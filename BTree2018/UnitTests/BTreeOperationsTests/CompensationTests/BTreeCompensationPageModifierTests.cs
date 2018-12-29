using System;
using System.Collections.Generic;
using BTree2018.BTreeOperations;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;
using NUnit.Framework;

namespace UnitTests.BTreeOperationsTests.CompensationTests
{
    [TestFixture]
    public class BTreeCompensationPageModifierTests
    {
        [Test]
        public void evenOutPagesTest_PagesOnePageIsFull()
        {
            try
            {
                createInputPages(out var fullPage, out var notFullPage, out var rootPage);
                createExpectedPages(out var expectedFullPage, out var expectedNotFullPage, out var expectedRootPage);
                var compensationModifier = new BTreeCompensationPageModifier<int>();

                compensationModifier.EvenOutKeys(ref rootPage, 0, ref fullPage, ref notFullPage);
                
                Console.WriteLine(@"rootPage {0}", rootPage);
                Console.WriteLine(@"fullPage {0}", fullPage);
                Console.WriteLine(@"notFullPage {0}", notFullPage);           
                Assert.IsTrue(rootPage.Equals(expectedRootPage));
                Assert.IsTrue(fullPage.Equals(expectedFullPage));
                Assert.IsTrue(notFullPage.Equals(expectedNotFullPage));
                Console.WriteLine(Logger.GetLog());
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail("Exception thrown or Assert failed.");
            }
        }

        #region evenOutPagesTest_PagesOnePageIsFull_SubRoutines

        private static void createInputPages(out IPage<int> fullPage, out IPage<int> notFullPage, 
            out IPage<int> rootPage)
        {
            fullPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 4, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.LEAF}
                }))
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            notFullPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 6},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 7},
                    null, null
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 6, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 7, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 8, PointsToPageType = PageType.LEAF},
                    null, null
                }))
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            rootPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 8},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 9},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 10}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 9, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 10, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 11, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 12, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 13, PointsToPageType = PageType.LEAF}
                }))
                .SetPageType(PageType.ROOT)
                .Build();
        }

        #endregion
        

        private static void createExpectedPages(out IPage<int> previouslyFullPage, out IPage<int> notFullPage, 
            out IPage<int> rootPage)
        {
            previouslyFullPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3},
                    null
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 4, PointsToPageType = PageType.LEAF},
                    null
                }))
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            notFullPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 6},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 7},
                    null
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 6, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 7, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 8, PointsToPageType = PageType.LEAF},
                    null
                }))
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            rootPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 8},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 9},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 10}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 9, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 10, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 11, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 12, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 13, PointsToPageType = PageType.LEAF}
                }))
                .SetPageType(PageType.ROOT)
                .Build();
        }

        [Test]
        public void evenOutPagesTest_PagesAreEven_NoChangesShouldOccur()
        {
            try
            {
                createEvenPages(out var actualParentPage, out var actualLeftPage, out var actualRightPage);
                createEvenPages(out var expectedParentPage, out var expectedLeftPage, out var expectedRightPage);
                var compensationModifier = new BTreeCompensationPageModifier<int>();

                compensationModifier.EvenOutKeys(ref actualLeftPage, 0, ref actualLeftPage, ref actualRightPage);

                Console.WriteLine(@"parentPage {0}", actualParentPage);
                Console.WriteLine(@"leftPage {0}", actualLeftPage);
                Console.WriteLine(@"rightPage {0}", actualRightPage);
                Assert.IsTrue(expectedLeftPage.Equals(actualLeftPage));
                Assert.IsTrue(expectedParentPage.Equals(actualParentPage));
                Assert.IsTrue(expectedRightPage.Equals(actualRightPage));
                Console.WriteLine(Logger.GetLog());
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail("Exception thrown or Assert failed.");
            }
        }

        private void createEvenPages(out IPage<int> parentPage, out IPage<int> leftPage, out IPage<int> rightPage)
        {
            parentPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 10},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 11},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 12}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 3, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 4, PointsToPageType = PageType.LEAF},
                    new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.LEAF}
                }))
                .SetPageType(PageType.ROOT)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            
            leftPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 5, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 6, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 7, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 8, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 9, PointsToPageType = PageType.NULL}
                }))
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
            
            rightPage = new BTreePageBuilder<int>().AddKeyRange(new List<IKey<int>>(new IKey<int>[]
                {
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 6},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 7},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 8},
                    new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 9}
                }))
                .AddPointerRange(new List<IPagePointer<int>>(new IPagePointer<int>[]
                {
                    new BTreePagePointer<int>() {Index = 10, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 11, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 12, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 13, PointsToPageType = PageType.NULL},
                    new BTreePagePointer<int>() {Index = 14, PointsToPageType = PageType.NULL}
                }))
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(BTreePagePointer<int>.NullPointer)
                .Build();
        }
        
        
    }
}