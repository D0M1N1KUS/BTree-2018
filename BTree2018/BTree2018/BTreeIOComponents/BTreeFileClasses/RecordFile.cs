using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeIOComponents.BTreeFileClasses
{
    public class RecordFile<T> : IRecordFile<T> where T : IComparable
    {
        private const long MAX_VALUES_IN_RECORD = 15;
        private const long TYPE_STRING_PREAMBLE_SIZE = 64 * sizeof(char);
        
        public IFileIO FileIO;
        public IFileBitmap FileMap;

        private readonly int sizeOfType;

        public RecordFile(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType, bool createNewRecordFile = false)
        {
            FileIO = fileIO;
            this.sizeOfType = sizeOfType;
            FileMap = fileMap;
            var typeStringBytes = FileIO.GetBytes(0, TYPE_STRING_PREAMBLE_SIZE);
            if (typeStringBytes.Length == TYPE_STRING_PREAMBLE_SIZE)
            {
                var typeOfRecordsInFile = TypeConverter<T>.TypeStringToType(typeStringBytes);
                if(typeof(T) != typeOfRecordsInFile) throw new Exception("Record file of type [" + typeOfRecordsInFile +
                    "] does not match the current object type [" + typeof(T) + "]");
            }
            else if (createNewRecordFile)
            {
                FileIO.WriteBytes(TypeConverter<T>.TypeTo64ByteString(), 0);
            }
        }
        
        public IRecord<T> this[IRecordPointer<T> pointer]
        {
            get => GetRecord(pointer);
            set { 
                if(value.RecordPointer != pointer)
                    throw new Exception("Record pointers differ: " + value + " " + pointer);
                SetRecord(value); 
            }
        }

        public IRecord<T> GetRecord(IRecordPointer<T> pointer)
        {
            if(pointer.PointerType == RecordPointerType.NULL) throw new NullReferenceException(pointer.ToString());
            if(pointer.Index < FileMap.CurrentMapSize && !FileMap[pointer.Index])
                Logger.Log("RecordFile: Possible inconsistency detected at " + pointer);
            var bytes = FileIO.GetBytes(pointer.Index + TYPE_STRING_PREAMBLE_SIZE, sizeOfType * MAX_VALUES_IN_RECORD);
            return new Record<T>(byteArrayToRecord(bytes), pointer);
        }
        

        public void SetRecord(IRecord<T> record)
        {
            if (record.RecordPointer != RecordPointer<T>.NullPointer)
                throw new NullReferenceException(record.ToString());
            FileIO.WriteBytes(recordToByteArray(record.ValueComponents), record.RecordPointer.Index);
        }

        public IRecordPointer<T> AddRecord(IRecord<T> record)
        {
            if (record.RecordPointer != RecordPointer<T>.NullPointer)
                throw new NullReferenceException(record.ToString());
            var index = record.RecordPointer.Index;
            if (index >= FileMap.CurrentMapSize || FileMap[index])
                index = FileMap.GetNextFreeIndex();
            FileIO.WriteBytes(recordToByteArray(record.ValueComponents), index);
            FileMap[index] = true;
            return new RecordPointer<T>(){Index = index, PointerType = RecordPointerType.NOT_NULL};
        }

        public void RemoveRecord(IRecord<T> record)
        {
            RemoveRecord(record.RecordPointer);
        }

        public void RemoveRecord(IRecordPointer<T> pointer)
        {
            if(pointer.PointerType == RecordPointerType.NULL)
                throw new NullReferenceException();
            FileMap[pointer.Index] = false;
        }

        private byte[] recordToByteArray(IReadOnlyCollection<T> valueComponents)
        {
            var bytesList = new List<byte>(valueComponents.Count);
            foreach (var value in valueComponents)
            {
                bytesList.AddRange(TypeConverter<T>.ToBytes(value));
            }
            for (var i = 0; i < MAX_VALUES_IN_RECORD - valueComponents.Count; i++)
            {
                bytesList.AddRange(TypeConverter<T>.ToBytes((T)(object)0));
            }

            return bytesList.ToArray();
        }

        private T[] byteArrayToRecord(byte[] bytes)
        {
            if (bytes.Length != sizeOfType * MAX_VALUES_IN_RECORD)
                throw new Exception("A byte array of " + bytes.Length + " values is not suitable for a record!");
            var valueComponents = new List<T>((int)MAX_VALUES_IN_RECORD);
            for (var i = 0; i < MAX_VALUES_IN_RECORD; i++)
            {
                var j = i * sizeOfType;
                var subBytes = new byte[sizeOfType];
                Array.Copy(bytes, j, subBytes, 0, sizeOfType);
                valueComponents.Add(TypeConverter<T>.ToValue(subBytes));
            }

            return valueComponents.ToArray();
        }
    }
}