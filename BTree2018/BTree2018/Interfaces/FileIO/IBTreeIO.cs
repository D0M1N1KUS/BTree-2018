using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreeIO<T> where T : IComparable
    {
        IPagePointer<T> WritePage(IPage<T> page);
        IPage<T> GetPage(IPagePointer<T> pointer);
        IPage<T> GetRootPage();
        IRecordPointer<T> WriteRecord(IRecord<T> record);
        IRecord<T> GetRecord(IRecordPointer<T> pointer);
    }
}