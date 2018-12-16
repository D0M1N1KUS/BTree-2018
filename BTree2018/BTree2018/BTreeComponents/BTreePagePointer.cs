using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public class BTreePagePointer<T> : IPagePointer<T> where T : IComparable
    {
        public PageType PointsToPageType { get; set; }
    }
}