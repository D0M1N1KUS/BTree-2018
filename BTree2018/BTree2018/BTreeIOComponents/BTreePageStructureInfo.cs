using System;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeIOComponents.Converters;

namespace BTree2018.BTreeIOComponents
{
    public class BTreePageStructureInfo<T> where T : IComparable
    {
        /// <summary>Size of type string in bytes</summary>
        public long SIZE_OF_TYPE_STRING { get; private set; }
        /// <summary>Size of the BTree's height property in bytes</summary>
        public long SIZE_OF_HEIGHT_VARIABLE { get; private set; }
        /// <summary>Size of the BTree's D property in bytes</summary>
        public long SIZE_OF_D { get; private set; }
        /// <summary>Size of the variable describing the current number of keys in a BTree page</summary>
        public long KEYS_IN_PAGE_SIZE { get; private set; }
        /// <summary>Size of the BTree page's type property in bytes</summary>
        public long PAGE_TYPE_SIZE { get; private set; }

        /// <summary>Contains the location of the first BTree page pointer's first byte in the file</summary>
        public long LocationOfFirstPointerInFile { get; private set; }
        /// <summary>Contains the location of the first BTree page key's first byte in the file</summary>
        public long LocationOfFirstKeyInFile { get; private set; }
        /// <summary>Contains the location of the Root page pointer's first byte in the file</summary>
        public long LocationOfRootPagePointer { get; private set; }
        /// <summary>Contains the location of the Tree height's first byte in the file</summary>
        public long LocationOfTreeHeight { get; private set; }
        /// <summary>Contains the location of the BTree's D property's first byte in the file</summary>
        public long LocationOfDInFile { get; private set; }
        /// <summary>Contains the offset of a BTree page's first pointer's location from the beginning of the current page</summary>
        public long FirstPointerInPageOffset { get; private set; }
        /// <summary>Contains the offset of a BTree page's first key's location from the beginning of the current page</summary>
        public long FirstKeyInPageOffset { get; private set; }

        /// <summary>Describes the size of the value type in bytes</summary>
        public int SizeOfType { get; private set; }

        /// <summary>Describes the size of a page pointer in bytes</summary>
        public long SizeOfPagePointer { get; private set; }
        /// <summary>Describes the size of a page key in bytes</summary>
        public long SizeOfPageKey { get; private set; }
        /// <summary>Describes the size of a BTree page in bytes</summary>
        public long PageSize { get; private set; }
        /// <summary>Describes the size of the BTree page file's property and information section,
        /// which contains the type string, the tree's height, the D property and the root page pointer</summary>
        public long LengthOfPreamble { get; private set; }
        /// <summary>Describes how many keys can be stored in one BTree page. (2 * D)</summary>
        public long PageLengthN { get; private set; }

        /// <summary>The BTree's D property</summary>
        public long D { get; protected set; }
        /// <summary>Describes the BTree page's length of the info section</summary>
        public long LengthOfPagePreamble { get; private set; }

        public BTreePageStructureInfo(long d, int sizeOfType)
        {
            CalculateSizesAndLocations(d, sizeOfType);
        }

        protected void CalculateSizesAndLocations(long d, int sizeOfType)
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
            LengthOfPagePreamble = KEYS_IN_PAGE_SIZE + SizeOfPagePointer + PAGE_TYPE_SIZE;

            FirstPointerInPageOffset = LengthOfPagePreamble;
            FirstKeyInPageOffset = FirstPointerInPageOffset + SizeOfPagePointer;
            LocationOfFirstPointerInFile = LengthOfPreamble;
            LocationOfFirstKeyInFile = LocationOfFirstPointerInFile + SizeOfPagePointer;
            LocationOfTreeHeight = SIZE_OF_TYPE_STRING;
            LocationOfRootPagePointer = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_VARIABLE + SIZE_OF_D;
            LocationOfDInFile = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_VARIABLE;
        }
    }
}