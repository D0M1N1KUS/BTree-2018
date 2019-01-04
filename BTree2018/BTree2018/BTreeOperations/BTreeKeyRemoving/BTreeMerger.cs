using System;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class BTreeMerger<T> : IBTreeMerging<T> where T : IComparable
    {
        //TODO: The compensation class has to chech if the number of keys in the modified pages is ok
        public void Merge(IPage<T> pageWithShortage)
        {
            throw new NotImplementedException();
        }
    }
}