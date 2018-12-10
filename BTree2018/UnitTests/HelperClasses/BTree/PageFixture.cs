using System;
using BTree2018.Interfaces.BTreeStructure;
using NUnit.Framework;

namespace UnitTests.HelperClasses.BTree
{
    [TestFixture]
    public class PageFixture<T> : IPage<T> where T : IComparable
    {
        public IKey<T>[] Keys;
        
        public void SetUpValues(params T[] Values)
        {
                
        }

        public void SetUpPointers(params IPagePointer<T>[] pagePointers)
        {
            
        }


        public long Length { get; }

        public T this[long index] => Keys[index].Value.Value;

        public long PageLength { get; }
        public long KeysInPage { get; }
        public IPagePointer<T> ParentPage { get; }
        public IPagePointer<T> PagePointer { get; }
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