using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeLeafKeyRemoval<T> where T : IComparable
    {
        IKey<T> RemoveBiggestKey(IPage<T> beginningPage, out IPage<T> modifiedLeafPage);
        IKey<T> RemoveSmallestKey(IPage<T> beginningPage, out IPage<T> modifiedLeafPage);
    }
}