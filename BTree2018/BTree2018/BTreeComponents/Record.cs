using System;
using System.Linq;
using System.Text;
using BTree2018.BTreeIOComponents;
using BTree2018.Enums;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;

namespace BTree2018.BTreeStructure
{
    public struct Record<T> : IRecord<T>, IComparable where T : IComparable
    {
        public IRecordPointer<T> RecordPointer { get; }
        public T Value { get; set; }
        public T[] ValueComponents { get; set; }

        public Record(T[] valueComponents, IRecordPointer<T> pointer)
        {
            ValueComponents = valueComponents;
            Value = valueComponents[0];
            foreach (var value in valueComponents)
            {
                if (value.CompareTo(Value) == (int) Comparison.GREATER)
                    Value = value;
            }

            RecordPointer = pointer;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is IRecord<T> otherRecord)) return 1;
            var compareValue = otherRecord.Value.CompareTo(Value);
            if (compareValue != 0) return compareValue;
            if (otherRecord.ValueComponents.Length == ValueComponents.Length) return 0;
            if (otherRecord.ValueComponents.Length < ValueComponents.Length) return 1;
            return -1;
        }

        public int CompareTo(Record<T> other)
        {
            var compareValue = other.Value.CompareTo(Value);
            if (compareValue != 0) return compareValue;
            if (other.ValueComponents.Length == ValueComponents.Length) return 0;
            if (other.ValueComponents.Length < ValueComponents.Length) return 1;
            return -1;
        }

        public int CompareTo(T other)
        {
            return other.CompareTo(Value);
        }

        public override bool Equals(object other)
        {
            if (!(other is IRecord<T> otherRecord)) return false;
            return CompareTo(otherRecord) == (int) Comparison.EQUAL;
        } 

        public override string ToString()
        {
            return string.Concat(
                "[Record(", base.ToString(),
                ") Value(", Value.ToString(),
                ") ValueComponents(", CollectionSerialization.Stringify(ValueComponents),
                ") Pointer(", RecordPointer.ToString(),
                ")");
        }
    }
}