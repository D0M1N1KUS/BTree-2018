using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BTree2018.Interfaces.FileIO;
using NUnit.Framework;

namespace UnitTests.HelperClasses
{
    public class MemoryFileIO : IFileIO
    {
        private List<byte> fileInMemory;

        public long Length => fileInMemory.Count;
        
        public MemoryFileIO()
        {
            fileInMemory = new List<byte>();
        }

        public long FileLength { get; }
        public byte[] GetBytes(long begin, long n)
        {
            return fileInMemory.GetRange((int) begin, (int) n).ToArray();
        }

        public byte GetByte(long index)
        {
            return fileInMemory[(int) index];
        }

        public void WriteBytes(byte[] bytes, long begin)
        {
            while(fileInMemory.Count - 1 < begin)
                fileInMemory.Add(0);
            for (var i = 0; i < bytes.Length; i++)
            {
                if(fileInMemory.Count > i + begin)fileInMemory[i + (int) begin] = bytes[i];
                else fileInMemory.Add(bytes[i]);
            }
        }

        public void WriteZeros(long begin, long n)
        {
            WriteBytes(Enumerable.Repeat((byte)0, (int)n).ToArray(), begin);
        }
    }

    [TestFixture]
    public class MemoryFileIOTests
    {
        [Test]
        public void writeBytes_fileInMemoryIsEmpty()
        {
            var memoryFile = new MemoryFileIO();
            var expectedBytes = new byte[] {0, 0, 1, 2, 3, 4, 5};
            
            memoryFile.WriteBytes(new byte[]{1,2,3,4,5}, 2);
            var actualBytes = memoryFile.GetBytes(0, 7);

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }
    }
}