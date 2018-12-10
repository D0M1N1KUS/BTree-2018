using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreeIO<T> where T : IComparable
    {
        void WritePage(IPage<T> page);
        IPage<T> GetPage(IPagePointer<T> pointer);
        IPage<T> GetRootPage();
        void WriteRecord(IRecord<T> record);
        IRecord<T> GetRecord(IRecordPointer<T> pointer);
    }
}