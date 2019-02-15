using System;
using BTree2018.Interfaces.FileIO;

namespace UnitTests.HelperClasses
{
    public class MemoryFileMap : IFileBitmap
    {
        private bool[] map;
        
        public byte CachedMapPiece { get; }
        public long CurrentMapSize { get; }

        public MemoryFileMap(long size)
        {
            map = new bool[size];
        }

        public bool this[long index]
        {
            get => map[index];
            set => map[index] = value;
        }

        public long GetNextFreeIndex()
        {
            for (var i = 0; i < map.Length; i++)
            {
                if (!map[i]) return i;
            }
            throw new Exception("No more free space :/");
        }

        public void Flush() { }
    }
}