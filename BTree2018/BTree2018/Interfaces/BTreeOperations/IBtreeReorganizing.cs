using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBtreeReorganizing<T> where T : IComparable
    {
        IPage<T> Reorganize(IPage<T> modifiedLeafPage);
        IPage<T> Reorganize(IPage<T> fullPage, IKey<T> keyToInsert);
    }
}