using System;
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
        
        public IBTreeIO<T> BTreeIO;
        public IBTreeSearching<T> BTreeSearching;
        public IBTreeCompensation<T> BTreeCompensation;
        public IBTreeSplitting<T> BTreeSplitting;
        
        public void Add(IKey<T> key)
        {
            if (BTreeSearching.SearchForPair(key))
            {
                throw new Exception("Key already exists: " + key);
            }
            
            AddToPage(key, BTreeSearching.FoundPage);
        }

        private void AddToPage(IKey<T> key, IPage<T> page)
        {
            currentPage = page;
            keyToAdd = key;
            
            if (currentPage.KeysInPage < currentPage.PageLength)//m < 2d
            {
                var keyAdded = false;
                var pageBuilder = new BTreePageBuilder<T>((int) currentPage.PageLength)
                    .SetPageType(currentPage.PageType)
                    .SetParentPagePointer(currentPage.ParentPage);
                for (var i = 0; i < currentPage.KeysInPage; i++)
                {
                    var currentKey = currentPage.KeyAt(i);
                    if (keyAdded || keyToAdd.CompareTo(currentKey) == (int) Comparison.LESS)
                    {
                        pageBuilder.AddKey(currentKey)
                            .AddPointer(currentPage.PointerAt(i));
                    } 
                    else if (!keyAdded && keyToAdd.CompareTo(currentKey) == (int) Comparison.GREATER)
                    {
                        pageBuilder.AddKey(keyToAdd)
                            .AddPointer(keyToAdd.LeftPagePointer);
                        pageBuilder.AddKey(currentKey)
                            .AddPointer(currentKey.RightPagePointer);
                        keyAdded = true;
                    }
                    else
                        throw KeyAddingException("Duplicate key detected while inserting.");
                }

                if (!keyAdded)
                    pageBuilder.AddKey(keyToAdd)
                        .AddPointer(keyToAdd.RightPagePointer);
                
                BTreeIO.WritePage(pageBuilder.Build());
            }
            else if (currentPage.KeysInPage == currentPage.PageLength)
            {
                if (!BTreeCompensation.Compensate(currentPage, key))
                    BTreeSplitting.Split(page, key);
            }
            else
                throw KeyAddingException("Page inconsistency detected: There are more keys in this page than allowed!");
        }

        private Exception KeyAddingException(string message)
        {
            var e = new Exception(message);
            e.Data.Add("Key to add", keyToAdd.ToString());
            e.Data.Add("Destination page", currentPage.ToString());
            return e;
        }
    }
}