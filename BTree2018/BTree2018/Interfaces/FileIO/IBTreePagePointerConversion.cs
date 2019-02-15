using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreePagePointerConversion<T> where T : IComparable
    {
        IPagePointer<T> ConvertToPointer(byte[] bytes, int begin = 0);
        byte[] ConvertToBytes(IPagePointer<T> pointer);
    }
}