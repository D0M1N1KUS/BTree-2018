using System;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Enums;
using BTree2018.Exceptions;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeOperations
{
    public class BTreeAdder<T> : IBTreeAdding<T> where T : IComparable
    {
//        private IKey<T> keyToAdd;
//        private IPage<T> currentPage;

        private IPagePointer<T> rightPointerOfAddedKey;
        
        public IBTreeIO<T> BTreeIO;
        public IBTreeSearching<T> BTreeSearching;
//        public IBTreeCompensation<T> BTreeCompensation;
//        public IBTreeSplitting<T> BTreeSplitting;
        public IBtreeReorganizing<T> Reorganizer;
        
        public IPage<T> Add(IKey<T> key)
        {
            if (BTreeSearching.SearchForKey(key))
            {
                throw new Exception("Key already exists: " + key);
            }

            rightPointerOfAddedKey = null;
            return AddToPage(key, BTreeSearching.FoundPage);
        }

        public IPage<T> AddToPage(IKey<T> key, IPage<T> page)
        {
            if (page.KeysInPage < page.PageLength) //|| page.KeysInPage == page.PageLength)//m < 2d
            {
                var newPage = addKeyToPage(page, key);
                BTreeIO.WritePage(newPage);
                return newPage;
            }
            else if (page.KeysInPage == page.PageLength)//found page is full
            {
//                if (!BTreeCompensation.Compensate(page, key))
//                    return BTreeSplitting.Split(page, key);
//                return BTreeCompensation.Page;
                return Reorganizer.Reorganize(page, key);
            }

            throw KeyAddingException("Page inconsistency detected: There are more keys in this page than allowed!",
                key, page);
        }

        private IPage<T> addKeyToPage(IPage<T> currentPage, IKey<T> keyToAdd)
        {
            var keyAdded = false;
            var pageBuilder = new BTreePageBuilder<T>((int) currentPage.PageLength)
                .CreateEmptyCloneFromPage(currentPage)
                .AddPointer(currentPage.LeftPointerAt(0));
            for (var i = 0; i < currentPage.KeysInPage; i++)
            {
                var currentKey = currentPage.KeyAt(i);
                if (keyAdded || keyToAdd.CompareTo(currentKey) == (int) Comparison.GREATER) //currentKey < keyToAdd
                {
                    pageBuilder.AddKey(currentKey); 
                    pageBuilder.AddPointer(currentPage.RightPointerAt(i));
                }
                else if (!keyAdded && keyToAdd.CompareTo(currentKey) == (int) Comparison.LESS)
                {
                    pageBuilder.AddKey(keyToAdd)
                        .AddPointer(rightPointerOfAddedKey ?? BTreePagePointer<T>.NullPointer);
                    pageBuilder.AddKey(currentKey);
                    pageBuilder.AddPointer(currentPage.RightPointerAt(i));
                    keyAdded = true;
                }
                else
                    throw KeyAddingException("Duplicate key detected while inserting.", keyToAdd, currentPage);
            }

            if (!keyAdded)
                pageBuilder.AddKey(keyToAdd)
                    .AddPointer(rightPointerOfAddedKey ?? BTreePagePointer<T>.NullPointer);
            
            return pageBuilder.Build();
        }

        private Exception KeyAddingException(string message, IKey<T> keyToAdd, IPage<T> currentPage)
        {
            var e = new Exception(message);
            e.Data.Add("Key to add", keyToAdd.ToString());
            e.Data.Add("Destination page", currentPage.ToString());
            return e;
        }

        /// <summary>
        /// Inserts key into page without checking, if this page is it's optimal destination.
        /// Use this operation only during splitting or to insert a key into a page located in memory! 
        /// </summary>
        /// <param name="page">Page to insert the key into</param>
        /// <param name="key">Key that needs to be inserted into the page</param>
        /// <param name="rightPointerOfKey"></param>
        /// <returns></returns>
        public IPage<T> InsertKeyIntoPage(IPage<T> page, IKey<T> key, IPagePointer<T> rightPointerOfKey = null)
        {
            rightPointerOfAddedKey = rightPointerOfKey;
            return addKeyToPage(page, key);
        }
    }
}