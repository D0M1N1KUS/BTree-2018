using System.IO;
using BTree2018.BTreeIOComponents.Basics;
using NUnit.Framework;

namespace UnitTests.FileIOTests.BasicIOTests
{
    [TestFixture]
    public class FileInputTests
    {
        private const string tempFilePath = "D:\\TestFile.bin";

        ~FileInputTests()
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
            var bytesToWrite1 = new byte[] {1, 2, 3, 4, 5};
            var bytesToWrite2 = new byte[] {6, 7, 8, 9, 10};
            using (var stream = File.Open(tempFilePath, FileMode.Open))
            {
                stream.Write(initialBytes, 0, initialBytes.Length);
            }
            
            fileInput.WriteBytes(bytesToWrite1, 3);
            fileInput.WriteBytes(bytesToWrite2, 8);
            var actualBytes = File.ReadAllBytes(tempFilePath);
            
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [Test]
        public void writeBytesAtEndOfFile_lastByteOfOriginalFileNeedsToBeOverwrittenAndThreeBytesNeedToBeAppended()
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

        [Test]
        public void appendBytes_appendingToEndOfFileAndFurtherThanEndOfFile()
        {
            File.Create(tempFilePath).Close();
            var fileInput = new FileInput(tempFilePath);
            var expectedBytes = new byte[] {1, 2, 3, 0, 0, 0, 4};
            
            fileInput.WriteBytes(new byte[]{1},0);
            fileInput.WriteBytes(new byte[]{2},1);
            fileInput.WriteBytes(new byte[]{3},2);
            fileInput.WriteBytes(new byte[]{4},6);

            var actualBytes = File.ReadAllBytes(tempFilePath);
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }
    }
}