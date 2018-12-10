using System;
using System.Collections.Generic;
using BTree2018.Interfaces.CustomCollection;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IPage<T> : ICustomCollection<T> where T : IComparable
    {
        long PageLength { get; }
        long KeysInPage { get; }
        IPagePointer<T> ParentPage { get; }
        IPagePointer<T> PagePointer { get; }
        IPagePointer<T> PointerAt(long index);
        IKey<T> KeyAt(long index);
        PageType PageType { get; }//TODO: Questionable way of knowing if an IPage pointer is null or not.
    }

//    public interface INullPage<T> : IPage<T> where T : IComparable<T>
//    {
//        
//    }//TODO: If this interface proofs to be unnecessary delete it!

    public enum PageType
    {
        NULL,
        ROOT,
        BRANCH,
        LEAF
    }
}