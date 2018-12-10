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
        public long? N = null;
        public IRecordPointer<T> RecordPointer;
        public IPagePointer<T> LeftPage;
        public IPagePointer<T> RightPage;
        
        public IKey<T> Build()
        {
            if(!allNecessaryParametersInitialized())
                throw new Exception("BTreeKeyBuilder: Not all necessary values have been initialized! (See above logs)");
            key = new BTreeKey<T> {N = N ?? -1, Record = RecordPointer, Value = Value};

            key.Value = Value;
            key.LeftPagePointer = LeftPage;
            key.RightPagePointer = RightPage;

            return key;
        }

        private bool allNecessaryParametersInitialized()
        {
            bool allInitialized = true;
            if (N == null)
            {
                allInitialized = false;
                Logger.Log("BTreeKeyBuilder: N is not initialized!");
            }

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
            
            if (LeftPage == null)
            {
                allInitialized = false;
                Logger.Log("BTreeKeyBuilder: LeftPage is not initialized!");
            }

            if (RightPage == null)
            {
                allInitialized = false;
                Logger.Log("BTreeKeyBuilder: RightPage is not initialized!");
            }

            return allInitialized;
        }

        public BTreeKeyBuilder<T> SetVaule(T value)
        {
            Value = value;
            return this;
        }

        public BTreeKeyBuilder<T> SetKeyN(long n)
        {
            N = n;
            return this;
        }

        public BTreeKeyBuilder<T> SetRecord(IRecordPointer<T> recordPointer)
        {
            RecordPointer = recordPointer;
            return this;
        }

        public BTreeKeyBuilder<T> SetPointerToLeftPage(IPagePointer<T> lp)
        {
            LeftPage = lp;
            return this;
        }

        public BTreeKeyBuilder<T> SetPointerToRightPage(IPagePointer<T> rp)
        {
            RightPage = rp;
            return this;
        }
    }
}