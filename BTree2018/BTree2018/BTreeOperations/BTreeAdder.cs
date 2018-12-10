using System;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class BTreeAdder<T> : IBTreeAdding<T> where T : IComparable
    {
        public IPage<T> RootPage;
        
        
        public void Add(IKey<T> key)
        {
            throw new NotImplementedException();
        }
    }
}