using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreePointer
    {
        long fileIndex { get; }
        long pageIndex { get; }
    }
}