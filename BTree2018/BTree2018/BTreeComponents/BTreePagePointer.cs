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
        
        public override bool Equals(object obj)
        {
            var pointer = obj as IPagePointer<T>;
            if (pointer == null) return false;
            return pointer.Index == Index && 
                   pointer.PointsToPageType == PointsToPageType;
        }

        public override string ToString()
        {
            return string.Concat(
                "[PagePointer(", base.ToString(),
                ") Index(", Index,
                ") PointsToPageType(", PointsToPageType.ToString("g"),
                ")]");
        }
    }
}