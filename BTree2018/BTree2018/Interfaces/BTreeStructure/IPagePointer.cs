using System;
using System.Windows.Input;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IPagePointer<T> where T : IComparable
    {
         IPage<T> Get();
    }

    public interface IPageNullPointer<T> : IPagePointer<T> where T : IComparable
    {
        new IPage<T> Get();
    }
}