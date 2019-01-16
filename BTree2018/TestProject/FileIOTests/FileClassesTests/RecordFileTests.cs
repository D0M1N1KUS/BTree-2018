using System;
using System.Linq;
using System.Text;
using BTree2018.BTreeIOComponents;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace UnitTests.FileIOTests.FileClassesTests
{
    [TestFixture]
    public class RecordFileTests
    {
        private const long TYPE_STRING_PREAMBLE = 64;
        
        [Test]
        [Ignore("Bug in test")]
        public void getRecordFileTest()
        {
            var fileMap = Substitute.For<IFileBitmap>();
            fileMap.CurrentMapSize.Returns(1000);
            var fileIO = Substitute.For<IFileIO>();
            var pointer = new RecordPointer<short>() {Index = 123, PointerType = RecordPointerType.NOT_NULL};
            fileMap[pointer.Index].Returns(true);
            fileIO.GetBytes(TYPE_STRING_PREAMBLE + pointer.Index, sizeof(short) * 15)
                .Returns(new byte[]
                {
                    0b0000_0001, 0b0000_0000, 0b0000_0010, 0b0000_0000, 0b0000_0011, 0b0000_0000,
                    0b0000_0100, 0b0000_0000, 0b0000_0101, 0b0000_0000, 0b0000_0110, 0b0000_0000,
                    0b0000_0111, 0b0000_0000, 0b0000_1000, 0b0000_0000, 0b0000_1001, 0b0000_0000,
                    0b0000_1010, 0b0000_0000, 0b0000_1011, 0b0000_0000, 0b0000_1100, 0b0000_0000,
                    0b0000_1101, 0b0000_0000, 0b0000_1110, 0b0000_0000, 0b0000_1111, 0b0000_0000
                });
            fileIO.GetBytes(0, TYPE_STRING_PREAMBLE).Returns(TypeConverter<short>.TypeTo64ByteString());
            fileIO.GetBytes(Arg.Is<long>(x => x != pointer.Index + TYPE_STRING_PREAMBLE && x != 0), 
                    Arg.Is<long>(x => x != sizeof(short) && x != TYPE_STRING_PREAMBLE))
                .Throws(new ArgumentException("Unforeseen arguments"));
            var expectedRecord = new Record<short>(new short[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15},
                pointer);
            var recordFile = new RecordFile<short>(fileIO, fileMap, sizeof(short));
            
            var record = recordFile.GetRecord(pointer);
            
            Assert.AreEqual(record, expectedRecord);
            fileIO.Received().GetBytes(TYPE_STRING_PREAMBLE + pointer.Index, sizeof(short) * 15);
            var value = fileMap.Received()[pointer.Index];
        }
    }
}