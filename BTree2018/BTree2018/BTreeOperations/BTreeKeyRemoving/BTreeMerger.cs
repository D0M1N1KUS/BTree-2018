using System;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeMerger<T> : BTreePageMergerBase<T>, IBTreeMerging<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        public IBTreePageNeighbours<T> BTreePageNeighbours;
        
        public IPage<T> ParentPage { get; private set; }
        
        public void Merge(IPage<T> pageWithShortage)
        {
            BTreePageNeighbours.GetNeighbours(pageWithShortage, out var leftNeighbourPtr, out var rightNeighbourPtr,
                out var parentKey, out var parentKeyIndex);
            var parentPage = BTreePageNeighbours.ParentPage;

            if (checkForSpecialRootMergingSituation(leftNeighbourPtr, rightNeighbourPtr, pageWithShortage,
                out var rootPage, out var leftLeafPage, out var rightLeafPage))
            {
                mergeSpecificRootConfiguration(rootPage, leftLeafPage, rightLeafPage);
                BTreeIO.DecreaseTreeHeight();
                return;
            }

            IPage<T> mergedPage, newParentPage;
            
            if (checkIfPagesCanBeMerged(leftNeighbourPtr, pageWithShortage, out var leftPage))
            {
                mergedPage = MergePagesAndKey(leftPage, parentKey, pageWithShortage);
                newParentPage =
                    RemoveParentPageKeyAndInsetNewPointer(parentPage, parentKeyIndex, mergedPage.PagePointer);
                BTreeIO.FreePage(pageWithShortage);
            }
            else if (checkIfPagesCanBeMerged(rightNeighbourPtr, pageWithShortage, out var rightPage))
            {
                mergedPage = MergePagesAndKey(pageWithShortage, parentKey, rightPage);
                newParentPage =
                    RemoveParentPageKeyAndInsetNewPointer(parentPage, parentKeyIndex, mergedPage.PagePointer);
                BTreeIO.FreePage(rightPage);
            }
            else
                throw new Exception("BTreeMerger error: Neighbouring pages of" + pageWithShortage +
                                    "aren't suitable for merging!");

            ParentPage = newParentPage;
            BTreeIO.WritePages(newParentPage, mergedPage);
        }


        private bool checkIfPagesCanBeMerged(IPagePointer<T> pointerToOtherPage, IPage<T> pageWithShortage, 
            out IPage<T> otherPage)
        {
            if (pointerToOtherPage == null || pointerToOtherPage.PointsToPageType == PageType.NULL)
            {
                otherPage = null;
                return false;
            }

            otherPage = BTreeIO.GetPage(pointerToOtherPage);
            return otherPage.PageType != PageType.NULL &&
                    otherPage.KeysInPage + pageWithShortage.KeysInPage + 1 <= pageWithShortage.PageLength;
        }

        private bool checkForSpecialRootMergingSituation(IPagePointer<T> leftPointer, IPagePointer<T> rightPointer, 
            IPage<T> pageWithShortage, out IPage<T> rootPage, out IPage<T> leftPage, out IPage<T> rightPage)
        {
            if (pageWithShortage.ParentPage.PointsToPageType == PageType.ROOT)
            {
                rootPage = BTreeIO.GetPage(pageWithShortage.ParentPage);
                if (rootPage.KeysInPage == 1)
                {
                    leftPage = !leftPointer.Equals(BTreePagePointer<T>.NullPointer)
                        ? BTreeIO.GetPage(leftPointer)
                        : pageWithShortage;
                    rightPage = !rightPointer.Equals(BTreePagePointer<T>.NullPointer)
                        ? BTreeIO.GetPage(rightPointer)
                        : pageWithShortage;
                    return true;
                }
            }

            rootPage = null;
            leftPage = null;
            rightPage = null;
            return false;
        }

        private void mergeSpecificRootConfiguration(IPage<T> rootPage, IPage<T> leftPage, IPage<T> rightPage)
        {
            var newRootPage = MergePagesAndKey(leftPage, rootPage.KeyAt(0), rightPage, PageType.ROOT);
            BTreeIO.FreePage(rightPage);
            BTreeIO.FreePage(rootPage);
            BTreeIO.WriteNewRootPage(newRootPage);
            ParentPage = newRootPage;
        }
        
    }
}