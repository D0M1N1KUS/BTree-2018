using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeAdding<T> where T : IComparable<T>
    {
        void Add(IKey<T> key);
    }
}