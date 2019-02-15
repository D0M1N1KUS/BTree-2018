using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreeStructure<T> where T : IComparable
    {
        //TODO: prolly remove this stuff
        long H { get; }
        long D { get; }
        IPagePointer<T> Root { get; }
    }
}