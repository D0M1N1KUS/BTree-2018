using System;
using System.Collections.Generic;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.FileIOTests
{
    [TestFixture]
    public class BTreePagePointerConverterTests
    {
        [Test]
        public void convertPointerToBytes()
        {
            var pointer = new BTreePagePointer<int>() {Index = 123, PointsToPageType = PageType.ROOT};
            var expectedBytesList = new List<byte>(sizeof(long) + 1);
            expectedBytesList.AddRange(BitConverter.GetBytes((long)123));
            expectedBytesList.Add(1);
            var expectedBytes = expectedBytesList.ToArray();

            var actualBytes = new BTreePagePointerConverter<int>().ConvertToBytes(pointer);
            
            Assert.AreEqual(expectedBytes, actualBytes);
        }

        [Test]
        public void convertBytesToPointer()
        {
            var bytesList = new List<byte>(sizeof(long) + 1);
            bytesList.AddRange(BitConverter.GetBytes((long)1234));
            bytesList.Add(2);
            var expectedPointer = new BTreePagePointer<int>() {Index = 1234, PointsToPageType = PageType.BRANCH};

            var actualPointer = new BTreePagePointerConverter<int>().ConvertToPointer(bytesList.ToArray());
            
            Assert.AreEqual(expectedPointer, actualPointer);
        }
    }
}