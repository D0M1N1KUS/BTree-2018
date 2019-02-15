using System;

namespace BTree2018.Interfaces.CustomCollection
{
    public interface IBisection<T> where T : IComparable
    {
        long LastIndex { get; }
        long GetClosestIndexTo(ICustomCollection<T> collection, T value);
    }
}