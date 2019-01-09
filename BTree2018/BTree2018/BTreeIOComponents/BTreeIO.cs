using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents
{
    public class BTreeIO<T> : IBTreeIO<T> where T : IComparable
    {
        public IFileIO FileIO;
        
        public IPagePointer<T> WritePage(IPage<T> page)
        {
            throw new NotImplementedException();
        }

        public IPagePointer<T>[] WritePages(params IPage<T>[] pages)
        {
            throw new NotImplementedException();
        }

        public IPagePointer<T> WriteNewRootPage(IPage<T> page)
        {
            throw new NotImplementedException();
        }

        public IPage<T> GetPage(IPagePointer<T> pointer)
        {
            throw new NotImplementedException();
        }

        public IPage<T> GetRootPage()
        {
            throw new NotImplementedException();
        }

        public IRecordPointer<T> WriteRecord(IRecord<T> record)
        {
            throw new NotImplementedException();
        }

        public IRecord<T> GetRecord(IRecordPointer<T> pointer)
        {
            throw new NotImplementedException();
        }

        public void FreePage(IPage<T> page)
        {
            throw new NotImplementedException();
        }

        public void FreePage(IPagePointer<T> pointer)
        {
            throw new NotImplementedException();
        }

        public void IncreaseTreeHeight(long value = 1)
        {
            throw new NotImplementedException();
        }

        public void DecreaseTreeHeight(long value = 2)
        {
            throw new NotImplementedException();
        }
    }
}