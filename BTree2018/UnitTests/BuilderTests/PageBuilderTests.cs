using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.BuilderTests
{
    [TestFixture]
    public class PageBuilderTests
    {
        [Test]
        public void copyPageTest_BothPagesShouldBeEqual()
        {
            var keys = new IKey<int>[]
            {
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1},
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2},
                new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3}
            };
            var pointers = new IPagePointer<int>[]
            {
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL},
                new BTreePagePointer<int>() {Index = 0, PointsToPageType = PageType.NULL}
            };
            var page = new BTreePage<int>()
            {
                KeysInPage = 3, PageType = PageType.ROOT, Keys = keys, Pointers = pointers,
                ParentPage = BTreePagePointer<int>.NullPointer
            };

            var pageCopy = new BTreePageBuilder<int>().ClonePage(page).Build();
            
            Assert.AreEqual(pageCopy, page);
        }
//        
//        [Test]
//        public void tryToBuildPage_BuildingPageWithoutPointers_()
    }
}