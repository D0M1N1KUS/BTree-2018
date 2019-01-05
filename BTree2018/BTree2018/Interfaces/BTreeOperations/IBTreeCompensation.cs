using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeCompensation<T> where T : IComparable
    {
        bool Compensate(IPage<T> page, IKey<T> keyToAdd);
        bool Compensate(IPage<T> page);
    }
}