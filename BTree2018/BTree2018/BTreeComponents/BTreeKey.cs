using System;
using System.Collections.Generic;
using BTree2018.BTreeIOComponents;
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

        public static IKey<T> NullKey => new BTreeKey<T>()
            {RecordPointer = RecordPointer<T>.NullPointer, Value = GenericArithmetic<T>.ConvertToGeneric(0)};

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
                ")]"
            );
        }

        public override bool Equals(object o)
        {
            var otherKey = o as IKey<T>;
            if (otherKey == null || !Value.Equals(otherKey.Value) || !RecordPointer.Equals(otherKey.RecordPointer))
                return false;
            return true;
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