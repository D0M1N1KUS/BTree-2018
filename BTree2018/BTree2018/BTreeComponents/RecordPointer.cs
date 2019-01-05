using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public struct RecordPointer<T> : IRecordPointer<T> where T : IComparable
    {
        public RecordPointerType PointerType { get; set; }
        public long Index { get; set; }

        public static IRecordPointer<T> NullPointer => new RecordPointer<T>()
            {Index = long.MinValue, PointerType = RecordPointerType.NULL};
        
        public override string ToString()
        {
            return string.Concat(
                "[RecordPointer(", base.ToString(),
                ") Index(", Index,
                ") Type(", PointerType.ToString("g"),
                ")]");
        }
    }
}