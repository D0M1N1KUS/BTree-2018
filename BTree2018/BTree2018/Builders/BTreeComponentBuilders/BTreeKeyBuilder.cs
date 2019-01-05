using System;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;

namespace BTree2018.Builders
{
    public class BTreeKeyBuilder<T> where T : IComparable
    {
        private BTreeKey<T> key;

        public T Value;
        public IRecordPointer<T> RecordPointer;
        
        public IKey<T> Build()
        {
            if(!allNecessaryParametersInitialized())
                throw new Exception("BTreeKeyBuilder: Not all necessary values have been initialized! (See above logs)");
            key = new BTreeKey<T> { RecordPointer = RecordPointer, Value = Value};

            key.Value = Value;

            return key;
        }

        private bool allNecessaryParametersInitialized()
        {
            bool allInitialized = true;
            if (RecordPointer == null)
            {
                allInitialized = false;
                Logger.Log("BTreeKeyBuilder: Record is not initialized!");
            }

            if (Value == null)
            {
                allInitialized = false;
                Logger.Log("BTreeKeyBuilder: Value is not initialized!");
            }

            return allInitialized;
        }

        public BTreeKeyBuilder<T> SetVaule(T value)
        {
            Value = value;
            return this;
        }

        public BTreeKeyBuilder<T> SetRecord(IRecordPointer<T> recordPointer)
        {
            RecordPointer = recordPointer;
            return this;
        }
    }
}