using System;
using System.Collections.Generic;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents.BTreeFileClasses
{
    //TypeString[64*char], H[long], D[long] RootPagePointer[...]
    public class BTreePageFile<T> : IBTreePageFile<T> where T : IComparable
    {
        private const long SIZE_OF_TYPE_STRING = 64 * sizeof(char);
        private const long SIZE_OF_HEIGHT_CONSTANT = sizeof(long);
        private const long SIZE_OF_D = sizeof(long);
        
        private readonly long SizeOfPagePointer = BTreePagePointerConverter<T>.SIZE_OF_PAGE_POINTER;
        private readonly long SizeOfPageKey;
        private readonly long PageSize;
        private readonly long LengthOfPreamble;

        private readonly long pageLengthN;
        
        private IFileIO FileIO;
        private IFileBitmap FileMap;
        public IBTreePageConversion<T> PageConverter;
        public IBTreePagePointerConversion<T> PagePointerConverter;
        private int sizeOfType;
        
        public long TreeHeight { get; private set; }
        public IPagePointer<T> RootPage { get; private set; }
        public long D { get; private set; }

        /// <summary>
        /// Creates new file with empty root.
        /// </summary>
        /// <param name="fileIO">IFileIO object initialized with empty file</param>
        /// <param name="fileMap">IFileBitmap object initialized with map file</param>
        /// <param name="sizeOfType">Size of generic value type</param>
        /// <param name="d">d - parameter of the BTree (2d = max. amount of keys in page)</param>
        public BTreePageFile(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType, long d)
        {
            initializeObjectFields(fileIO, fileMap, sizeOfType);

            TreeHeight = 1;
            D = d;
            pageLengthN = 2 * D;
            
            SizeOfPageKey = sizeOfType + BTreeKeyConverter<T>.SizeOfRecordPointer;
            PageSize = pageLengthN * SizeOfPageKey + pageLengthN * SizeOfPagePointer + 2 * SizeOfPagePointer;
            LengthOfPreamble = SIZE_OF_TYPE_STRING + SIZE_OF_HEIGHT_CONSTANT + SIZE_OF_D + SizeOfPagePointer;
            
            writeInitialValuesToFile();
        }

        /// <summary>
        /// Loads values from existing BTree file.
        /// </summary>
        /// <param name="fileIO">IFileIO object initialized with existing file</param>
        /// <param name="fileMap">IFileBitmap object initialized with map file</param>
        /// <param name="sizeOfType">Size of generic value type</param>
        public BTreePageFile(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType)
        {
            initializeObjectFields(fileIO, fileMap, sizeOfType);

            TreeHeight = BitConverter.ToInt64(FileIO.GetBytes(0, SIZE_OF_HEIGHT_CONSTANT), 0);
            D = BitConverter.ToInt64(FileIO.GetBytes(SIZE_OF_HEIGHT_CONSTANT, SIZE_OF_D), 0);
            pageLengthN = 2 * D;
            RootPage = PagePointerConverter.ConvertToPointer(FileIO.GetBytes(SIZE_OF_HEIGHT_CONSTANT + SIZE_OF_D,
                SizeOfPagePointer));
            
            SizeOfPageKey = sizeOfType + BTreeKeyConverter<T>.SizeOfRecordPointer;
            PageSize = 2 * D * SizeOfPageKey + 2 * SizeOfPagePointer + SizeOfPagePointer;
        }
        
        private void initializeObjectFields(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType)
        {
            FileIO = fileIO;
            FileMap = fileMap;
            this.sizeOfType = sizeOfType;
        }

        private void writeInitialValuesToFile()
        {
            var bytesList = new List<byte>((int) LengthOfPreamble);
            bytesList.AddRange(TypeConverter<T>.TypeTo64ByteString());
            bytesList.AddRange(BitConverter.GetBytes(D));
            bytesList.AddRange(BitConverter.GetBytes((long)1));
            bytesList.AddRange(PagePointerConverter.ConvertToBytes(new BTreePagePointer<T>()
                {Index = 0, PointsToPageType = PageType.ROOT}));
            FileIO.WriteBytes(bytesList.ToArray(), 0);
        }

        
        public IPage<T> this[IPagePointer<T> index] => throw new NotImplementedException();

        public IPagePointer<T> this[IPage<T> page]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IPage<T> PageAt(IPagePointer<T> index)
        {
           throw new NotImplementedException();
        }

        public void SetPage(IPage<T> page)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(IPage<T> page)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(IPagePointer<T> pointer)
        {
            throw new NotImplementedException();
        }
    }
}