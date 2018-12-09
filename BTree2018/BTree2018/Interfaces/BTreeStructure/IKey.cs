using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IKey<T> where T : IComparable<T>
    {
        T Value { get; }
        long N { get; }
        IRecord<T> Record { get; }//TODO: check if this can be applied to the record class
        IPagePointer<T> LeftPagePointer { get; }
        IPagePointer<T> RightPagePointer { get; }
    }
}