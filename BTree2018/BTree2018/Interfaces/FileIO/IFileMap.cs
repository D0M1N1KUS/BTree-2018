namespace BTree2018.Interfaces.FileIO
{
    public interface IFileMap
    {
        long GetNextFreeIndex();
        bool IsFree(long index);
        void SetAsFree(long index);
        void SetAsTaken(long index);
    }
}