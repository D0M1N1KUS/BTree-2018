using System;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeStructure
{
    public class RecordPointer<T> : IRecordPointer<T> where T : IComparable
    {
        private IRecord<T> record;
        
        public IRecord<T> GetRecord()
        {
            if (record == null)
            {
                //TODO: Get record from file
            }

            return record;
        }
    }
}