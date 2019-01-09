namespace BTree2018.Interfaces.FileIO
{
    public interface IFileOutput
    {
        byte[] GetBytes(long begin, long n);
    }
}