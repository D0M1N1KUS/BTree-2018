using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public struct BTreePage<T> : IPage<T> where T : IComparable<T>
    {
        public IPagePointer<T>[] Pointers;
        public IKey<T>[] Keys;
        
        public long Length { get; set; }
        public IPagePointer<T> ParentPage { get; set; }
        public IPagePointer<T> PointerAt(long index)
        {
            if(PageType == PageType.NULL)
                throw new NullReferenceException("Can't access pointers of Null-type page!");
            return Pointers[index];
        }

        public IKey<T> KeyAt(long index)
        {
            if(PageType == PageType.NULL)
                throw new NullReferenceException("Can't access keys of Null-type page!");
            return Keys[index];
        }

        public PageType PageType { get; set; }
    }
}