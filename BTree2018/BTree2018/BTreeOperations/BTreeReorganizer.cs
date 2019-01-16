using System;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class BTreeReorganizer<T> : IBtreeReorganizing<T> where T : IComparable
    {
        public IBTreeCompensation<T> BTreeCompensation;
        public IBTreeMerging<T> BTreeMerger;
        
        public void Reorganize(IPage<T> modifiedLeafPage)
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
        }
    }
}