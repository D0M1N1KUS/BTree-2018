using System;
using System.Collections.Generic;
using BTree2018.Enums;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    /// <summary> This struct should be initialized by a builder </summary>
    public struct BTreeKey<T> : IKey<T>, IComparable<BTreeKey<T>> where T : IComparable
    {
        public T Value { get; set; }
        public IRecordPointer<T> RecordPointer { get; set; }
        //public IPagePointer<T> LeftPagePointer { get; set; }
        //public IPagePointer<T> RightPagePointer { get; set; }

        public int CompareTo(IKey<T> other)
        {
            var comparisonValue = Value.CompareTo(other.Value);
            return comparisonValue;
        }

        public int CompareTo(BTreeKey<T> other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return string.Concat(
                "[Key(", base.ToString(),
                ") Value(", Value.ToString(),
                ") RecordPointer(", RecordPointer.ToString(), 
                //") LeftPagePointer(", LeftPagePointer.ToString(),
                //") RightPagePointer(", RightPagePointer.ToString(),
                ")]"
            );
        }

        public static bool operator <(BTreeKey<T> a, BTreeKey<T> b)
        {
            return a.CompareTo(b) == (int)Comparison.LESS;
        }

        public static bool operator >(BTreeKey<T> a, BTreeKey<T> b)
        {
            return a.CompareTo(b) == (int) Comparison.GREATER;
        }

        public static bool operator ==(BTreeKey<T> a, BTreeKey<T> b)
        {
            return a.CompareTo(b) == (int) Comparison.EQUAL;
        }

        public static bool operator !=(BTreeKey<T> a, BTreeKey<T> b)
        {
            return !(a == b);
        }

        public static bool operator <=(BTreeKey<T> a, BTreeKey<T> b)
        {
            return a < b || a == b;
        }

        public static bool operator >=(BTreeKey<T> a, BTreeKey<T> b)
        {
            return a > b || a == b;
        }
    }
}