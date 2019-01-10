using System;
using System.Collections.Generic;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents.Converters
{
    //index[long], pointsToPageType[1]
    public class BTreePagePointerConverter<T> : IBTreePagePointerConversion<T> where T : IComparable
    {
        private const long SizeOfPagePointer = sizeof(long) + 1;

        public static long SIZE_OF_PAGE_POINTER => SizeOfPagePointer;

        public IPagePointer<T> ConvertToPointer(byte[] bytes, int begin = 0)
        {
            return new BTreePagePointer<T>()
            {
                Index = BitConverter.ToInt64(bytes, begin),
                PointsToPageType = (PageType) bytes[begin + SIZE_OF_PAGE_POINTER - 1]
            };
        }

        public byte[] ConvertToBytes(IPagePointer<T> pointer)
        {
            var bytesList = new List<byte>((int)SIZE_OF_PAGE_POINTER);
            bytesList.AddRange(BitConverter.GetBytes(pointer.Index));
            bytesList.Add((byte)pointer.PointsToPageType);
            return bytesList.ToArray();
        }
    }
}