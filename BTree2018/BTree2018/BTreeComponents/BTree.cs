using System;
using System.Collections.Generic;
using System.Windows.Data;
using BTree2018.BTreeIOComponents;
using BTree2018.Builders;
using BTree2018.Exceptions;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeStructure
{
    public class BTree<T> : IBTree<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        
        public IBTreeAdding<T> Adder;
        public IBTreeRemoving<T> Remover;
        public IBTreeSearching<T> Searcher;
        
        private IKey<T> lastSearchedKey = null;

        public long D => BTreeIO.D;
        public long H => BTreeIO.H;

        public void Add(IRecord<T> record)
        {
            var newKey = new BTreeKey<T>(){RecordPointer = RecordPointer<T>.NullPointer, Value = record.Value};
            if(Searcher.SearchForKey(newKey))
                throw new DuplicateKeyException("A record with the value " + record.Value + 
                                                " already exists. Record: " + record);
            newKey.RecordPointer = BTreeIO.WriteRecord(record);
            Adder.AddToPage(newKey, Searcher.FoundPage);
        }

        public void Remove(IRecord<T> record)
        {
            var keyToRemove = new BTreeKey<T>(){RecordPointer = RecordPointer<T>.NullPointer, Value = record.Value};
            keyToRemove.RecordPointer = Remover.RemoveKey(keyToRemove);
            BTreeIO.FreeRecord(record);
        }

        public void Remove(IKey<T> key)
        {
            var recordPointer = Remover.RemoveKey(key);
            BTreeIO.FreeRecord(recordPointer);
        }

        public void Remove(T key)
        {
            var keyToRemove = new BTreeKey<T>(){Value = key, RecordPointer = RecordPointer<T>.NullPointer};
            Remove(keyToRemove);
        }

        public void Replace(T currentKey, IRecord<T> newRecord)
        {
            var newKey = new BTreeKey<T>() {Value = newRecord.Value, RecordPointer = RecordPointer<T>.NullPointer};
            if (newKey.Value.Equals(currentKey))
            {
                if (Searcher.SearchForKey(newKey))
                {
                    BTreeIO.WriteRecord(new Record<T>(newRecord.ValueComponents, Searcher.FoundKey.RecordPointer));
                    return;
                }
            }
            else
            {
                if (HasKey(newKey.Value))
                {
                    Remove(currentKey);
                    Add(newRecord);
                    return;
                }
                    
            }
            Logger.Log("BTree Replace warning: The key " + currentKey +
                       " does not exist. New record will be added instead.");
            Add(newRecord);
        }

        public void Replace(IKey<T> currentKey, IRecord<T> newRecord)
        {
            Replace(currentKey.Value, newRecord);
        }

        public void Replace(IRecord<T> currentRecord, IRecord<T> newRecord)
        {
            Replace(currentRecord.Value, newRecord);
        }

        public bool HasKey(T key)
        {
            var keyToSearchFor = new BTreeKey<T>(){Value = key, RecordPointer = RecordPointer<T>.NullPointer};
            if (!Searcher.SearchForKey(keyToSearchFor)) return false;
            lastSearchedKey = Searcher.FoundKey;
            return true;
        }

        public IRecord<T> Get(IKey<T> key)
        {
            if (lastSearchedKey == null || !key.Equals(lastSearchedKey))
                if(!HasKey(key.Value)) throw new KeyNotFoundException("The key " + key + " does not exist.");
            return BTreeIO.GetRecord(lastSearchedKey.RecordPointer);
        }

        public IPage<T> GetPage(IPagePointer<T> pointer)
        {
            return BTreeIO.GetPage(pointer);
        }

        public IPage<T> GetRootPage()
        {
            return BTreeIO.GetRootPage();
        }

        public void Set(IKey<T> key, IRecord<T> record)
        {
            if (Searcher.SearchForKey(key))
            {
                BTreeIO.WriteRecord(new Record<T>(record.ValueComponents, Searcher.FoundKey.RecordPointer));
                if (key.Value.Equals(record.Value)) return;
                Remove(key);
                Add(record);
            }
            else if(key.Value.Equals(record.Value))
            {
                Add(record);
            }
            throw new KeyNotFoundException("Key " + key + " not found! Replacing or adding of " + 
                                           record + " is not possible.");
        }

        public IRecord<T> this[IKey<T> key]
        {
            get => Get(key);
            set => Set(key, value);
        }
    }
}