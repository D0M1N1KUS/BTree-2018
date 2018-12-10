using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeSearching<T> where T : IComparable
    {
        IKey<T>[] FoundKeys { get; }
        IRecord<T>[] FoundRecords { get; }
        bool SearchForPair(IKey<T> key, IRecord<T> record);
    }
}