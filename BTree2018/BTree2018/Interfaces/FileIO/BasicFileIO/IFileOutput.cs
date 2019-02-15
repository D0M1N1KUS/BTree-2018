namespace BTree2018.Interfaces.FileIO
{
    public interface IFileOutput
    {
        long Length { get; }
        string FilePath { get; }
        byte[] GetBytes(long begin, long n);
    }
}