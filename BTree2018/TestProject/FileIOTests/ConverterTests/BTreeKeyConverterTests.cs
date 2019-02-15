using System;
using System.Collections.Generic;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.FileIOTests
{
    [TestFixture]
    public class BTreeKeyConverterTests
    {
        [Test]
        public void makeKeyFromBytes_PointerInKeyIsNotNull()
        {
            var bytesList = new List<byte>(sizeof(float) + sizeof(long));
            bytesList.AddRange(BitConverter.GetBytes((float)100));
            bytesList.AddRange(BitConverter.GetBytes((long)123));
            var expectedKey = new BTreeKey<float>()
            {
                Value = 100,
                RecordPointer = new RecordPointer<float>() {Index = 123, PointerType = RecordPointerType.NOT_NULL}
            };

            var actualKey = new BTreeKeyConverter<float>(sizeof(float)).ConvertToKey(bytesList.ToArray(), 0);
            
            Assert.AreEqual(expectedKey, actualKey);
        }

        [Test]
        public void makeKeyFromBytes_PointerInKeyIsNull()
        {
            var bytesList = new List<byte>(sizeof(double) + sizeof(long));
            bytesList.AddRange(BitConverter.GetBytes((double)100.05));
            bytesList.AddRange(BitConverter.GetBytes(RecordPointer<double>.NullPointer.Index));
            var expectedKey = new BTreeKey<double>() {Value = 100.05, RecordPointer = RecordPointer<double>.NullPointer};

            var actualKey = new BTreeKeyConverter<double>(sizeof(double)).ConvertToKey(bytesList.ToArray(), 0);
            
            Assert.AreEqual(expectedKey, actualKey);
        }

        [Test]
        public void getBytesFromKey_PointerInKeyIsNotNull()
        {
            var key = new BTreeKey<short>()
            {
                Value = 100,
                RecordPointer = new RecordPointer<short>() {Index = 123, PointerType = RecordPointerType.NOT_NULL}
            };
            var expectedByteList = new List<byte>(sizeof(short) + sizeof(long));
            expectedByteList.AddRange(BitConverter.GetBytes((short)100));
            expectedByteList.AddRange(BitConverter.GetBytes((long)123));
            var expectedBytes = expectedByteList.ToArray();

            var actualBytes = new BTreeKeyConverter<short>(sizeof(short)).ConvertToBytes(key);
            
            Assert.AreEqual(expectedBytes, actualBytes);
        }

        [Test]
        public void getBytesFromKey_PointerInKeyIsNull()
        {
            var key = new BTreeKey<short>()
            {
                Value = 100,
                RecordPointer = RecordPointer<short>.NullPointer
            };
            var expectedByteList = new List<byte>(sizeof(short) + sizeof(long));
            expectedByteList.AddRange(BitConverter.GetBytes((short)100));
            expectedByteList.AddRange(BitConverter.GetBytes(RecordPointer<short>.NullPointer.Index));
            var expectedBytes = expectedByteList.ToArray();

            var actualBytes = new BTreeKeyConverter<short>(sizeof(short)).ConvertToBytes(key);
            
            Assert.AreEqual(expectedBytes, actualBytes);
        }
    }
}