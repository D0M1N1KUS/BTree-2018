using System;
using System.IO;
using BTree2018.BTreeIOComponents.Basics;
using NUnit.Framework;

namespace UnitTests.FileIOTests.BasicIOTests
{
    [TestFixture]
    public class FileOutputTests
    {
        private const string tempFilePath = "D:\\TestFile2.bin";
        
        ~FileOutputTests()
        {
            if(File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }

        [Test]
        public void readBytesFromFile_fileIsLargeEnough()
        {
            var bytesInFile = new byte[] {1, 2, 5, 6, 3, 4, 9, 10, 7, 8};
            createTestFile(bytesInFile);
            var fileOutput = new FileOutput(tempFilePath);
            var expectedBytes = new byte[] {5, 6, 3, 4, 9};
            var expectedBytes2 = new byte[] {1, 2, 5};
            var expectedBytes3 = new byte[] {10, 7, 8};

            var actualBytes = fileOutput.GetBytes(2, 5);
            var actualBytes2 = fileOutput.GetBytes(0, 3);
            var actualBytes3 = fileOutput.GetBytes(7, 3);
            
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
            CollectionAssert.AreEqual(expectedBytes2, actualBytes2);
            CollectionAssert.AreEqual(expectedBytes3, actualBytes3);
        }

        [Test]
        public void readBytesFromFile_ReadingPastEndOfFile()
        {
            var bytesInFile = new byte[]  {0, 1, 2, 3, 4};
            createTestFile(bytesInFile);
            var fileOutput = new FileOutput(tempFilePath);

            Assert.Throws<EndOfStreamException>(() => fileOutput.GetBytes(4, 2));
            Assert.Throws<EndOfStreamException>(() => fileOutput.GetBytes(5, 1));
        }

        private void createTestFile(byte[] bytes)
        {
            using (var stream = File.Create(tempFilePath))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }
    }
}