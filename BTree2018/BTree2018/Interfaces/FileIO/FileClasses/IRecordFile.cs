using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IRecordFile<T> where T : IComparable
    {
        IRecord<T> this[IRecordPointer<T> pointer] { get; set; }
        IRecord<T> GetRecord(IRecordPointer<T> pointer);
        void SetRecord(IRecord<T> record);
        IRecordPointer<T> AddRecord(IRecord<T> record);
        void RemoveRecord(IRecord<T> record);
        void RemoveRecord(IRecordPointer<T> pointer);
    }
}