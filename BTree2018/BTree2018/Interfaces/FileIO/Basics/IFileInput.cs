namespace BTree2018.Interfaces.FileIO
{
    public interface IFileInput
    {
        void WriteBytes(byte[] bytes, long begin);
    }
}