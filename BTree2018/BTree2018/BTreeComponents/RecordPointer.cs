using System;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public class RecordPointer<T> : IRecordPointer<T> where T : IComparable<T>
    {
        private T record;
        
        public T GetRecord()
        {
            if (record == null)
            {
                //TODO: Get record from file
            }

            return record;
        }
    }
}