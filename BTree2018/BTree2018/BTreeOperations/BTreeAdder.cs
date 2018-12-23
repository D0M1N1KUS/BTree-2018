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
        private IKey<T> keyToAdd;
        private IPage<T> currentPage;

        private IPagePointer<T> rightPointerOfAddedKey;
        
        public IBTreeIO<T> BTreeIO;
        public IBTreeSearching<T> BTreeSearching;
        public IBTreeCompensation<T> BTreeCompensation;
        public IBTreeSplitting<T> BTreeSplitting;
        
        public IPage<T> Add(IKey<T> key)
        {
            if (BTreeSearching.SearchForPair(key))
            {
                throw new Exception("Key already exists: " + key);
            }

            rightPointerOfAddedKey = null;
            return AddToPage(key, BTreeSearching.FoundPage);
        }

        private IPage<T> AddToPage(IKey<T> key, IPage<T> page, bool addWithOverflow = false)
        {
            currentPage = page;
            keyToAdd = key;
            
            if (currentPage.KeysInPage < currentPage.PageLength || 
                currentPage.KeysInPage == currentPage.PageLength && addWithOverflow)//m < 2d
            {
                //BTreeIO.WritePage(addKeyToPage());
                return addKeyToPage();
            }
            else if (currentPage.KeysInPage == currentPage.PageLength)//found page is full
            {
                if (!BTreeCompensation.Compensate(currentPage, key))
                    BTreeSplitting.Split(page, key);
                return null;//TODO: return page;
            }

            throw KeyAddingException("Page inconsistency detected: There are more keys in this page than allowed!");
        }

        private IPage<T> addKeyToPage()
        {
            var keyAdded = false;
            var pageBuilder = new BTreePageBuilder<T>((int) currentPage.PageLength)
                .SetPageType(currentPage.PageType)
                .SetParentPagePointer(currentPage.ParentPage)
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
                    throw KeyAddingException("Duplicate key detected while inserting.");
            }

            if (!keyAdded)
                pageBuilder.AddKey(keyToAdd)
                    .AddPointer(rightPointerOfAddedKey ?? BTreePagePointer<T>.NullPointer);
            
            return pageBuilder.Build();
        }

        private Exception KeyAddingException(string message)
        {
            var e = new Exception(message);
            e.Data.Add("Key to add", keyToAdd.ToString());
            e.Data.Add("Destination page", currentPage.ToString());
            return e;
        }

        public IPage<T> InsertKeyIntoPage(IPage<T> page, IKey<T> key, IPagePointer<T> rightPointerOfKey = null)
        {
            rightPointerOfAddedKey = rightPointerOfKey;
            return AddToPage(key, page, addWithOverflow: true);
        }
    }
}