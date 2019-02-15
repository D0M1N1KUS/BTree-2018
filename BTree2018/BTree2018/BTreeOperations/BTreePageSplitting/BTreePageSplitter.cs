using System;
using System.Windows.Controls;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeOperations.BTreeSplitting
{
    public class BTreePageSplitter<T> : IBTreeSplitting<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        public IBTreeAdding<T> BTreeAdding;
        
        public IPage<T> Split(IPage<T> page)
        {
            checkPage(page);
            return page.PageType == PageType.ROOT 
                ? splitRootPage(page, page.KeysInPage / 2) 
                : splitNonRootPage(page, page.KeysInPage / 2);
        }

        private IPage<T> splitNonRootPage(IPage<T> page, long keysInSplittedPages)
        {
            var leftPageBuilder = new BTreePageBuilder<T>((int)page.PageLength)
                .SetPagePointer(page.PagePointer)
                .SetParentPagePointer(page.ParentPage)
                .SetPageType(page.PageType);
            var rightPageBuilder = new BTreePageBuilder<T>((int)page.PageLength)
                .SetPagePointer(BTreePagePointer<T>.NullPointer)
                .SetParentPagePointer(page.ParentPage)
                .SetPageType(page.PageType);

            distributeKeysAndPointers(page, leftPageBuilder, rightPageBuilder, keysInSplittedPages);

            var leftPagePointer = BTreeIO.WritePage(leftPageBuilder.Build());
            var rightPagePointer = BTreeIO.WritePage(rightPageBuilder.Build());
            
            updateParentPagePointersAfterSplit(leftPagePointer);
            updateParentPagePointersAfterSplit(rightPagePointer);
            
            var modifiedParentPage = BTreeAdding.InsertKeyIntoPage(BTreeIO.GetPage(page.ParentPage),
                page.KeyAt(keysInSplittedPages), rightPagePointer);
            if(!modifiedParentPage.OverFlown) BTreeIO.WritePage(modifiedParentPage);
            return modifiedParentPage;
        }

        

        private IPage<T> splitRootPage(IPage<T> rootPage, long keysInSplittedPages)
        {
            var newChildrenPageType = BTreeIO.H < 2 ? PageType.LEAF : PageType.BRANCH;
            
            var leftPageBuilder = new BTreePageBuilder<T>((int)rootPage.PageLength)
                .SetPagePointer(BTreePagePointer<T>.NullPointer)
                .SetParentPagePointer(rootPage.PagePointer)
                .SetPageType(newChildrenPageType);
            var rightPageBuilder = new BTreePageBuilder<T>((int)rootPage.PageLength)
                .SetPagePointer(BTreePagePointer<T>.NullPointer)
                .SetParentPagePointer(rootPage.PagePointer)
                .SetPageType(newChildrenPageType);

            distributeKeysAndPointers(rootPage, leftPageBuilder, rightPageBuilder, keysInSplittedPages);

            var leftPagePointer = BTreeIO.WritePage(leftPageBuilder.Build());
            var rightPagePointer = BTreeIO.WritePage(rightPageBuilder.Build());
            
            updateParentPagePointersAfterSplit(leftPagePointer);
            updateParentPagePointersAfterSplit(rightPagePointer);
            
            var newRootPage = new BTreePageBuilder<T>((int) rootPage.PageLength)
                .CreateEmptyCloneFromPage(rootPage)
                .AddKey(rootPage.KeyAt(keysInSplittedPages))
                .AddPointer(leftPagePointer)
                .AddPointer(rightPagePointer)
                .Build();
            
            BTreeIO.WriteNewRootPage(newRootPage);
            BTreeIO.IncreaseTreeHeight();
            
            return newRootPage;
        }

        private void checkPage(IPage<T> page)
        {
            if(page.KeysInPage < page.PageLength)
                Logger.Log("BTreeSplitter warning: The provided page isn't full: " + page);
            if(page.KeysInPage % 2 == 0)
                Logger.Log("BTreeSplitter waring: The provided page has an even number of keys: " + 
                           page);
        }
        
        private static void distributeKeysAndPointers(IPage<T> page, BTreePageBuilder<T> leftPageBuilder, BTreePageBuilder<T> rightPageBuilder,
            long keysInSplittedPages)
        {
            leftPageBuilder.AddPointer(page.PointerAt(0));
            rightPageBuilder.AddPointer(page.PointerAt(keysInSplittedPages + 1));
            for (var i = 0; i < keysInSplittedPages; i++)
            {
                leftPageBuilder.AddKey(page.KeyAt(i));
                leftPageBuilder.AddPointer(page.PointerAt(i + 1));
                rightPageBuilder.AddKey(page.KeyAt(i + keysInSplittedPages + 1));
                rightPageBuilder.AddPointer(page.PointerAt(i + keysInSplittedPages + 2));
            }
        }
        
        

        private void updateParentPagePointersAfterSplit(IPagePointer<T> newPagePointer)
        {
            if (newPagePointer.Equals(BTreePagePointer<T>.NullPointer)) 
                throw new NullReferenceException("BTreePageSplitter error: Tried to access page with null pointer:" +
                                                 newPagePointer);
            var newPage = BTreeIO.GetPage(newPagePointer);

            if (newPage.PageType == PageType.LEAF) return;
            for (var i = 0; i <= newPage.KeysInPage; i++)
                BTreeIO.SetPageParentPointer(newPage.PointerAt(i), newPagePointer);
        }

        public IPage<T> Split(IPage<T> page, IKey<T> keyToInsert)
        {
            var newPage = BTreeAdding.InsertKeyIntoPage(page, keyToInsert);
            return Split(newPage);
        }
    }
}