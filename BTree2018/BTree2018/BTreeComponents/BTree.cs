using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public class BTree<T> : IBTree<T> where T : IComparable<T>
    {
        public void Add(IRecord<T> record)
        {
            throw new NotImplementedException();
        }

        public void Remove(IRecord<T> record)
        {
            throw new NotImplementedException();
        }

        public void Remove(T key)
        {
            throw new NotImplementedException();
        }

        public bool HasKey(T key)
        {
            throw new NotImplementedException();
        }

        public void Get(IKey<T> key)
        {
            throw new NotImplementedException();
        }

        public IRecord<T> this[IKey<T> key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Reorganize()
        {
            throw new NotImplementedException();
        }
    }
}