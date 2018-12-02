using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class ValuePointerTestClass<T> : IValuePointer<T> where T : IComparable
    {
        public T GetValue()
        {
            return new TestValueClass<T>().Value;
        }
    }
    
    public class TestValueClass<T> : IRecord<T> where T : IComparable
    {
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public T Value { get; } = default(T);
        public T[] ValueComponents { get; } = null;
    }

    public class TestClass
    {
        public void method()
        {
            var valuePointer = new ValuePointerTestClass<double>();
            double value = valuePointer.GetValue();
        }
    }
}