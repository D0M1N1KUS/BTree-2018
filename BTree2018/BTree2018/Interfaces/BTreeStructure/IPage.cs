using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    public interface IPage<T> where T : IComparable
    {
        long Length { get; }
        IPagePointer<T> ParentPage { get; }
        IPagePointer<T> PointerAt(long index);
        IKey<T> KeyAt(long index);
    }
}