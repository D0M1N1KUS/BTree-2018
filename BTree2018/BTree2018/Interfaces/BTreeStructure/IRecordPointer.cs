using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IRecordPointer<T> where T : IComparable<T>
    {
        T GetRecord();
    }
}