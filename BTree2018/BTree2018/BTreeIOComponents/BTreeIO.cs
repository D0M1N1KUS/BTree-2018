using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents
{
    public class BTreeIO<T> : IBTreeIO<T> where T : IComparable
    {
        public IFileIO FileIO;
        public IBTreePageFile<T> BTreePageFile;
        public IRecordFile<T> RecordFile;
        
        public IPagePointer<T> WritePage(IPage<T> page)
        {
            if (page.PagePointer.Equals(BTreePagePointer<T>.NullPointer))
                return BTreePageFile.AddNewPage(page);
            BTreePageFile.SetPage(page);
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
            return BTreePageFile.AddNewRootPage(page);
        }

        public IPage<T> GetPage(IPagePointer<T> pointer)
        {
            return BTreePageFile.PageAt(pointer);
        }

        public IPage<T> GetRootPage()
        {
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

        public void IncreaseTreeHeight(long value = 1)
        {
            BTreePageFile.SetTreeHeight(BTreePageFile.TreeHeight + value);
        }

        public void DecreaseTreeHeight(long value = 1)
        {
            BTreePageFile.SetTreeHeight(BTreePageFile.TreeHeight - value);
        }
    }
}