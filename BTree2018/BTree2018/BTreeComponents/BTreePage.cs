using System;
using System.Linq;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.CustomCollection;
using BTree2018.Logging;

namespace BTree2018.BTreeStructure
{
    public struct BTreePage<T> : IPage<T> where T : IComparable
    {
        public IPagePointer<T>[] Pointers;
        public IKey<T>[] Keys;

        public bool OverFlown { get; set; }
        public long PageLength { get; set; }
        public long KeysInPage { get; set; }

        public long Length => KeysInPage;
        public T this[long index] => Keys[index].Value;

        public IPagePointer<T> ParentPage { get; set; }
        public IPagePointer<T> PagePointer { get; }

        public PageType PageType { get; set; }
        
        public IPagePointer<T> PointerAt(long index)
        {
            if(PageType == PageType.NULL)
                throw new NullReferenceException("Can't access pointers of Null-type page!");
            return Pointers[index];
        }
        
        public IPagePointer<T> LeftPointerAt(long keyIndex)
        {
            return PointerAt(keyIndex);
        }
        
        public IPagePointer<T> RightPointerAt(long keyIndex)
        {
            return PointerAt(keyIndex + 1);
        }

        public IKey<T> KeyAt(long index)
        {
            if(PageType == PageType.NULL)
                throw new NullReferenceException("Can't access keys of Null-type page!");
            return Keys[index];
        }
        
        public override string ToString()
        {
            return string.Concat(
                "[Page(", base.ToString(),
                ") KeysInPage(", KeysInPage,
                ") PointersInPage(", Pointers?.Length ?? -1,
                ") KeyValues(", CollectionSerialization.Stringify(this),
                ") PageType(", PageType.ToString("g"),
                ") ParentPage(", ParentPage?.ToString() ?? "NULL",
                ") PagePointer(", PagePointer?.ToString() ?? "NULL",
                ")]");
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