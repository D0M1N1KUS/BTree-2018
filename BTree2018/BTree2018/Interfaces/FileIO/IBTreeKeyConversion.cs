using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreeKeyConversion<T> where T : IComparable
    {
        IKey<T> ConvertToKey(byte[] bytes, int begin, int sizeOfValue);
        byte[] ConvertToBytes(IKey<T> key, int sizeOfValue);
    }
}