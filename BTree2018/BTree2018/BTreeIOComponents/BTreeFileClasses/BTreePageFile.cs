using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeIOComponents.BTreeFileClasses
{
    //TypeString[64*char], H[long], D[long] RootPagePointer[...], Pages[...]
    public class BTreePageFile<T> : BTreePageStructureInfo<T>, IBTreePageFile<T> where T : IComparable
    {
        private IFileIO FileIO;
        private IFileBitmap FileMap;
        public IBTreePageConversion<T> PageConverter;
        public IBTreePagePointerConversion<T> PagePointerConverter;
        
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
        public BTreePageFile(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType, long d) : base(d, sizeOfType)
        {
            initializeObjectFields(fileIO, fileMap, sizeOfType);

            TreeHeight = 1;
            D = d;
            
            writeInitialValuesToFile();
        }

        /// <summary>
        /// Loads values from existing BTree file.
        /// </summary>
        /// <param name="fileIO">IFileIO object initialized with existing file</param>
        /// <param name="fileMap">IFileBitmap object initialized with map file</param>
        /// <param name="sizeOfType">Size of generic value type</param>
        public BTreePageFile(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType) : base(0, sizeOfType)
        {
            initializeObjectFields(fileIO, fileMap, sizeOfType);

            TreeHeight = BitConverter.ToInt64(FileIO.GetBytes(0, SIZE_OF_HEIGHT_CONSTANT), 0);
            D = BitConverter.ToInt64(FileIO.GetBytes(SIZE_OF_HEIGHT_CONSTANT, SIZE_OF_D), 0);
            RootPage = PagePointerConverter.ConvertToPointer(FileIO.GetBytes(SIZE_OF_HEIGHT_CONSTANT + SIZE_OF_D,
                SizeOfPagePointer));
        }
        
        private void initializeObjectFields(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType)
        {
            FileIO = fileIO;
            FileMap = fileMap;
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

        public IPage<T> PageAt(IPagePointer<T> pointer)
        {
            checkPointer(pointer);
            try
            {
                var occupied = FileMap[pointer.Index];
                return occupied 
                    ? PageConverter.ConvertToPage(FileIO.GetBytes(pointer.Index, PageSize), pointer) 
                    : BTreePageBuilder<T>.BuildNullPage();
            }
            catch (ArgumentException e)
            {
                throw new Exception("Pointer points out of range" + pointer, e);
            }
            catch (Exception e)
            {
                throw new Exception("BTreePageFile FATAL: " + e.Message, e);
            }
        }

        public void SetPage(IPage<T> page)
        {
            checkPage(page);
            FileMap[page.PagePointer.Index] = true;
            FileIO.WriteBytes(
                PageConverter.ConvertToBytes(page),
                LengthOfPreamble + page.PagePointer.Index);
        }

        public IPagePointer<T> AddNewPage(IPage<T> page)
        {
            if(page.PagePointer != null && !page.PagePointer.Equals(BTreePagePointer<T>.NullPointer))
                Logger.Log("BTreePageFile warning: Adding page with already initialized pointer - " + page);
            
            var newPagePointer = new BTreePagePointer<T>()
                {Index = FileMap.GetNextFreeIndex(), PointsToPageType = page.PageType};
            FileIO.WriteBytes(PageConverter.ConvertToBytes(page), newPagePointer.Index);
            return newPagePointer;
        }

        public void RemovePage(IPage<T> page)
        {
            checkPage(page);
            FileMap[page.PagePointer.Index] = false;
        }

        public void RemovePage(IPagePointer<T> pointer)
        {
            checkPointer(pointer);
            FileMap[pointer.Index] = false;
        }
        
        private static void checkPointer(IPagePointer<T> pointer)
        {
            if (pointer == null || pointer.Equals(BTreePagePointer<T>.NullPointer))
                throw new NullReferenceException("Tried to access page file with pointer marked as NULL: " +
                                                 pointer ?? string.Empty);
        }

        private static void checkPage(IPage<T> page)
        {
            if (page.PagePointer == null || page.PagePointer.Equals(BTreePagePointer<T>.NullPointer))
                throw new NullReferenceException("Tried to access page file with pointer marked as NULL " +
                                                 page.PagePointer ?? string.Empty);
        }
    }
}