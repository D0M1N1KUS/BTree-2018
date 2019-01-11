using System.IO;
using BTree2018.BTreeIOComponents.Basics;
using NUnit.Framework;

namespace UnitTests.FileIOTests.BasicIOTests
{
    [TestFixture]
    public class FileOutputTests
    {
        private const string tempFilePath = "D:\\TestFile.bin";

        ~FileOutputTests()
        {
            if(File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
        
        [Test]
        public void writeBytesMidFile()
        {
            File.Create(tempFilePath).Close();
            var fileInput = new FileInput(tempFilePath);
            var initialBytes = new byte[] {0, 0, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0};
            var expectedBytes = new byte[] {0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0};
            var bytesToWrite = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            using (var stream = File.Open(tempFilePath, FileMode.Open))
            {
                stream.Write(initialBytes, 0, initialBytes.Length);
            }
            
            fileInput.WriteBytes(bytesToWrite, 3);
            var actualBytes = File.ReadAllBytes(tempFilePath);
            
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [Test]
        public void writeBytesAtEndOfFile_lastByteOfOriginalFileNeedsToBeOverwritten()
        {
            File.Create(tempFilePath).Close();
            var fileInput = new FileInput(tempFilePath);
            var initialBytes = new byte[] {0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1};
            var expectedBytes = new byte[] {0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0};
            var bytesToWrite = new byte[] {10, 0, 0, 0};
            using (var stream = File.Open(tempFilePath, FileMode.Open))
            {
                stream.Write(initialBytes, 0, initialBytes.Length);
            }
            
            fileInput.WriteBytes(bytesToWrite, 12);
            var actualBytes = File.ReadAllBytes(tempFilePath);
            
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }
    }
}