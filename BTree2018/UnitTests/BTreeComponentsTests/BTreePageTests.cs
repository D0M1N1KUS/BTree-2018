using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.BTreeComponentsTests
{
    [TestFixture]
    public class BTreePageTests
    {
        [Test]
        public void bTreePageEqualsTest_BothPagesAreEqual_EqualsShouldReturnTrue()
        {
            var keys = new IKey<int>[]
            {
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2}
            };
            var pointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL}
            };
            var page1 = new BTreePage<int>()
            {
                KeysInPage = 2, PageType = PageType.ROOT, Keys = keys, Pointers = pointers,
                ParentPage = BTreePagePointer<int>.NullPointer
            };
            var page2 = new BTreePage<int>()
            {
                KeysInPage = 2, PageType = PageType.ROOT, Keys = keys, Pointers = pointers,
                ParentPage = BTreePagePointer<int>.NullPointer
            };

            var iPage1 = page1 as IPage<int>;
            var iPage2 = new BTreePageBuilder<int>().ClonePage(page2).Build();
            
            Assert.AreEqual(page1, page2);
            Assert.IsTrue(page1.Equals(page2));
            Assert.AreEqual(iPage1, iPage2);
            Assert.IsTrue(iPage1.Equals(iPage2));
            Assert.AreEqual(iPage1, page1);
            Assert.IsTrue(iPage1.Equals(page1));
        }

        [Test]
        public void bTreePageEqualsTest_PageValuesAreDifferent_EqualsShouldReturnFalse()
        {
            var keys = new IKey<int>[]
            {
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2}
            };
            var keys2 = new IKey<int>[]
            {
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3}
            };
            var pointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL}
            };
            var page1 = new BTreePage<int>()
            {
                KeysInPage = 2, PageType = PageType.ROOT, Keys = keys, Pointers = pointers,
                ParentPage = BTreePagePointer<int>.NullPointer
            };
            var page2 = new BTreePage<int>()
            {
                KeysInPage = 2, PageType = PageType.ROOT, Keys = keys2, Pointers = pointers,
                ParentPage = BTreePagePointer<int>.NullPointer
            };
            
            var iPage1 = page1 as IPage<int>;
            var iPage2 = new BTreePageBuilder<int>().ClonePage(page2).Build();
            
            Assert.AreNotEqual(page1, page2);
            Assert.IsFalse(page1.Equals(page2));
            Assert.AreNotEqual(iPage1, iPage2);
            Assert.IsFalse(iPage1.Equals(iPage2));
        }
        
        [Test]
        public void bTreePageEqualsTest_PagePointersAreDifferent_EqualsShouldReturnFalse()
        {
            var keys = new IKey<int>[]
            {
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2}
            };
            var pointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL}
            };
            var pointers2 = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL}
            };
            var page1 = new BTreePage<int>()
            {
                KeysInPage = 2, PageType = PageType.ROOT, Keys = keys, Pointers = pointers,
                ParentPage = BTreePagePointer<int>.NullPointer
            };
            var page2 = new BTreePage<int>()
            {
                KeysInPage = 2, PageType = PageType.ROOT, Keys = keys, Pointers = pointers2,
                ParentPage = BTreePagePointer<int>.NullPointer
            };
            
            var iPage1 = page1 as IPage<int>;
            var iPage2 = new BTreePageBuilder<int>().ClonePage(page2).Build();
            
            Assert.AreNotEqual(page1, page2);
            Assert.IsFalse(page1.Equals(page2));
            Assert.AreNotEqual(iPage1, iPage2);
            Assert.IsFalse(iPage1.Equals(iPage2));
        }
    }
}