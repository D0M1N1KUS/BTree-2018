namespace BTree2018.Interfaces.FileIO
{
    public interface IFileIO
    {
        byte[] GetBytes(long begin, long n);
        byte GetByte(long index);
        void WriteBytes(byte[] bytes, long begin);
        void WriteZeros(long begin, long n);
    }
}