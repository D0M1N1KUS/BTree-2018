using System;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreePageNeighbours<T> : IBTreePageNeighbours<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        
        public bool GetNeighbours(IPage<T> page, out IPagePointer<T> leftNeighbourPtr, 
            out IPagePointer<T> rightNeighbourPtr, out IKey<T> parentKey)
        {
            leftNeighbourPtr = null;
            rightNeighbourPtr = null;
            parentKey = null;

            if (page.PageType == PageType.NULL || page.PageType == PageType.ROOT)
                return false;
            
            var parentPage = BTreeIO.GetPage(page.ParentPage);

            getNeighbours(page, ref leftNeighbourPtr, ref rightNeighbourPtr, ref parentKey, parentPage);

            return leftNeighbourPtr != null || rightNeighbourPtr != null;
        }

        private static void getNeighbours(IPage<T> page, ref IPagePointer<T> leftNeighbourPtr, ref IPagePointer<T> rightNeighbourPtr,
            ref IKey<T> parentKey, IPage<T> parentPage)
        {
            for (var i = 0; i < parentPage.KeysInPage + 1; i++)
            {
                var currentPointer = parentPage.PointerAt(i);
                if (!page.PagePointer.Equals(currentPointer)) continue;
                leftNeighbourPtr = i > 0 ? parentPage.PointerAt(i - 1) : null;
                rightNeighbourPtr = i < parentPage.KeysInPage ? parentPage.PointerAt(i + 1) : null;
                parentKey = parentPage.KeyAt(i);
                break;
            }
        }
    }
}