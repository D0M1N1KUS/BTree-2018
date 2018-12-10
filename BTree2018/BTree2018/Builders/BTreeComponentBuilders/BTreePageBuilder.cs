using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Builders
{
    public class BTreePageBuilder<T> where T : IComparable
    {
        private IPage<T> page;
        
        public IPagePointer<T>[] Pointers;
        public IKey<T>[] Keys;
        private long Length;
        public IPagePointer<T> ParentPage;
        public PageType PageType;

        public IPage<T> Build()
        {
            throw new NotImplementedException();//TODO
        }
    }
}