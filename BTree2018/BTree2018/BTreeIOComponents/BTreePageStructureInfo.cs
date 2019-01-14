using System;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeIOComponents.Converters;

namespace BTree2018.BTreeIOComponents
{
    public class BTreePageStructureInfo<T> where T : IComparable
    {
        /// <summary>Size of type string in bytes</summary>
        public readonly long SIZE_OF_TYPE_STRING;
        /// <summary>Size of the BTree's height property in bytes</summary>
        public readonly long SIZE_OF_HEIGHT_VARIABLE;
        /// <summary>Size of the BTree's D property in bytes</summary>
        public readonly long SIZE_OF_D;
        /// <summary>Size of the variable describing the current number of keys in a BTree page</summary>
        public readonly long KEYS_IN_PAGE_SIZE;
        /// <summary>Size of the BTree page's type property in bytes</summary>
        public readonly long PAGE_TYPE_SIZE;

        /// <summary>Contains the location of the first BTree page pointer's first byte in the file</summary>
        public readonly long LocationOfFirstPointer;
        /// <summary>Contains the location of the first BTree page key's first byte in the file</summary>
        public readonly long LocationOfFirstKey;
        /// <summary>Contains the location of the Root page pointer's first byte in the file</summary>
        public readonly long LocationOfRootPagePointer;
        /// <summary>Contains the location of the Tree height's first byte in the file</summary>
        public readonly long LocationOfTreeHeight;
        /// <summary>Contains the location of the BTree's D property's first byte in the file</summary>
        public readonly long LocationOfD;

        /// <summary>Describes the size of the value type in bytes</summary>
        public readonly int SizeOfType;

        /// <summary>Describes the size of a page pointer in bytes</summary>
        public readonly long SizeOfPagePointer;
        /// <summary>Describes the size of a page key in bytes</summary>
        public readonly long SizeOfPageKey;
        /// <summary>Describes the size of a BTree page in bytes</summary>
        public readonly long PageSize;
        /// <summary>Describes the size of the BTree page file's property and information section,
        /// which contains the type string, the tree's height, the D property and the root page pointer</summary>
        public readonly long LengthOfPreamble;
        /// <summary>Describes how many keys can be stored in one BTree page. (2 * D)</summary>
        public readonly long PageLengthN;
        /// <summary>The BTree's D property</summary>
        public readonly long D;

        public BTreePageStructureInfo(long d, int sizeOfType)
        {
            SIZE_OF_TYPE_STRING = 64 * sizeof(byte);
            SIZE_OF_HEIGHT_VARIABLE = sizeof(long);  
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

            LengthOfPreamble = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_VARIABLE + SIZE_OF_D + SizeOfPagePointer;
            
            LocationOfFirstPointer = LengthOfPreamble;
            LocationOfFirstKey = LocationOfFirstPointer + SizeOfPagePointer;
            LocationOfTreeHeight = SIZE_OF_TYPE_STRING;
            LocationOfRootPagePointer = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_VARIABLE + SIZE_OF_D;
            LocationOfD = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_VARIABLE;
        }
    }
}