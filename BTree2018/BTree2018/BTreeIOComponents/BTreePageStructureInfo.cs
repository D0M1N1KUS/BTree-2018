using System;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeIOComponents.Converters;

namespace BTree2018.BTreeIOComponents
{
    public class BTreePageStructureInfo<T> where T : IComparable
    {
        public readonly long SIZE_OF_TYPE_STRING;
        public readonly long SIZE_OF_HEIGHT_CONSTANT;
        public readonly long SIZE_OF_D;
        public readonly long KEYS_IN_PAGE_SIZE;
        public readonly long PAGE_TYPE_SIZE;

        public readonly long LocationOfFirstPointer;
        public readonly long LocationOfFirstKey;
        public readonly long LocationOfRootPagePointer;
        public readonly long LocationOfTreeHeight;
        public readonly long LocationOfD;

        public readonly int SizeOfType;

        public readonly long SizeOfPagePointer;
        public readonly long SizeOfPageKey;
        public readonly long PageSize;
        public readonly long LengthOfPreamble;
        public readonly long PageLengthN;
        public readonly long D;

        public BTreePageStructureInfo(long d, int sizeOfType)
        {
            SIZE_OF_TYPE_STRING = 64 * sizeof(char);
            SIZE_OF_HEIGHT_CONSTANT = sizeof(long);  
            SIZE_OF_D = sizeof(long);                
            KEYS_IN_PAGE_SIZE = sizeof(long);  
            PAGE_TYPE_SIZE = sizeof(byte);     
            SizeOfType = sizeOfType;
            D = d;
            PageLengthN = 2 * D;
            SizeOfPageKey = sizeOfType + BTreeKeyConverter<T>.SizeOfRecordPointer;
            SizeOfPagePointer = BTreePagePointerConverter<T>.SIZE_OF_PAGE_POINTER;
            PageSize = KEYS_IN_PAGE_SIZE + PAGE_TYPE_SIZE + 
                       PageLengthN * SizeOfPageKey + PageLengthN * SizeOfPagePointer + 2 * SizeOfPagePointer;

            LengthOfPreamble = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_CONSTANT + SIZE_OF_D + SizeOfPagePointer;
            
            LocationOfFirstPointer = KEYS_IN_PAGE_SIZE + SizeOfPagePointer + PAGE_TYPE_SIZE;
            LocationOfFirstKey = LocationOfFirstPointer + SizeOfPagePointer;
            LocationOfTreeHeight = SIZE_OF_TYPE_STRING;
            LocationOfRootPagePointer = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_CONSTANT + SIZE_OF_D;
            LocationOfD = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_CONSTANT;
        }
    }
}