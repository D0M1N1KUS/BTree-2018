using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.HelperClasses.BTree
{
    [TestFixture]
    public class PageFixture<T> : IPage<T> where T : IComparable
    {
        public IKey<T>[] Keys;
        
        public void SetUpValues(params T[] values)
        {
            var listOfNewKeys = new List<IKey<T>>();
            foreach (var value in values)
            {
                var key = new BTreeKey<T>();

            }
        }

        public void SetUpPointers(params IPagePointer<T>[] pagePointers)
        {
            
        }


        public long Length { get; }

        public T this[long index] => Keys[index].Value;

        public long PageLength { get; set; }
        public long KeysInPage { get; set; }
        public IPagePointer<T> ParentPage { get; set; }
        public IPagePointer<T> PagePointer { get; set; }
        public IPagePointer<T> PointerAt(long index)
        {
            throw new NotImplementedException();
        }

        public IKey<T> KeyAt(long index)
        {
            throw new NotImplementedException();
        }

        public PageType PageType { get; }
    }
}