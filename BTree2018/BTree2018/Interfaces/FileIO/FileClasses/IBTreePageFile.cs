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

        IPagePointer<T> AddNewPage(IPage<T> page);
        
        void RemovePage(IPage<T> page);
        void RemovePage(IPagePointer<T> pointer);
    }
}