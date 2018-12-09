using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IPage<T> where T : IComparable<T>
    {
        long Length { get; }
        IPagePointer<T> ParentPage { get; }
        IPagePointer<T> PointerAt(long index);
        IKey<T> KeyAt(long index);
        PageType PageType { get; }//TODO: Questionable way of knowing if an IPage pointer is null or not.
    }

    public interface INullPage<T> : IPage<T> where T : IComparable<T>
    {
        
    }

    public enum PageType
    {
        NULL,
        ROOT,
        BRANCH,
        LEAF
    }
}