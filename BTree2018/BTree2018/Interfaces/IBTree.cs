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

        void Add(IRecord<T> record); //Adding
        void Remove(IRecord<T> record); //Removing
        void Remove(IKey<T> key);
        void Remove(T key);

        bool HasKey(T key);//Searching
        void Get(IKey<T> key); //Getting
        IRecord<T> this[IKey<T> key] { get; set; }//Getting/Altering

        void Reorganize(); //Reorganizing on demand
    }
}