using System;
using System.Windows.Input;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IPagePointer<T> where T : IComparable
    {
        PageType PointsToPageType { get; }
        //TODO: Pointers shouldn't be responsible for loading pages, records etc from disk.
    }

    public interface IPageNullPointer<T> : IPagePointer<T> where T : IComparable
    {
        new IPage<T> Get();
    }
}