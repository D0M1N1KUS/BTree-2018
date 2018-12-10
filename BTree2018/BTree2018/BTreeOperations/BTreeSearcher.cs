using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.CustomCollection;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeSearcher<T> : IBTreeSearching<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        public IBisection<T> BisectSearch;
        
        public IKey<T> FoundKey { get; private set; }
        public IRecord<T> FoundRecord { get; private set; }
        
        public bool SearchForPair(IKey<T> key, IRecord<T> record)
        {
            var currentPage = BTreeIO.GetRootPage();

            while (currentPage.PageType != PageType.NULL)
            {
                BisectSearch.GetClosestIndexTo(currentPage, key.Value.Value);
                if()
            }

            return false;
        }
    }
}