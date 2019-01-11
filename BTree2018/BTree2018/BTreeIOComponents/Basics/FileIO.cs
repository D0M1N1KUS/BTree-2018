using System.Linq;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents.Basics
{
    public class FileIO : IFileIO
    {
        private IFileInput input;
        private IFileOutput output;

        public FileIO(IFileInput fileInput, IFileOutput fileOutput)
        {
            input = fileInput;
            output = fileOutput;
        }

        public FileIO(string filePath)
        {
            input = new FileInput(filePath);
            output = new FileOutput(filePath);
        }
        
        public byte[] GetBytes(long begin, long n)
        {
            return output.GetBytes(begin, n);
        }

        public byte GetByte(long index)
        {
            
            return output.GetBytes(index, 1)[0];
        }

        public void WriteBytes(byte[] bytes, long begin)
        {
            input.WriteBytes(bytes, begin);
        }

        public void WriteZeros(long begin, long n)
        {
            input.WriteBytes(Enumerable.Repeat((byte)0, (int)n).ToArray(), begin);
        }
    }
}