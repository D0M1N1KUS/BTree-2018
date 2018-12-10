using System;

namespace BTree2018.Interfaces
{
    public interface IRecord<T> : IComparable<T> where T : IComparable<T>
    {
        T Value { get; }
        T[] ValueComponents { get; }
    }
}