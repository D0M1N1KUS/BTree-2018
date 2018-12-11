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
                var key = new BTreeKey<T>() { Value = value, N = 0 };
                while (listOfNewKeys.Contains(key))
                    key.N++;
                listOfNewKeys.Add(key);
            }
            
            Keys = listOfNewKeys.ToArray();
            KeysInPage = listOfNewKeys.Count;
        }

        public void SetUpPointers(params IPagePointer<T>[] pagePointers)
        {
            if (Keys == null)
                throw new Exception("No key values initialized! Values need to be set up before pointers!");
            var pointerList = new List<IPagePointer<T>>();
            for (var i = 0; i < Keys.Length; i++)
            {
                pointerList.Add(pagePointers[i]);
                Keys[i].LeftPagePointer = pagePointers[i];
                Keys[i].RightPagePointer = pagePointers[i + 1];
            }
            pointerList.Add(pagePointers[Keys.Length]);
            pagePointers = pointerList.ToArray();
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

        public IKey<T> KeyAt(long index)
        {
            return Keys[index];
        }

        public PageType PageType { get; set; }
    }
}