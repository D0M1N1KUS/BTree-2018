using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreeStructure<T> where T : IComparable
    {
        long H { get; }
        long D { get; }
        IPagePointer<T> Root { get; } //TODO: this may be a pointer
    }
}