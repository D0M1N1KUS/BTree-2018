using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public struct BTreeKey<T> : IKey<T> where T : IComparable<T>
    {
        public T Value { get; set; }
        public long N { get; set; }
        public IRecord<T> Record { get; set; }
        public IPagePointer<T> LeftPagePointer { get; set; }
        public IPagePointer<T> RightPagePointer { get; set; }
    }
}