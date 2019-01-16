using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeIOComponents.BTreeFileClasses
{
    //TypeString[64*char]
    public class RecordFile<T> : IRecordFile<T> where T : IComparable
    {
        private const long MAX_VALUES_IN_RECORD = 15;
        private const long TYPE_STRING_PREAMBLE_SIZE = 64 * sizeof(byte);
        private readonly long SizeOfRecord;
        
        public IFileIO FileIO;
        public IFileBitmap FileMap;

        private readonly int sizeOfType;

        /// <summary>
        /// For creating a new record file
        /// </summary>
        /// <param name="sizeOfType"></param>
        public RecordFile(int sizeOfType)
        {
            this.sizeOfType = sizeOfType;
            SizeOfRecord = MAX_VALUES_IN_RECORD * sizeOfType;
        }

        public void WriteInitialValuesToFile()
        {
            FileIO.WriteBytes(TypeConverter<T>.TypeTo64ByteString(), 0);
        }
        
        /// <summary>
        /// Opens existing record file
        /// </summary>
        /// <param name="fileIO">Initialized FileIO object</param>
        /// <param name="fileMap">Initialized FileBitmap object</param>
        /// <param name="sizeOfType">Size of file type</param>
        /// <exception cref="Exception"></exception>
        public RecordFile(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType)
        {
            FileIO = fileIO;
            FileMap = fileMap;
            this.sizeOfType = sizeOfType;
            SizeOfRecord = MAX_VALUES_IN_RECORD * sizeOfType;
            var typeStringBytes = FileIO.GetBytes(0, TYPE_STRING_PREAMBLE_SIZE);
            var typeOfRecordsInFile = TypeConverter<T>.TypeStringToType(typeStringBytes);
            if(typeof(T) != typeOfRecordsInFile) throw new Exception("Record file of type [" + typeOfRecordsInFile +
                "] does not match the current object type [" + typeof(T) + "]");
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
            var bytes = FileIO.GetBytes(pointer.Index * SizeOfRecord + 
                                        TYPE_STRING_PREAMBLE_SIZE, sizeOfType * MAX_VALUES_IN_RECORD);
            return new Record<T>(byteArrayToRecord(bytes), pointer);
        }
        

        public void SetRecord(IRecord<T> record)
        {
            if (record.RecordPointer != RecordPointer<T>.NullPointer)
                throw new NullReferenceException(record.ToString());
            FileIO.WriteBytes(recordToByteArray(record.ValueComponents), 
                TYPE_STRING_PREAMBLE_SIZE + record.RecordPointer.Index * SizeOfRecord);
        }

        public IRecordPointer<T> AddRecord(IRecord<T> record)
        {
            var index = getIndexForNewRecord(record); 
            FileIO.WriteBytes(recordToByteArray(record.ValueComponents), 
                TYPE_STRING_PREAMBLE_SIZE + index * SizeOfRecord);
            FileMap[index] = true;
            return new RecordPointer<T>(){Index = index, PointerType = RecordPointerType.NOT_NULL};
        }

        private long getIndexForNewRecord(IRecord<T> record)
        {
            long index;
            if (!record.RecordPointer.Equals(RecordPointer<T>.NullPointer))
            {
                Logger.Log("RecordFile warning: Adding record with already initialized pointer. " + record);
                index = record.RecordPointer.Index;
                if (index >= FileMap.CurrentMapSize || FileMap[index])
                    index = FileMap.GetNextFreeIndex();
            }
            else
                index = FileMap.GetNextFreeIndex();

            return index;
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

        private byte[] recordToByteArray(T[] valueComponents)
        {
            var bytesList = new List<byte>(valueComponents.Length);
            foreach (var value in valueComponents)
            {
                bytesList.AddRange(TypeConverter<T>.ToBytes(value));
            }
            for (var i = 0; i < MAX_VALUES_IN_RECORD - valueComponents.Length; i++)
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