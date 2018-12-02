using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeMerging<T> where T : IComparable
    {
        void Merge(IPage<T> pageWithShortage, IPage<T> parentPage, IPage<T> pageWithoutShortage);
    }
}