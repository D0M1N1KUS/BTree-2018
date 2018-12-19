using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public struct BTreePagePointer<T> : IPagePointer<T> where T : IComparable
    {
        public PageType PointsToPageType { get; set; }
        public long Index { get; set; }
        public static IPagePointer<T> NullPointer => new BTreePagePointer<T>()
        {
            PointsToPageType = PageType.NULL, 
            Index = long.MinValue
        };
    }
}