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
        private IPage<T> currentPage;
        private List<IRecord<T>> foundRecords = new List<IRecord<T>>();
        
        private IPage<T> BiggerPageOfFoundKey => 
            BTreeIO.GetPage(currentPage.KeyAt(BisectSearch.LastIndex).RightPagePointer);
        private IPage<T> SmallerPageOfFoundKey => 
            BTreeIO.GetPage(currentPage.KeyAt(BisectSearch.LastIndex).LeftPagePointer);
        
        public IBTreeIO<T> BTreeIO;
        public IBisection<T> BisectSearch;
        
        public IKey<T>[] FoundKeys { get; private set; }
        public IRecord<T>[] FoundRecords {
            get
            {
                if (foundRecords.Count > 0) return foundRecords.ToArray();
                foreach (var key in FoundKeys)
                {
                    foundRecords.Add(BTreeIO.GetRecord(key.Record));
                }

                return foundRecords.ToArray();
            } 
        }
        
        public bool SearchForPair(IKey<T> key, IRecord<T> record)
        {
            currentPage = BTreeIO.GetRootPage();
            foundRecords.Clear();

            while (currentPage.PageType != PageType.NULL)
            {
                BisectSearch.GetClosestIndexTo(currentPage, key.Value);
                if (key.Equals(currentPage.KeyAt(BisectSearch.LastIndex)))
                {
                    GetAllKeysWithSameValue(BisectSearch.LastIndex);
                    return true;
                }

                if (key.CompareTo(currentPage.KeyAt(BisectSearch.LastIndex)) == (int) Comparison.LESS)
                    currentPage = SmallerPageOfFoundKey;
                else
                    currentPage = BiggerPageOfFoundKey;
            }

            return false;
        }

        private void GetAllKeysWithSameValue(long index)
        {
            var listOfFoundKeys = new List<IKey<T>>();
            var currentIndex = index;
            var currentValue = currentPage.KeyAt(index).Value;
            listOfFoundKeys.Add(currentPage.KeyAt(index));
            while (currentIndex < currentPage.PageLength - 1 && currentValue.Equals(currentPage.KeyAt(currentIndex + 1).Value))
            {
                listOfFoundKeys.Add(currentPage.KeyAt(++currentIndex));
            }

            currentIndex = index;
            while (currentIndex > 0 && currentValue.Equals(currentPage.KeyAt(currentIndex - 1).Value))
            {
                listOfFoundKeys.Add(currentPage.KeyAt(--currentIndex));
            }
        }
    }
}