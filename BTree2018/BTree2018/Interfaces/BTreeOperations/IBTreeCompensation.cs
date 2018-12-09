using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeCompensation<T> where T : IComparable<T>
    {
        void Compensate(IPage<T> parent, IPage<T> leftChild, IPage<T> rightChild);
    }
}