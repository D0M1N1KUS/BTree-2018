using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeIOComponents
{
    public class BTreeIO<T> : IBTreeIO<T> where T : IComparable
    {
        public IBTreePageFile<T> BTreePageFile;
        public IRecordFile<T> RecordFile;

        public long D => BTreePageFile.D;
        public long H => BTreePageFile.TreeHeight;

        public IPagePointer<T> WritePage(IPage<T> page)
        {
            if (page.PagePointer.Equals(BTreePagePointer<T>.NullPointer))
                return BTreePageFile.AddNewPage(page);
            BTreePageFile.SetPage(page);
            Statistics.AddWrittenPages(1);
            return page.PagePointer;
        }

        public IPagePointer<T>[] WritePages(params IPage<T>[] pages)
        {
            var pagePointerList = new List<IPagePointer<T>>(pages.Length);
            foreach (var page in pages)
            {
                pagePointerList.Add(WritePage(page));
            }

            return pagePointerList.ToArray();
        }

        public IPagePointer<T> WriteNewRootPage(IPage<T> page)
        {
            FreePage(BTreePageFile.RootPage);
            Statistics.AddWrittenPages(1);
            return BTreePageFile.AddNewRootPage(page);
        }

        public IPage<T> GetPage(IPagePointer<T> pointer)
        {
            Statistics.AddReadPages(1);
            return BTreePageFile.PageAt(pointer);
        }

        public IPage<T> GetRootPage()
        {
            Statistics.AddReadPages(1);
            return BTreePageFile.PageAt(BTreePageFile.RootPage);
        }

        public IRecordPointer<T> WriteRecord(IRecord<T> record)
        {
            if (record.RecordPointer.Equals(RecordPointer<T>.NullPointer))
                return RecordFile.AddRecord(record);
            RecordFile.SetRecord(record);
            return record.RecordPointer;
        }

        public IRecord<T> GetRecord(IRecordPointer<T> pointer)
        {
            return RecordFile.GetRecord(pointer);
        }

        public void FreePage(IPage<T> page)
        {
            BTreePageFile.RemovePage(page);
        }

        public void FreePage(IPagePointer<T> pointer)
        {
            BTreePageFile.RemovePage(pointer);
        }

        public void FreeRecord(IRecord<T> record)
        {
            RecordFile.RemoveRecord(record);
        }

        public void FreeRecord(IRecordPointer<T> pointer)
        {
            RecordFile.RemoveRecord(pointer);
        }

        public void IncreaseTreeHeight(long value = 1)
        {
            BTreePageFile.SetTreeHeight(BTreePageFile.TreeHeight + value);
        }

        public void DecreaseTreeHeight(long value = 1)
        {
            BTreePageFile.SetTreeHeight(BTreePageFile.TreeHeight - value);
        }

        public void SetPageParentPointer(IPagePointer<T> targetPage, IPagePointer<T> parentPage)
        {
            BTreePageFile.SetPageParent(targetPage, parentPage);
        }
    }
}