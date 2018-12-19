using System;
using System.Collections.Generic;
using BTree2018.Enums;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.CustomCollection;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeSearcher<T> : IBTreeSearching<T> where T : IComparable
    {
        private IRecord<T> foundRecord;
        
        public IBTreeIO<T> BTreeIO;
        public IBisection<T> BisectSearch;

        public IKey<T> FoundKey { get; private set; }
        public IPage<T> FoundPage { get; private set; }
        public IRecord<T> FoundRecord
        {
            get
            {
                if (foundRecord != null) return foundRecord;
                if(FoundKey == null) throw new Exception("Can't find record! No key was found yet.");
                foundRecord = BTreeIO.GetRecord(FoundKey.RecordPointer);
                return foundRecord;
            }
            private set => foundRecord = value;
        }

        public bool SearchForPair(IKey<T> key)
        {
            FoundKey = null;
            FoundRecord = null;
            FoundPage = null;
            
            return SearchForPair(key, BTreeIO.GetRootPage());
        }

        private bool SearchForPair(IKey<T> key, IPage<T> beginningPage)
        {
            var currentPage = beginningPage;
            while (currentPage.PageType != PageType.NULL)
            {
                var index = BisectSearch.GetClosestIndexTo(currentPage, key.Value);
                if (key.Equals(currentPage.KeyAt(index)))
                {
                    FoundKey = currentPage.KeyAt(index);
                    FoundPage = currentPage;
                    return true;
                }

                if (currentPage.PageType == PageType.LEAF) return false;
                if (key.CompareTo(currentPage.KeyAt(index)) == (int) Comparison.LESS)
                    currentPage = getRightPage(currentPage, index);
                else
                    currentPage = getLeftPage(currentPage, index);
            }

            return false;
        }

        private IPage<T> getRightPage(IPage<T> currentPage, long index)
        {
            return BTreeIO.GetPage(currentPage.RightPointerAt(index));
        }

        private IPage<T> getLeftPage(IPage<T> currentPage, long index)
        {
            return BTreeIO.GetPage(currentPage.LeftPointerAt(index));
        }
        
        private IPage<T> getPage(IPagePointer<T> pagePointer)
        {
            return BTreeIO.GetPage(pagePointer);
        }
    }
}