using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.HelperClasses.BTree
{
    public class PageTestFixture<T> : IPage<T> where T : IComparable
    {
        private BTreeKey<T>[] Keys;
        private IPagePointer<T>[] PagePointers;
        
        public void SetUpValues(params T[] values)
        {
            var listOfNewKeys = new List<BTreeKey<T>>();
            foreach (var value in values)
            {
                var key = new BTreeKey<T>() { Value = value, RecordPointer = RecordPointer<T>.NullPointer};
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

        public bool OverFlown { get; set; }
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
        public string ToString(string format = "")
        {
            throw new NotImplementedException();
        }


        public override bool Equals(object o)
        {
            var otherPage = o as IPage<T>;
            if (otherPage == null || KeysInPage != otherPage.KeysInPage || 
                PageLength != otherPage.PageLength || PageType != otherPage.PageType) 
                return false;
            for (var i = 0; i < KeysInPage; i++)
            {
                if (!KeyAt(i).Equals(otherPage.KeyAt(i))) return false;
                if (!PointerAt(i).Equals(otherPage.PointerAt(i))) return false;
            }
            if (!PointerAt(KeysInPage).Equals(otherPage.PointerAt(KeysInPage))) return false;

            return true;
        }
    }
}