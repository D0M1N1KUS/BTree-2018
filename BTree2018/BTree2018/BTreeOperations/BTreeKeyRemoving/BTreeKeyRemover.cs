using System;
using BTree2018.Bisection;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeKeyRemover<T> : IBTreeRemoving<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        public IBTreeSearching<T> BTreeSearching;
        public IBTreeLeafKeyRemoval<T> LeafKeyRemoval;
        public IBTreeCompensation<T> BTreeCompensation;
        
        public void RemoveKey(IKey<T> key)
        {
            if (!BTreeSearching.SearchForKey(key)) return;
            if (getKeysLeftAndRightPage(BTreeSearching.FoundKeyIndex, BTreeSearching.FoundPage, 
                out var leftPage, out var rightPage)) //Non leaves
            {
                removeKeyFromNonLeafPage(leftPage, rightPage, out var newPage, out var modifiedLeafPage);
                if (modifiedLeafPage.KeysInPage < modifiedLeafPage.PageLength / 2)
                    if (!BTreeCompensation.Compensate(modifiedLeafPage)) 
                        ;//TODO: Merging
                BTreeIO.WritePages(newPage, modifiedLeafPage);
            }
            else // Leaves
            {
                var newPage = RemoveKeyFromLeafPage(BTreeSearching.FoundKeyIndex, BTreeSearching.FoundPage);
                BTreeIO.WritePage(newPage);
            }
        }

        public static IPage<T> RemoveKeyFromLeafPage(IKey<T> key, IPage<T> page)
        {
            var index = new BisectSearch<T>().GetClosestIndexTo(page, key.Value);
            return RemoveKeyFromLeafPage(index, page);
        }

        public static IPage<T> RemoveKeyFromLeafPage(long index, IPage<T> page)
        {
            var newPageBuilder = new BTreePageBuilder<T>((int) page.PageLength)
                .CreateEmptyCloneFromPage(page);
            for (var i = 0; i < page.KeysInPage; i++)
            {
                if(i == index) continue;
                newPageBuilder.AddKey(page.KeyAt(i));
            }

            return newPageBuilder.Build();
        }

        private void removeKeyFromNonLeafPage(IPage<T> leftPage, IPage<T> rightPage, out IPage<T> newPage, 
            out IPage<T> modifiedLeafPage)
        {
            var pageWithMoreKeys = getPageWithMoreKeys(leftPage, rightPage, out var smallerPageReturned);
            var newPageBuilder = new BTreePageBuilder<T>((int) BTreeSearching.FoundPage.PageLength)
                .ClonePage(BTreeSearching.FoundPage);
            var removedKey = smallerPageReturned
                ? LeafKeyRemoval.RemoveSmallestKey(pageWithMoreKeys, out modifiedLeafPage)
                : LeafKeyRemoval.RemoveBiggestKey(pageWithMoreKeys, out modifiedLeafPage);

            newPageBuilder.ModifyKeyAt((int) BTreeSearching.FoundKeyIndex, removedKey);
            newPage = newPageBuilder.Build();
        }

        private bool getKeysLeftAndRightPage(long index, IPage<T> foundPage, out IPage<T> leftPage, out IPage<T> rightPage)
        {
            leftPage = null;
            rightPage = null;

            if (foundPage.LeftPointerAt(index).Equals(BTreePagePointer<T>.NullPointer))
                leftPage = BTreeIO.GetPage(foundPage.LeftPointerAt(index));
            if (foundPage.RightPointerAt(index).Equals(BTreePagePointer<T>.NullPointer))
                rightPage = BTreeIO.GetPage(foundPage.RightPointerAt(index));

            return leftPage != null || rightPage != null;
        }

        private IPage<T> getPageWithMoreKeys(IPage<T> leftPage, IPage<T> rightPage, out bool smallerPageReturned)
        {
            if (leftPage != null && rightPage != null)
            {
                if (leftPage.KeysInPage > rightPage.KeysInPage)
                {
                    smallerPageReturned = true;
                    return leftPage;
                }

                smallerPageReturned = false;
                return rightPage;
            }
            if (leftPage != null)
            {
                smallerPageReturned = true;
                return leftPage;
            }
            if (rightPage != null)
            {
                smallerPageReturned = false;
                return rightPage;
            }
            
            throw new Exception("BTreeKeyRemover error: left and right page of key are null!");
        }
    }
}