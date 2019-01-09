using BTree2018.BTreeIOComponents;
using BTree2018.Interfaces.FileIO;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace UnitTests.FileIOTests.FileClassesTests
{
    [TestFixture]
    public class FileMapTests
    {
        [Test]
        public void getBitAtIndex()
        {
            const long FILE_INFO_LENGTH = 8;
            var fileIO = Substitute.For<IFileIO>();
            fileIO.GetByte(0 + FILE_INFO_LENGTH).Returns((byte) 0b1111_1111);
            fileIO.GetByte(1 + FILE_INFO_LENGTH).Returns((byte) 0b0000_0000);
            fileIO.GetBytes(0, 8).Returns(new byte[]
            {
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000,
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000
            });
            var fileMap = new FileMap(fileIO);
            var expectedBits = new bool[]
            {
                true, true, true, true, true, true, true, true,
                false, false, false, false, false, false, false, false
            };

            for (var i = 0; i < 16; i++)
            {
                fileMap[i] = expectedBits[i];
            }

            var actualBits = new bool[16];
            for (var i = 0; i < 16; i++)
            {
                actualBits[i] = fileMap[i];
            }
            
            CollectionAssert.AreEqual(expectedBits, actualBits);
        }

        [Test]
        public void setBits()
        {
            const long FILE_INFO_LENGTH = 4;
            var fileIO = Substitute.For<IFileIO>();
            fileIO.GetBytes(0, 8).Returns(new byte[]
            {
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000,
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000
            });
            var fileMap = new FileMap(fileIO);
            byte expectedFirstCachedMap = 0b1010_1010;
            byte expectedSecondCachedMap = 0b0000_1111;

            for (var i = 0; i < 8; i++)
            {
                fileMap[i] = i % 2 == 0;
            }
            var actualFirstCachedMapPiece = fileMap.CachedMapPiece;
            
            for(var i = 0; i < 8; i++)
            {
                fileMap[i + 8] = i > 3;
            }
            var actualSecondCachedMapPiece = fileMap.CachedMapPiece;

            Assert.AreEqual(expectedFirstCachedMap, actualFirstCachedMapPiece);
            Assert.AreEqual(expectedSecondCachedMap, actualSecondCachedMapPiece);
        }

        [Test]
        public void mapsSizePropertyTest()
        {
            var fileIO = Substitute.For<IFileIO>();
            fileIO.GetBytes(0, 8).Returns(new byte[]
            {
                0b0000_1000, 0b0000_0000, 0b0000_0000, 0b0000_0000,
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000
            });
            var fileMap = new FileMap(fileIO);
            const int expectedMapSize1 = 8;
            const int expectedMapSize2 = 16;

            for (var i = 0; i < 8; i++)
            {
                fileMap[i] = true;
            }
            var actualMapSize1 = fileMap.CurrentMapSize;

            for (var i = 8; i < 13; i++)
            {
                fileMap[i] = false;
            }
            var actualMapSize2 = fileMap.CurrentMapSize;
            
            Assert.AreEqual(expectedMapSize1, actualMapSize1);
            Assert.AreEqual(expectedMapSize2, actualMapSize2);
        }

        [Test]
        public void getNextFreeIndexTest_CompletelyNewObjectAndFile()
        {
            const long FILE_INFO_LENGTH = 8;
            var fileIO = Substitute.For<IFileIO>();
            fileIO.GetByte(Arg.Any<long>()).Returns((byte) 0b0000_0000);
            fileIO.GetBytes(0, 8).Returns(new byte[]
            {
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000,
                0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000
            });
            var expectedIndexes = new long[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            var actualIndexes = new long[10];
            
            var fileMap = new FileMap(fileIO);
            for (var i = 0; i < 10; i++)
            {
                actualIndexes[i] = fileMap.GetNextFreeIndex();
                fileMap[actualIndexes[i]] = true;
            }
            
            CollectionAssert.AreEqual(expectedIndexes, actualIndexes);
            fileIO.Received().WriteBytes(Arg.Any<byte[]>(), 0 + FILE_INFO_LENGTH);
        }
    }
}