using System;
using BTree2018.Interfaces.CustomCollection;
using NUnit.Framework;

namespace UnitTests.HelperClasses
{
    public class CustomCollection<T> : ICustomCollection<T> where T : IComparable
    {
        private T[] values;
        
        public CustomCollection(params T[] values)
        {
            Length = values.Length;
            this.values = values;
        }

        public long Length { get; }

        public T this[long index] => values[index];
    }
}