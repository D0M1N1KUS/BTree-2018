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
        
        public void Split(IPage<T> page)
        {
            checkPage(page);
            var keysInSplittedPages = page.KeysInPage / 2;
            var newPagesType = page.PageType == PageType.ROOT ? PageType.LEAF : page.PageType;

            var leftPageBuilder = new BTreePageBuilder<T>((int)page.PageLength)
                .SetPagePointer(page.PagePointer)
                .SetParentPagePointer(page.ParentPage)
                .SetPageType(newPagesType);
            var rightPageBuilder = new BTreePageBuilder<T>((int)page.PageLength)
                .SetPagePointer(BTreePagePointer<T>.NullPointer)
                .SetParentPagePointer(page.ParentPage)
                .SetPageType(newPagesType);

            leftPageBuilder.AddPointer(page.PointerAt(0));
            rightPageBuilder.AddPointer(page.PointerAt(keysInSplittedPages + 1));
            for (var i = 0; i < keysInSplittedPages; i++) // TODO: check if this stuff lines up
            {
                leftPageBuilder.AddKey(page.KeyAt(i));
                leftPageBuilder.AddPointer(page.PointerAt(i + 1));
                rightPageBuilder.AddKey(page.KeyAt(i + keysInSplittedPages + 1));
                rightPageBuilder.AddPointer(page.PointerAt(i + keysInSplittedPages + 2));
            }

            BTreeIO.WritePage(leftPageBuilder.Build());
            var rightPagePointer = BTreeIO.WritePage(rightPageBuilder.Build());
            if (page.PageType != PageType.ROOT)
                BTreeAdding.InsertKeyIntoPage(BTreeIO.GetPage(page.ParentPage), page.KeyAt(keysInSplittedPages),
                    rightPagePointer);
            else
            {
                var newRootPage = new BTreePageBuilder<T>((int) page.PageLength)
                    .SetPageType(PageType.ROOT)
                    .SetParentPagePointer(BTreePagePointer<T>.NullPointer)
                    .AddKey(page.KeyAt(keysInSplittedPages))
                    .AddPointer(page.LeftPointerAt(keysInSplittedPages))
                    .AddPointer(rightPagePointer)
                    .Build();
                BTreeIO.WriteNewRootPage(newRootPage);
            }
        }

        private void checkPage(IPage<T> page)
        {
            if(page.KeysInPage < page.PageLength)
                Logger.Log("BTreeSplitter warning: The provided page isn't full: " + page);
            if(page.KeysInPage % 2 == 1)
                Logger.Log("BTreeSplitter waring: The provided page has an even number of keys: " + 
                           page);
        }

        public void Split(IPage<T> page, IKey<T> keyToInsert)
        {
            throw new NotImplementedException();
        }
    }
}