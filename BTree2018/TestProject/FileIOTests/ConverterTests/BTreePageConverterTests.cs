using System;
using System.Collections.Generic;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using NUnit.Framework;

namespace UnitTests.FileIOTests
{
    [TestFixture]
    public class BTreePageConverterTests
    {
        private readonly IBTreePagePointerConversion<double> PagePointerConverter = 
            new BTreePagePointerConverter<double>();
        private readonly IBTreeKeyConversion<double> KeyConverter = new BTreeKeyConverter<double>(sizeof(double));

        [Test]
        public void convertBytesToPage()
        {
            arrange(out var D, out var numberOfKeys, out var parentPagePointer, out var pointerToSelf, 
                out var currentPageType, out var keys, out var pointers, out var pageBytesList, out var expectedPage);

            var actualPage = new BTreePageConverter<double>(D, sizeof(double)).ConvertToPage(pageBytesList.ToArray(),
                pointerToSelf);
            
            Assert.AreEqual(expectedPage, actualPage);
        }

        [Test]
        public void convertPageToBytes()
        {
            arrange(out var D, out var numberOfKeys, out var parentPagePointer, out var pointerToSelf, 
                out var currentPageType, out var keys, out var pointers, out var expectedBytesList, out var inputPage);

            var actualBytesArray = new BTreePageConverter<double>(D, sizeof(double)).ConvertToBytes(inputPage);
            
            CollectionAssert.AreEqual(expectedBytesList.ToArray(), actualBytesArray);
        }

        private void arrange(out long D, out long numberOfKeys, out BTreePagePointer<double> parentPagePointer,
            out BTreePagePointer<double> pointerToSelf, out PageType currentPageType, out IKey<double>[] keys, out IPagePointer<double>[] pointers,
            out List<byte> pageBytesList, out IPage<double> expectedPage)
        {
            D = 2;
            numberOfKeys = 2;
            getLoosePointers(out parentPagePointer, out pointerToSelf);
            currentPageType = PageType.BRANCH;
            getKeysAndPointers(out keys, out pointers);
            pageBytesList = BuildPageBytesList(parentPagePointer, currentPageType, pointers, D, keys);
            expectedPage = buildPage(pointers, keys, D, pointerToSelf, parentPagePointer);
        }

        private static IPage<double> buildPage(IPagePointer<double>[] pointers, IKey<double>[] keys, long D, BTreePagePointer<double> pointerToSelf,
            BTreePagePointer<double> parentPagePointer)
        {
            var pointersForBuilder = new IPagePointer<double>[3];
            var keysForBuilder = new IKey<double>[2];
            Array.Copy(pointers, 0, pointersForBuilder, 0, 3);
            Array.Copy(keys, 0, keysForBuilder, 0, 2);
            var expectedPage = new BTreePageBuilder<double>((int) (2 * D))
                .SetPagePointer(pointerToSelf)
                .SetPageType(PageType.BRANCH)
                .SetParentPagePointer(parentPagePointer)
                .AddPointerRange(pointersForBuilder)
                .AddKeyRange(keysForBuilder)
                .Build();
            return expectedPage;
        }

        private List<byte> BuildPageBytesList(BTreePagePointer<double> parentPagePointer, PageType currentPageType, IPagePointer<double>[] pointers,
            long D, IKey<double>[] keys)
        {
            var pageBytesList = new List<byte>();
            pageBytesList.AddRange(BitConverter.GetBytes((long) 2));
            pageBytesList.AddRange(PagePointerConverter.ConvertToBytes(parentPagePointer));
            pageBytesList.Add((byte) currentPageType);
            pageBytesList.AddRange(PagePointerConverter.ConvertToBytes(pointers[0]));
            for (var i = 0; i < 2 * D; i++)
            {
                pageBytesList.AddRange(KeyConverter.ConvertToBytes(keys[i]));
                pageBytesList.AddRange(PagePointerConverter.ConvertToBytes(pointers[i + 1]));
            }

            return pageBytesList;
        }

        private static void getKeysAndPointers(out IKey<double>[] keys, out IPagePointer<double>[] pointers)
        {
            keys = new IKey<double>[]
            {
                new BTreeKey<double>() {RecordPointer = RecordPointer<double>.NullPointer, Value = 1.05},
                new BTreeKey<double>() {RecordPointer = RecordPointer<double>.NullPointer, Value = 1.1},
                BTreeKey<double>.NullKey,
                BTreeKey<double>.NullKey
            };
            pointers = new IPagePointer<double>[]
            {
                new BTreePagePointer<double>() {Index = 1, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<double>() {Index = 2, PointsToPageType = PageType.LEAF},
                new BTreePagePointer<double>() {Index = 3, PointsToPageType = PageType.LEAF},
                BTreePagePointer<double>.NullPointer,
                BTreePagePointer<double>.NullPointer
            };
        }

        private static void getLoosePointers(out BTreePagePointer<double> parentPagePointer, out BTreePagePointer<double> pointerToSelf)
        {
            parentPagePointer = new BTreePagePointer<double>() {Index = 123, PointsToPageType = PageType.ROOT};
            pointerToSelf = new BTreePagePointer<double>() {Index = 100, PointsToPageType = PageType.BRANCH};
        }
    }
}