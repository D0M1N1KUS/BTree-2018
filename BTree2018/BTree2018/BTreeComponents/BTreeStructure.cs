using System;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeStructure
{
    public struct BTreeStructure<T> : IBTreeStructure<T> where T : IComparable<T>
    {
        public long H { get; set; }
        public long D { get; set; }
        public IPagePointer<T> Root { get; set; }
    }
}