using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBtreeReorganizing<T> where T : IComparable
    {
        void Reorganize(IPage<T> modifiedLeafPage);
    }
}