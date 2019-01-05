namespace BTree2018.Interfaces.FileIO
{
    public interface IFileIO
    {
        byte[] GetBytes(long begin, long n);
        void WriteBytes(byte[] bytes, long begin);
    }
}