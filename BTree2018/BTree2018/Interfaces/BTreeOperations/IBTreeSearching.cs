using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.Interfaces.BTreeOperations
{
    public interface IBTreeSearching<T> where T : IComparable
    {
        ///<summary>Contains the found key, if search was successful</summary>
        IKey<T> FoundKey { get; }
        //<summary>Gets the found record, is search was successful.</summary>
        //IRecord<T> FoundRecord { get; }
        ///<summary>Contains the page where the searched key should be located, if it exists.</summary>
        IPage<T> FoundPage { get; }
        /// <summary>Contains the found key's index in the page, if search was successful</summary>
        long FoundKeyIndex { get; }
        bool SearchForKey(IKey<T> key);
    }
}