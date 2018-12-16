using System;
using BTree2018.BTreeStructure;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IKey<T> : IComparable<IKey<T>> where T : IComparable
    {
        T Value { get; }
        IRecordPointer<T> RecordPointer { get; }//TODO: check if this can be applied to the record class
        IPagePointer<T> LeftPagePointer { get; }
        IPagePointer<T> RightPagePointer { get; }
    }
}