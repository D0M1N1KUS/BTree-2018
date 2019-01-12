using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BTree2018.BTreeIOComponents;
using BTree2018.BTreeIOComponents.Basics;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;
using NSubstitute;
using NUnit.Framework;
using UnitTests.HelperClasses;

namespace UnitTests.FileIOTests.FileClassesTests
{
    [TestFixture]
    public class BTreePageFileTests
    {
        private readonly string tempPageFile = "D:\\tempPageFile.pf";
        private readonly string tempMapFile = "D:\\tempMapFile.map";
        
        [Test]
        public void writeInitialValuesToFileTest()
        {
            var fileMap = Substitute.For<IFileBitmap>();
            var fileIO = Substitute.For<IFileIO>();
            var bTreePageFile = new BTreePageFile<int>(sizeof(int), 2)
            {
                PageConverter = new BTreePageConverter<int>(2, sizeof(int)),
                PagePointerConverter = new BTreePagePointerConverter<int>(),
                FileIO = fileIO, FileMap = fileMap
            };
            var expectedBytesInFile = getInitialPageFilePreambleBytesList(2, 1);

            bTreePageFile.WriteInitialValuesToFile();

            fileIO.Received().WriteBytes(Arg.Is<byte[]>(b => b.SequenceEqual(expectedBytesInFile.ToArray())), 0);
        }

        private static List<byte> getInitialPageFilePreambleBytesList(long d, long h)
        {
            var expectedBytesInFile = new List<byte>();
            expectedBytesInFile.AddRange(TypeConverter<int>.TypeTo64ByteString()); //TypeString
            expectedBytesInFile.AddRange(BitConverter.GetBytes(h));
            expectedBytesInFile.AddRange(BitConverter.GetBytes(d));
            expectedBytesInFile.AddRange(new BTreePagePointerConverter<int>().ConvertToBytes( //RootPointer
                BTreePagePointer<int>.NullPointer));
            return expectedBytesInFile;
        }

        [Test]
        public void addingNewPageTest_fileIsEmpty()
        {
            try
            {
                var fileMap = new MemoryFileMap(100);
                var fileIO = new MemoryFileIO();
                var bTreePageFile = new BTreePageFile<int>(sizeof(int), 2)
                {
                    PageConverter = new BTreePageConverter<int>(2, sizeof(int)), FileIO = fileIO, FileMap = fileMap,
                    PagePointerConverter = new BTreePagePointerConverter<int>()
                };
                var rootPage = getRootPage(2);
                bTreePageFile.WriteInitialValuesToFile();
                var rootPagePointer = bTreePageFile.AddNewRootPage(rootPage);
                getLeafPages(rootPagePointer, 2, out var leftPage, out var rightPage);
                rootPage.PagePointer = rootPagePointer;
                bTreePageFile.SetPage(leftPage);
                bTreePageFile.SetPage(rightPage);

                var returnedRootPage = bTreePageFile.PageAt(bTreePageFile.RootPage);
                var returnedLeftPage = bTreePageFile.PageAt(returnedRootPage.LeftPointerAt(0));
                var returnedRightPage = bTreePageFile.PageAt(returnedRootPage.RightPointerAt(0));
                var returnedRootPagePointer = bTreePageFile.RootPage;
                bTreePageFile.SetTreeHeight(2);
                var actualHeight = BitConverter.ToInt64(fileIO.GetBytes(bTreePageFile.LocationOfTreeHeight, 8), 0);

                Assert.AreEqual(rootPagePointer, returnedRootPagePointer);
                Assert.AreEqual(rootPage, returnedRootPage);
                Assert.AreEqual(leftPage, returnedLeftPage);
                Assert.AreEqual(rightPage, returnedRightPage);
                Assert.AreEqual(bTreePageFile.TreeHeight, actualHeight);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Console.WriteLine(Logger.GetLog());
                Assert.Fail();
            }
        }

        private BTreePage<int> getRootPage(long d)
        {
            return (BTreePage<int>)new BTreePageBuilder<int>((int)(2 * d))
                .SetPageType(PageType.ROOT)
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 3})
                .AddPointer(new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF})
                .AddPointer(new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF})
                .Build();
        }

        private void getLeafPages(IPagePointer<int> rootPagePointer, long d, out BTreePage<int> leftPage, out BTreePage<int> rightPage)
        {
            leftPage = (BTreePage<int>)new BTreePageBuilder<int>((int)(2 * d))
                .SetPageType(PageType.LEAF)
                .SetPagePointer(new BTreePagePointer<int>() {Index = 1, PointsToPageType = PageType.LEAF})
                .SetParentPagePointer(rootPagePointer)
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 1})
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 2})
                .Build();
            rightPage = (BTreePage<int>)new BTreePageBuilder<int>((int)(2 * d))
                .SetPageType(PageType.LEAF)
                .SetPagePointer(new BTreePagePointer<int>() {Index = 2, PointsToPageType = PageType.LEAF})
                .SetParentPagePointer(rootPagePointer)
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4})
                .AddKey(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 5})
                .Build();
        }
    }
}