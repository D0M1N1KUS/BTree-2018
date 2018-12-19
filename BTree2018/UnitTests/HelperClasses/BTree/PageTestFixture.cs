using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.HelperClasses.BTree
{
    [TestFixture]
    public class PageTestFixture<T> : IPage<T> where T : IComparable
    {
        private BTreeKey<T>[] Keys;
        private IPagePointer<T>[] PagePointers;
        
        public void SetUpValues(params T[] values)
        {
            var listOfNewKeys = new List<BTreeKey<T>>();
            foreach (var value in values)
            {
                var key = new BTreeKey<T>() { Value = value };
                if (listOfNewKeys.Contains(key))
                    throw new Exception("Duplicate values are not allowed!");
                listOfNewKeys.Add(key);
            }
            
            Keys = listOfNewKeys.ToArray();
            KeysInPage = listOfNewKeys.Count;
        }

        public void SetUpPointers(params IPagePointer<T>[] pagePointers)
        {
            PagePointers = pagePointers;
        }

        public long Length => KeysInPage;

        public T this[long index] => Keys[index].Value;

        public long PageLength { get; set; }
        public long KeysInPage { get; set; }
        public IPagePointer<T> ParentPage { get; set; }
        public IPagePointer<T> PagePointer { get; set; }

        public IPagePointer<T> PointerAt(long index)
        {
            return PagePointers[index];
        }

        public IPagePointer<T> LeftPointerAt(long keyIndex)
        {
            return PagePointers[keyIndex];
        }
        
        public IPagePointer<T> RightPointerAt(long keyIndex)
        {
            return PagePointers[keyIndex + 1];
        }
        
        public IKey<T> KeyAt(long index)
        {
            return Keys[index];
        }

        public PageType PageType { get; set; }
    }
}