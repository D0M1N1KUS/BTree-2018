using System.IO;
using System.Linq;
using BTree2018.BTreeIOComponents.Basics;
using BTree2018.Interfaces.FileIO;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.FileIOTests.BasicIOTests
{
    [TestFixture]
    public class FileIOTests
    {
        private const string tempFilePath = "D:\\NonExistingFile.bin";

        ~FileIOTests()
        {
            if(File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
        
        
        [Test]
        public void writeZerosTest()
        {
            File.Create(tempFilePath).Close();
            var input = Substitute.For<IFileInput>();
            input.FilePath.Returns(tempFilePath);
            var output = Substitute.For<IFileOutput>();
            output.FilePath.Returns(tempFilePath);
            var FileIO = new FileIO(input, output, new FileInfo(tempFilePath));
            var expectedByteArray = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            
            FileIO.WriteZeros(0, 10);
            
            input.Received().WriteBytes(Arg.Is<byte[]>(x => x.SequenceEqual(expectedByteArray)), 0);
        }
    }
}