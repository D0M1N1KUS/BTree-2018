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
    public class BTreePageConverter<T> : BTreePageStructureInfo<T>, IBTreePageConversion<T> where T : IComparable
    {
        public IBTreePagePointerConversion<T> PagePointerConverter = new BTreePagePointerConverter<T>();
        public IBTreeKeyConversion<T> KeyConverter;

        public BTreePageConverter(long d, int sizeOfType) : base(d, sizeOfType)
        {
            KeyConverter = new BTreeKeyConverter<T>(sizeOfType);
        }
        
        //TODO: fill up page with zeros or change the converter to output a constant page size
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