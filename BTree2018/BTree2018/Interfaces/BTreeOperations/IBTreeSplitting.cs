using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeSplitting<T> where T : IComparable
    {
        IPage<T> Split(IPage<T> page);
        IPage<T> Split(IPage<T> page, IKey<T> keyToInsert);
    }
}