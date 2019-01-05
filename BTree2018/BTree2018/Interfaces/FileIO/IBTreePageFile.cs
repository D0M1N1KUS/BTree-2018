using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.FileIO
{
    public interface IBTreePageFile<T> where T : IComparable
    {
        long TreeHeight { get; }
        IPagePointer<T> RootPage { get; set; }
        long D { get; }
        IPage<T> this[IPagePointer<T> index] { get; }
        IPagePointer<T> this[IPage<T> page] { set; }
        IPage<T> PageAt(IPagePointer<T> index);
        void SetPage(IPage<T> page);
    }
}