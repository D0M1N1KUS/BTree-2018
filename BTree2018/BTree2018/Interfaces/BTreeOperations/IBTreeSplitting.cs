using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeSplitting<T> where T : IComparable<T>
    {
        void Split(IPage<T> page);
        void Split(IPage<T> page, T keyToInsert);
    }
}