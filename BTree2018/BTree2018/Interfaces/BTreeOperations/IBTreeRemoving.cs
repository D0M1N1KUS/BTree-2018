using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeRemoving<T> where T : IComparable<T>
    {
        void RemoveKey(IKey<T> key);
    }
}