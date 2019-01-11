using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests.HelperClasses
{
    [TestFixture]
    public class BTreeIOTestFixture<T> : IBTreeIO<T> where T : IComparable
    {
        public int WritePageCalls { get; private set; } = 0;
        public int GetPageCalls { get; private set; } = 0;
        public int GetRootPageCalls { get; private set; } = 0;
        public int WriteRecordCalls { get; private set; } = 0;
        public int GetRecordCalls { get; private set; } = 0;
        
        public void AddToWritePage(IPagePointer<T> pagePointer) {returnValuesOfWritePage.Add(pagePointer);}
        public void AddToGetPage(IPage<T> page) {returnValuesOfGetPage.Add(page);}
        public void AddToGetRootPage(IPage<T> page) {returnValuesOfGetPage.Add(page);}
        public void AddToWriteRecord(IRecordPointer<T> recordPointer) {returnValuesOfWriteRecord.Add(recordPointer);}
        public void AddToGetRecord(IRecord<T> record) {returnValuesOfGetRecord.Add(record);}
        
        public List<IPage<T>> WrittenPage { get; private set; } = new List<IPage<T>>();
        public List<IPagePointer<T>> GottenPageFromPointer { get; private set; } = new List<IPagePointer<T>>();
        public List<IRecord<T>> WrittenRecord { get; private set; } = new List<IRecord<T>>();
        public List<IRecordPointer<T>> GottenRecordFromPointer { get; private set; } = new List<IRecordPointer<T>>();
        
        private List<IPagePointer<T>> returnValuesOfWritePage = new List<IPagePointer<T>>();
        private List<IPage<T>> returnValuesOfGetPage = new List<IPage<T>>();
        private List<IPage<T>> returnValuesOfGetRootPage = new List<IPage<T>>();
        private List<IRecordPointer<T>> returnValuesOfWriteRecord = new List<IRecordPointer<T>>();
        private List<IRecord<T>> returnValuesOfGetRecord = new List<IRecord<T>>();
        
        public IPagePointer<T> WritePage(IPage<T> page)
        {
            WrittenPage.Add(page);
            var returnValue = returnValuesOfWritePage.Count > WritePageCalls
                ? returnValuesOfWritePage[WritePageCalls]
                : null;
            WritePageCalls++;
            return returnValue;
        }

        public IPagePointer<T>[] WritePages(params IPage<T>[] pages)
        {
            WrittenPage.AddRange(pages);
            return null; // complicated plus idk if I'll ever use this return value.
        }

        public IPagePointer<T> WriteNewRootPage(IPage<T> page)
        {
            throw new NotImplementedException();
        }

        public IPage<T> GetPage(IPagePointer<T> pointer)
        {
            GottenPageFromPointer.Add(pointer);
            var returnValue = returnValuesOfGetPage.Count > GetPageCalls
                ? returnValuesOfGetPage[GetPageCalls]
                : null;
            GetPageCalls++;
            return returnValue;
        }

        public IPage<T> GetRootPage()
        {
            var returnValue = returnValuesOfGetRootPage.Count > GetRootPageCalls
                ? returnValuesOfGetRootPage[GetRootPageCalls]
                : null;
            GetRootPageCalls++;
            return returnValue;
        }

        public IRecordPointer<T> WriteRecord(IRecord<T> record)
        {
            WrittenRecord.Add(record);
            var returnValue = returnValuesOfWriteRecord.Count > WriteRecordCalls
                ? returnValuesOfWriteRecord[WriteRecordCalls]
                : null;
            WriteRecordCalls++;
            return returnValue;
        }

        public IRecord<T> GetRecord(IRecordPointer<T> pointer)
        {
            GottenRecordFromPointer.Add(pointer);
            var returnValue = returnValuesOfGetRecord.Count > GetRecordCalls
                ? returnValuesOfGetRecord[GetRecordCalls]
                : null;
            GetRecordCalls++;
            return returnValue;
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