using System;
using System.Collections.Generic;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents.Converters
{
    //KeysInPage[long], ParentPagePointer[...], PageType[byte], FirstPointer, FirstKey, ...
    public class BTreePageConverter<T> : IBTreePageConversion<T> where T : IComparable
    {
        private const long KEYS_IN_PAGE_SIZE = sizeof(long);
        private const long PAGE_TYPE_SIZE = sizeof(byte);

        private readonly long LocationOfFirstPointer;
        private readonly long LocationOfFirstKey;

        private int sizeOfType;
        
        public readonly long SizeOfPagePointer = BTreePagePointerConverter<T>.SIZE_OF_PAGE_POINTER;
        public readonly long SizeOfPageKey;
        public readonly long PageSize;
        public readonly long PageLengthN;
        public readonly long D;

        public IBTreePagePointerConversion<T> PagePointerConverter = new BTreePagePointerConverter<T>();
        public IBTreeKeyConversion<T> KeyConverter;

        public BTreePageConverter(long d, int sizeOfType)
        {
            this.sizeOfType = sizeOfType;
            
            D = d;
            PageLengthN = 2 * D;
            SizeOfPageKey = sizeOfType + BTreeKeyConverter<T>.SizeOfRecordPointer;
            PageSize = KEYS_IN_PAGE_SIZE + PAGE_TYPE_SIZE + 
                       PageLengthN * SizeOfPageKey + PageLengthN * SizeOfPagePointer + 2 * SizeOfPagePointer;

            LocationOfFirstPointer = KEYS_IN_PAGE_SIZE + SizeOfPagePointer + PAGE_TYPE_SIZE;
            LocationOfFirstKey = LocationOfFirstPointer + SizeOfPagePointer;
            
            KeyConverter = new BTreeKeyConverter<T>(sizeOfType);
        }
        
        
        public IPage<T> ConvertToPage(byte[] bytes, IPagePointer<T> pointerToPage)
        {
            if(bytes.Length != PageSize)
                throw new ArgumentException("Given byte array doest not equal expected page size! Expected " + 
                                            PageSize + "B, but given byte array is " + bytes.Length + "B long.");
            
            var keysInPage = BitConverter.ToInt64(bytes, 0);
            
            var pageBuilder = new BTreePageBuilder<T>((int) PageLengthN)
                .SetPagePointer(pointerToPage)
                .SetParentPagePointer(PagePointerConverter.ConvertToPointer(bytes, (int) KEYS_IN_PAGE_SIZE));
            
            var pageType = (PageType)bytes[KEYS_IN_PAGE_SIZE + SizeOfPagePointer];    
            pageBuilder.SetPageType(pageType);

            var byteArrayPointer = (int)LocationOfFirstPointer;
            if(pageType != PageType.LEAF && keysInPage > 0)
                pageBuilder.AddPointer(PagePointerConverter.ConvertToPointer(bytes, byteArrayPointer));
            byteArrayPointer += (int)SizeOfPagePointer;
            for (var i = 0; i < keysInPage; i++)
            {
                pageBuilder.AddKey(KeyConverter.ConvertToKey(bytes, byteArrayPointer));
                if (pageType == PageType.LEAF) byteArrayPointer += (int)SizeOfPageKey + (int)SizeOfPagePointer;
                else
                {
                    byteArrayPointer += (int) SizeOfPageKey;
                    pageBuilder.AddPointer(PagePointerConverter.ConvertToPointer(bytes, byteArrayPointer));
                    byteArrayPointer += (int) SizeOfPagePointer;
                }
            }

            return pageBuilder.Build();
        }

        public byte[] ConvertToBytes(IPage<T> page)
        {
            if(page.KeysInPage > PageLengthN)
                throw new ArgumentException("The size of the page" + page + 
                                            " is not suitable for given file D(" + D + ")");
            
            var byteList = new List<byte>((int)PageSize);
            byteList.AddRange(BitConverter.GetBytes(page.KeysInPage));
            byteList.AddRange(PagePointerConverter.ConvertToBytes(page.ParentPage));
            byteList.Add((byte)page.PageType);
            
            if(page.KeysInPage > 0) byteList.AddRange(PagePointerConverter.ConvertToBytes(page.PointerAt(0)));
            for (var i = 0; i < PageLengthN; i++)
            {
                byteList.AddRange(KeyConverter.ConvertToBytes(i < page.KeysInPage
                    ? page.KeyAt(i)
                    : BTreeKey<T>.NullKey));
                byteList.AddRange(PagePointerConverter.ConvertToBytes(i < page.KeysInPage 
                    ? page.PointerAt(i + 1) 
                    : BTreePagePointer<T>.NullPointer));
            }
            
            return byteList.ToArray();
        }
    }
}