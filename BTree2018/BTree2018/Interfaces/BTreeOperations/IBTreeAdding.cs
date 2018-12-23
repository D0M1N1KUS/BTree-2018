using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeAdding<T> where T : IComparable
    {
        IPage<T> Add(IKey<T> key);
        IPage<T> InsertKeyIntoPage(IPage<T> page, IKey<T> key, IPagePointer<T> rightPointerOfKey = null);
    }
}