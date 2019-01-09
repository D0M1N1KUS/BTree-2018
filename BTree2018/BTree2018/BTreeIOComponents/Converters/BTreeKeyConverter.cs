using System;
using System.Collections.Generic;
using System.ComponentModel;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents.BTreeFileClasses
{
    //Value[2-8 bytes], index[8 bytes (long)]
    public class BTreeKeyConverter<T> : IBTreeKeyConversion<T> where T : IComparable
    {
        private const long SIZE_OF_RECORD_POINTER = sizeof(long);

        private byte[] Bytes;
        private int SizeOfValue;

        private long sizeOfKey => SizeOfValue + SIZE_OF_RECORD_POINTER;
        
        public static long SizeOfRecordPointer => SIZE_OF_RECORD_POINTER;
        
        public IKey<T> ConvertToKey(byte[] bytes, int begin, int sizeOfValue)
        {
            Bytes = bytes;
            SizeOfValue = sizeOfValue;
            //TODO: determine if this checking is really necessary, since other objects are checking if the byte array is ok
//            if(bytes.Length != sizeOfKey)
//                throw new ArgumentException(bytes.Length + " is not the appropriate size of a key (with pointer).");
            
            var index = BitConverter.ToInt64(bytes, begin + sizeOfValue);
            
            return new BTreeKey<T>()
            {
                Value = getValue(),
                RecordPointer = index >= 0 
                    ? new RecordPointer<T>() { Index = index, PointerType = RecordPointerType.NOT_NULL }
                    : RecordPointer<T>.NullPointer
            };
        }

        private T getValue()
        {
            var valueBytes = new byte[SizeOfValue];
            Array.Copy(Bytes, 0, valueBytes, 0, SizeOfValue);
            return TypeConverter<T>.ToValue(valueBytes);
        }

        public byte[] ConvertToBytes(IKey<T> key, int sizeOfValue)
        {
            var byteList = new List<byte>((int)sizeOfKey);
            byteList.AddRange(TypeConverter<T>.ToBytes(key.Value));
            byteList.AddRange(BitConverter.GetBytes(key.RecordPointer.Index));
            return byteList.ToArray();
        }
    }
}