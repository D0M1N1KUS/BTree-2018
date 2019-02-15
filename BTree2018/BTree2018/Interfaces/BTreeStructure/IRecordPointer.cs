using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IRecordPointer<T> where T : IComparable
    {
        RecordPointerType PointerType { get; }
        long Index { get; }
    }

    public enum RecordPointerType
    {
        NULL,
        NOT_NULL
    }
}