using System;
using System.Collections.Generic;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    /// <summary> This struct should be initialized by a builder </summary>
    public struct BTreeKey<T> : IKey<T>, IComparable<BTreeKey<T>> where T : IComparable
    {
        public IRecord<T> Value { get; set; }
        public long N { get; set; }
        public IRecordPointer<T> Record { get; set; }
        public IPagePointer<T> LeftPagePointer { get; set; }
        public IPagePointer<T> RightPagePointer { get; set; }

        public int CompareTo(BTreeKey<T> other)
        {
            var valueComparison = Comparer<IRecord<T>>.Default.Compare(Value, other.Value);
            if (valueComparison != 0) return valueComparison;
            return N.CompareTo(other.N);
        }

        public int CompareTo(IKey<T> other)
        {
            var comparisonValue = other.Value.CompareTo(Value);
            if (comparisonValue == 0)
            {
                if (other.N == N) return 0;
                if (other.N < N) return -1;
                return 1;
            }

            return comparisonValue;
        }
    }
}