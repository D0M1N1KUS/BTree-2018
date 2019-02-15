using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreePageConversion<T> where T : IComparable
    {
        IPage<T> ConvertToPage(byte[] bytes, IPagePointer<T> pointerToPage);
        byte[] ConvertToBytes(IPage<T> page);
    }
}