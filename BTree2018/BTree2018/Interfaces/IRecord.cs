using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces
{
    public interface IRecord<T> : IComparable where T : IComparable
    {
        IRecordPointer<T> RecordPointer { get; }
        T Value { get; }
        T[] ValueComponents { get; }
    }
}