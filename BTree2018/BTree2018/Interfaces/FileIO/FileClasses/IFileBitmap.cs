namespace BTree2018.Interfaces.FileIO
{
    public interface IFileBitmap
    {
        byte CachedMapPiece { get; }
        long CurrentMapSize { get; }
        
        bool this[long index] { get; set; }
        long GetNextFreeIndex();
        void Flush();
    }
}