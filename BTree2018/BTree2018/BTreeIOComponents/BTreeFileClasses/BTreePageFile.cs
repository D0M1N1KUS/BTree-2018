using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BTree2018.BTreeIOComponents.Basics;
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
        public IFileIO FileIO;
        public IFileBitmap FileMap;
        public IBTreePageConversion<T> PageConverter;
        public IBTreePagePointerConversion<T> PagePointerConverter;
        
        public long TreeHeight { get; private set; }
        public IPagePointer<T> RootPage { get; private set; }
        public long D { get; private set; }

        /// <summary>
        /// Creates new file with empty root.
        /// </summary>
        /// <param name="sizeOfType">Size of generic value type</param>
        /// <param name="d">d - parameter of the BTree (2d = max. amount of keys in page)</param>
        /// <param name="pageFilePath">Location of the new page file</param>
        /// <param name="pageFileMapPath">Location of the new map file</param>
        public BTreePageFile(int sizeOfType, long d, string pageFilePath, string pageFileMapPath) : base(d, sizeOfType)
        {
            TreeHeight = 1;
            RootPage = new BTreePagePointer<T>() { Index = 0, PointsToPageType = PageType.ROOT };
            D = d;
            FileIO = new FileIO(pageFilePath);
            FileMap = new FileMap(pageFileMapPath);
            PageConverter = new BTreePageConverter<T>(d, sizeOfType);
            PagePointerConverter = new BTreePagePointerConverter<T>();
            writeInitialValuesToFile();
        }
        
        /// <summary>
        /// Creates new file with empty root.
        /// </summary>
        /// <param name="sizeOfType">Size of generic value type</param>
        /// <param name="d">d - parameter of the BTree (2d = max. amount of keys in page)</param>
        /// <param name="fileIO">IFileIO object initialized with existing file</param>
        /// <param name="fileMap">IFileBitmap object initialized with map file</param>
        /// <param name="pageConverter">IBTreePageConversion object</param>
        /// <param name="pagePointerConverter">IBTreePagePointerConversion object</param>
        public BTreePageFile(int sizeOfType, long d, IFileIO fileIO, IFileBitmap fileMap,
            IBTreePageConversion<T> pageConverter, IBTreePagePointerConversion<T> pagePointerConverter)
            : base(d, sizeOfType)
        {
            TreeHeight = 1;
            RootPage = new BTreePagePointer<T>() { Index = 0, PointsToPageType = PageType.ROOT };
            D = d;
            FileIO = fileIO;
            FileMap = fileMap;
            PageConverter = pageConverter;
            PagePointerConverter = pagePointerConverter;
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

            TreeHeight = BitConverter.ToInt64(FileIO.GetBytes(LocationOfTreeHeight, SIZE_OF_HEIGHT_VARIABLE),
                0);
            D = BitConverter.ToInt64(FileIO.GetBytes(LocationOfDInFile, SIZE_OF_D), 0);
            RootPage = PagePointerConverter.ConvertToPointer(FileIO.GetBytes(LocationOfRootPagePointer,
                SizeOfPagePointer));
            
            PageConverter = new BTreePageConverter<T>(D, sizeOfType);
            PagePointerConverter = new BTreePagePointerConverter<T>();
        }
        
        private void initializeObjectFields(IFileIO fileIO, IFileBitmap fileMap, int sizeOfType)
        {
            FileIO = fileIO;
            FileMap = fileMap;
        }

        private void writeInitialValuesToFile()
        {
            if(FileIO == null || FileMap == null || PageConverter == null || PagePointerConverter == null)
                throw new Exception("Object is not initialized properly!");
            var bytesList = new List<byte>((int) LengthOfPreamble);
            bytesList.AddRange(TypeConverter<T>.TypeTo64ByteString());
            bytesList.AddRange(BitConverter.GetBytes(TreeHeight));
            bytesList.AddRange(BitConverter.GetBytes(D));
            bytesList.AddRange(PagePointerConverter.ConvertToBytes(RootPage));
            bytesList.AddRange(PageConverter.ConvertToBytes(
                BTreePageBuilder<T>.BuildEmptyPage(PageLengthN, PageType.ROOT, RootPage)));
            FileIO.WriteBytes(bytesList.ToArray(), 0);
            FileMap[RootPage.Index] = true;
        }

        public IPage<T> PageAt(IPagePointer<T> pointer)
        {
            checkPointer(pointer);
            try
            {
                var occupied = FileMap[pointer.Index];
                return occupied 
                    ? PageConverter.ConvertToPage(FileIO.GetBytes(LengthOfPreamble + PageSize * pointer.Index, 
                        PageSize), pointer) 
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
                LengthOfPreamble + PageSize * page.PagePointer.Index);
        }

        public IPagePointer<T> AddNewRootPage(IPage<T> newRootPage)
        {
            if(newRootPage.PageType != PageType.ROOT) throw new Exception("The page " + newRootPage + 
                                                                          " is not marked as a root page");

            var newRootPagePointer = AddNewPage(newRootPage);
            FileIO.WriteBytes(PagePointerConverter.ConvertToBytes(newRootPagePointer), LocationOfRootPagePointer);
            RootPage = newRootPagePointer;
            return newRootPagePointer;
        }

        public void SetTreeHeight(long newTreeHeight)
        {
            TreeHeight = newTreeHeight;
            FileIO.WriteBytes(BitConverter.GetBytes(newTreeHeight), LocationOfTreeHeight);
        }

        public IPagePointer<T> AddNewPage(IPage<T> page)
        {
            if(page.PagePointer != null && !page.PagePointer.Equals(BTreePagePointer<T>.NullPointer))
                Logger.Log("BTreePageFile warning: Adding page with already initialized pointer - " + page);
            
            var newPagePointer = new BTreePagePointer<T>()
                {Index = FileMap.GetNextFreeIndex(), PointsToPageType = page.PageType};
            FileIO.WriteBytes(PageConverter.ConvertToBytes(page), 
                LengthOfPreamble + PageSize * newPagePointer.Index);
            FileMap[newPagePointer.Index] = true;
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