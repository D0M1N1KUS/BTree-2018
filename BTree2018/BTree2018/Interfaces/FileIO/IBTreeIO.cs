using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreeIO<T> where T : IComparable
    {
        long D { get; }
        long H { get; }
        
        IPagePointer<T> WritePage(IPage<T> page); //TODO: A page with a null-self-pointer is a new page
        IPagePointer<T>[] WritePages(params IPage<T>[] pages);
        IPagePointer<T> WriteNewRootPage(IPage<T> page);
        
        IPage<T> GetPage(IPagePointer<T> pointer);
        IPage<T> GetRootPage();
        IRecordPointer<T> WriteRecord(IRecord<T> record);
        IRecord<T> GetRecord(IRecordPointer<T> pointer);
        
        void FreePage(IPage<T> page);
        void FreePage(IPagePointer<T> pointer);
        void FreeRecord(IRecord<T> record);
        void FreeRecord(IRecordPointer<T> pointer);

        void IncreaseTreeHeight(long value = 1);
        void DecreaseTreeHeight(long value = 1);

        void SetPageParentPointer(IPagePointer<T> targetPage, IPagePointer<T> parentPage);
    }
}