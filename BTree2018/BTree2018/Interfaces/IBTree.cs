using System;

namespace BTree2018.Interfaces.BTreeStructure
{
    /*
     * Btree Interface which provides methods required by the project description
     */
    public interface IBTree<T> where T : IComparable
    {
        //Compensation class is required
        //Splitting class is required
        //Merging required

        long D { get; }
        long H { get; }

        void Add(IRecord<T> record); //Adding
        void Remove(IRecord<T> record); //Removing
        void Remove(IKey<T> key);
        void Remove(T key);
        void Replace(T currentKey, IRecord<T> newRecord);
        void Replace(IKey<T> currentKey, IRecord<T> newRecord);
        void Replace(IRecord<T> currentRecord, IRecord<T> newRecord);

        bool HasKey(T key);//Searching
        IRecord<T> Get(IKey<T> key); //Getting
        IPage<T> GetPage(IPagePointer<T> pointer);
        IPage<T> GetRootPage();
        void Set(IKey<T> key, IRecord<T> record);
        IRecord<T> this[IKey<T> key] { get; set; }//Getting/Altering
    }
}