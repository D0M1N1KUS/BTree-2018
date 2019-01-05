using System;
using System.Windows.Input;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IPagePointer<T> where T : IComparable
    {
        PageType PointsToPageType { get; }
        long Index { get; }
    }
}