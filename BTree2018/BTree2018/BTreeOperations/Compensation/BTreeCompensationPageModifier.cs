using System;
using System.Collections.Generic;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class BTreeCompensationPageModifier<T> where T : IComparable
    {
        public void EvenOutKeys(ref IPage<T> parentPage, int parentKeyIndex, ref IPage<T> leftPage,
            ref IPage<T> rightPage)
        {
            checkValues(parentPage, parentKeyIndex, leftPage, rightPage);
            distributeKeysAcrossPages(parentPage, parentKeyIndex, ref leftPage, ref rightPage, out var parentKey);

            
            var parentPageBuilder = new BTreePageBuilder<T>();
            for (var i = 0; i < parentPage.KeysInPage; i++)
            {
                parentPageBuilder.AddKey(i != parentKeyIndex ? parentPage.KeyAt(i) : parentKey);
                parentPageBuilder.AddPointer(parentPage.PointerAt(i));
            }
            parentPageBuilder.AddPointer(parentPage.PointerAt(parentPage.KeysInPage));
            
        }

        private void distributeKeysAcrossPages(IPage<T> parentPage, int parentKeyIndex, ref IPage<T> leftPage,
            ref IPage<T> rightPage, out IKey<T> parentKey)
        {
            var keysInTotal = leftPage.KeysInPage + rightPage.KeysInPage + 1;
            var keysPerPage = (leftPage.KeysInPage + rightPage.KeysInPage) / 2;
            var listOfKeys = getListOfKeys(ref leftPage, parentPage.KeyAt(parentKeyIndex), ref rightPage);

            var leftPageBuilder = new BTreePageBuilder<T>((int) keysPerPage);
            parentKey = null;
            var rightPageBuilder = new BTreePageBuilder<T>((int) keysPerPage);
            for (var i = 0; i < keysInTotal; i++)
            {
                if (i < keysPerPage)
                {
                    leftPageBuilder.AddKey(listOfKeys[i]);
                }
                else if (i == keysInTotal / 2)
                {
                    parentKey = listOfKeys[i];
                }
                else
                {
                    rightPageBuilder.AddKey(listOfKeys[i]);
                }
            }

            leftPage = leftPageBuilder.Build();
            rightPage = rightPageBuilder.Build();
        }

        private static void checkValues(IPage<T> parentPage, int parentKeyIndex, IPage<T> page1, IPage<T> page2)
        {
            if (parentPage == null || parentPage.PageType == PageType.NULL ||
                parentKeyIndex < 0 || parentKeyIndex > parentPage.KeysInPage - 1 ||
                page1 == null || page1.PageType == PageType.NULL ||
                page2 == null || page2.PageType == PageType.NULL)
            {
                var e = new Exception("BTreeCompensationPageModifier: Invalid value(s) passed to method!");
                e.Data.Add("parentPage", parentPage != null ? parentPage.ToString() : "NULL");
                e.Data.Add("parentKeyIndex", parentKeyIndex.ToString());
                e.Data.Add("page1", page1 != null ? page1.ToString() : "NULL");
                e.Data.Add("page2", page2 != null ? page2.ToString() : "NULL");
                throw e;
            }
        }

        private IKey<T>[] getListOfKeys(ref IPage<T> page1, IKey<T> parentKey, ref IPage<T> page2)
        {
            var listOfKeys = new List<IKey<T>>((int)page1.KeysInPage + (int)page2.KeysInPage + 1);
            for (var i = 0; i < page1.KeysInPage; i++)
            {
                listOfKeys.Add(page1.KeyAt(i));
            }
            
            listOfKeys.Add(parentKey);

            for (var i = 0; i < page2.KeysInPage; i++)
            {
                listOfKeys.Add(page2.KeyAt(i));
            }

            return listOfKeys.ToArray();
        }
    }
}