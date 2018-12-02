using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IValuePointer<T> where T : IComparable
    {
        T GetValue();
    }
}