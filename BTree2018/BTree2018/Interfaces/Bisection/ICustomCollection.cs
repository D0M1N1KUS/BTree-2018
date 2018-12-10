using System;

namespace BTree2018.Interfaces.CustomCollection
{
    public interface ICustomCollection<T> where T : IComparable
    {
        long Length { get; }
        T this[long index] { get; }
    }
}