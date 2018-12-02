using System;

namespace BTree2018.Interfaces
{
    public interface IRecord<T> : IComparable where T : IComparable
    {
        T Value { get; }
        T[] ValueComponents { get; }
    }
}