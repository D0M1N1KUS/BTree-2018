using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeCompensationPageModifier<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        public IBTreeAdding<T> BTreeAdding;
        public IBTreePageNeighbours<T> BTreePageNeighbours;
        
        //TODO: Something is broken while turning left :/
        public bool EvenOutKeys(ref IPage<T> parentPage, int parentKeyIndex, ref IPage<T> leftPage,
            ref IPage<T> rightPage)
        {
            checkParameters(parentPage, parentKeyIndex, leftPage, rightPage);
            if (!checkIfPagesContainEnoughValues(leftPage, rightPage)) return false;
            distributeKeysAcrossPages(ref parentPage, parentKeyIndex, ref leftPage, ref rightPage, out var parentKey);
//            updateParentPagePointersAfterCompensation(leftPage.PagePointer);
//            updateParentPagePointersAfterCompensation(rightPage.PagePointer);
            return true;
        }
        
        private void distributeKeysAcrossPages(ref IPage<T> parentPage, int parentKeyIndex, ref IPage<T> leftPage,
            ref IPage<T> rightPage, out IKey<T> parentKey)
        {
            var keysInTotal = leftPage.KeysInPage + rightPage.KeysInPage + 1;
            var sumOfKeysInNeighbouringPages = leftPage.KeysInPage + rightPage.KeysInPage;
            var keysToAddToLeftPage = sumOfKeysInNeighbouringPages / 2;
            var keysToAddFromRightPage = sumOfKeysInNeighbouringPages - keysToAddToLeftPage;
            var listOfKeys = getListOfKeys(ref leftPage, parentPage.KeyAt(parentKeyIndex), ref rightPage,
                out var listOfPointers);

            var leftPageBuilder = new BTreePageBuilder<T>((int) parentPage.PageLength)
                .CreateEmptyCloneFromPage(leftPage)
                .AddPointer(listOfPointers[0]);
            parentKey = null;
            var rightPageBuilder = new BTreePageBuilder<T>((int) parentPage.PageLength)
                .CreateEmptyCloneFromPage(rightPage)
                .AddPointer(listOfPointers[(keysInTotal + 1) / 2]);
            
            for (var i = 0; i < keysInTotal; i++)
            {
                if (i < keysToAddToLeftPage)
                {
                    leftPageBuilder.AddKey(listOfKeys[i]);
                    leftPageBuilder.AddPointer(listOfPointers[i + 1]);
                }
                else if (i >= keysInTotal - keysToAddFromRightPage)
                {
                    rightPageBuilder.AddKey(listOfKeys[i]);
                    rightPageBuilder.AddPointer(listOfPointers[i + 1]);
                }
                else
                {
                    parentKey = listOfKeys[i];
                }
            }
            
            leftPage = leftPageBuilder.Build();
            rightPage = rightPageBuilder.Build();
            parentPage = new BTreePageBuilder<T>().ClonePage(parentPage)
                .ModifyKeyAt(parentKeyIndex, parentKey)
                .Build();
        }

        private static void checkParameters(IPage<T> parentPage, int parentKeyIndex, IPage<T> page1, IPage<T> page2)
        {
            if (parentPage == null || parentPage.PageType == PageType.NULL ||
                parentKeyIndex < 0 || parentKeyIndex > parentPage.KeysInPage - 1 ||
                page1 == null || page1.PageType == PageType.NULL ||
                page2 == null || page2.PageType == PageType.NULL ||
                page1.PageLength != page2.PageLength)
            {
                var e = new Exception("BTreeCompensationPageModifier: Invalid value(s) passed to method!");
                e.Data.Add("parentPage", parentPage != null ? parentPage.ToString() : "NULL");
                e.Data.Add("parentKeyIndex", parentKeyIndex.ToString());
                e.Data.Add("page1", page1 != null ? page1.ToString() : "NULL");
                e.Data.Add("page2", page2 != null ? page2.ToString() : "NULL");
                throw e;
            }
        }

        private bool checkIfPagesContainEnoughValues(IPage<T> page1, IPage<T> page2)
        {
            return page1.KeysInPage != page2.KeysInPage && page1.KeysInPage + page2.KeysInPage <= 4 * BTreeIO.D &&
                page1.KeysInPage + page2.KeysInPage >= 2 * BTreeIO.D;
        }

        private IKey<T>[] getListOfKeys(ref IPage<T> page1, IKey<T> parentKey, ref IPage<T> page2, 
            out IPagePointer<T>[] pagePointers)
        {
            var listOfKeys = new List<IKey<T>>((int)page1.KeysInPage + (int)page2.KeysInPage + 1);
            var listOfPointers = new List<IPagePointer<T>>((int) page1.KeysInPage + (int) page2.KeysInPage + 2);
            for (var i = 0; i < page1.KeysInPage; i++)
            {
                listOfKeys.Add(page1.KeyAt(i));
                listOfPointers.Add(page1.LeftPointerAt(i));
            }
            listOfPointers.Add(page1.RightPointerAt(page1.KeysInPage - 1));
            
            listOfKeys.Add(parentKey);

            for (var i = 0; i < page2.KeysInPage; i++)
            {
                listOfKeys.Add(page2.KeyAt(i));
                listOfPointers.Add(page2.LeftPointerAt(i));
            }
            listOfPointers.Add(page2.RightPointerAt(page2.KeysInPage - 1));

            pagePointers = listOfPointers.ToArray();
            return listOfKeys.ToArray();
        }
        
        protected void updateParentPagePointersAfterCompensation(IPagePointer<T> newPagePointer)
        {
            if (newPagePointer.Equals(BTreePagePointer<T>.NullPointer)) 
                throw new NullReferenceException("BTreePageSplitter error: Tried to access page with null pointer:" +
                                                 newPagePointer);
            var newPage = BTreeIO.GetPage(newPagePointer);

            if (newPage.PageType == PageType.LEAF) return;
            for (var i = 0; i <= newPage.KeysInPage; i++)
                BTreeIO.SetPageParentPointer(newPage.PointerAt(i), newPagePointer);
        }
    }
}