using System;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class BTreeReorganizer<T> : IBtreeReorganizing<T> where T : IComparable
    {
        public IBTreeCompensation<T> BTreeCompensation;
        public IBTreeMerging<T> BTreeMerger;
        public IBTreeAdding<T> BTreeAdder;
        public IBTreeSplitting<T> BTreeSplitter;
        
        public IPage<T> Reorganize(IPage<T> modifiedLeafPage)
        {
            var currentPage = modifiedLeafPage;
            while (true)
            {
                if (!BTreeCompensation.Compensate(currentPage))
                {
                    BTreeMerger.Merge(currentPage);
                    if (BTreeMerger.ParentPage != null &&
                        BTreeMerger.ParentPage.KeysInPage < BTreeMerger.ParentPage.PageLength / 2 &&
                        BTreeMerger.ParentPage.PageType != PageType.ROOT)
                    {
                        currentPage = BTreeMerger.ParentPage;
                        continue;
                    }
                }
                break;
            }

            return currentPage;
        }

        public IPage<T> Reorganize(IPage<T> fullPage, IKey<T> keyToInsert)
        {
            var currentPage = BTreeAdder.InsertKeyIntoPage(fullPage, keyToInsert);
            while (true)
            {
                if (!BTreeCompensation.Compensate(currentPage))
                {
                    var parentPage = BTreeSplitter.Split(currentPage);
                    if (parentPage != null && parentPage.OverFlown)
                    {
                        currentPage = parentPage;
                        continue;
                    }
                }
                break;
            }

            return currentPage;
        }
    }
}