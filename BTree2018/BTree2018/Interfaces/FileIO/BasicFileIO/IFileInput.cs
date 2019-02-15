namespace BTree2018.Interfaces.FileIO
{
    public interface IFileInput
    {
        long Length { get; }
        string FilePath { get; }
        void WriteBytes(byte[] bytes, long begin);
    }
}