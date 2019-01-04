using System;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations 
{
    public class BTreeLeafKeyRemover<T> : IBTreeLeafKeyRemoval<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        
        public IKey<T> RemoveBiggestKey(IPage<T> beginningPage, out IPage<T> modifiedLeafPage)
        {
            var currentPage = beginningPage;
            while (currentPage.PageType != PageType.LEAF)
            {
                currentPage = BTreeIO.GetPage(currentPage.PointerAt(currentPage.KeysInPage));
            }

            var newLeafPage = new BTreePageBuilder<T>((int) currentPage.Length)
                .CreateEmptyCloneFromPage(currentPage)
                .AddPointer(currentPage.PointerAt(0));

            for (var i = 0; i < currentPage.KeysInPage - 1; i++)
            {
                newLeafPage.AddKey(currentPage.KeyAt(i));
                newLeafPage.AddPointer(currentPage.PointerAt(i + 1));
            }

            modifiedLeafPage = newLeafPage.Build();

            return currentPage.KeyAt(currentPage.KeysInPage - 1);
        }

        public IKey<T> RemoveSmallestKey(IPage<T> beginningPage, out IPage<T> modifiedLeafPage)
        {
            var currentPage = beginningPage;
            while (currentPage.PageType != PageType.LEAF)
            {
                currentPage = BTreeIO.GetPage(currentPage.PointerAt(0));
            }

            var newLeafPage = new BTreePageBuilder<T>((int) currentPage.Length)
                .CreateEmptyCloneFromPage(currentPage);

            for (var i = 1; i < currentPage.KeysInPage; i++)
            {
                newLeafPage.AddKey(currentPage.KeyAt(i));
                newLeafPage.AddPointer(currentPage.PointerAt(i));
            }
            newLeafPage.AddPointer(currentPage.PointerAt(currentPage.KeysInPage));

            modifiedLeafPage = newLeafPage.Build();

            return currentPage.KeyAt(0);
        }

    }
}