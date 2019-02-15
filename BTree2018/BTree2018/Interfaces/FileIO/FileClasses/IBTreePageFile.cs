using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreePageFile<T> where T : IComparable
    {
        long TreeHeight { get; }
        IPagePointer<T> RootPage { get; }
        long D { get; }
        
        IPage<T> PageAt(IPagePointer<T> pointer);
        void SetPage(IPage<T> page);
        void SetPageParent(IPagePointer<T> targetPage, IPagePointer<T> parentPagePointer);
        IPagePointer<T> AddNewRootPage(IPage<T> newRootPage);
        void SetTreeHeight(long newTreeHeight);

        IPagePointer<T> AddNewPage(IPage<T> page);
        
        void RemovePage(IPage<T> page);
        void RemovePage(IPagePointer<T> pointer);
    }
}