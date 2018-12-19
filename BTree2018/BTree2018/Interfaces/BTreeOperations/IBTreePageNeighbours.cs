using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreePageNeighbours<T> where T : IComparable
    {
        bool GetNeighbours(IPage<T> page, out IPagePointer<T> leftNeighbourPtr, 
            out IPagePointer<T> rightNeighbourPtr, out IKey<T> parentKey);
    }
}