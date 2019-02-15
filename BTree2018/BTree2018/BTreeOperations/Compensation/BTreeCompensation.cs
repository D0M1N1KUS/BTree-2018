using System;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeCompensation<T> : BTreeCompensationPageModifier<T>, IBTreeCompensation<T> where T : IComparable
    {
        public IPage<T> Page { get; private set; }
        
        public bool Compensate(IPage<T> page, IKey<T> keyToAdd)
        {
            var overfilledPage = BTreeAdding.InsertKeyIntoPage(page, keyToAdd);
            return Compensate(overfilledPage);
        }
        //TODO: A BTree with D = 1 is a special occasion, where an underfilled page is an empty page.
        //TODO: There is a parentPagePointer bug while splitting. Every parent page points to root
        public bool Compensate(IPage<T> page)
        {
            if (!BTreePageNeighbours.GetNeighbours(page, out var leftNeighbourPtr,
                out var rightNeighbourPtr, out var parentKey, out var parentKeyIndex)) return false;
            var parentPage = BTreePageNeighbours.ParentPage;
            if (checkIfPageCanBeCompensated(leftNeighbourPtr, out var leftNeighbourPage))
            {
                if (!rightNeighbourPtr.Equals(BTreePagePointer<T>.NullPointer))
                    parentKey = BTreePageNeighbours.ParentPage.KeyAt(--parentKeyIndex);
                if (!EvenOutKeys(ref parentPage, parentKeyIndex, ref leftNeighbourPage, ref page))
                    return false;
                var pointers = BTreeIO.WritePages(parentPage, leftNeighbourPage, page);
                updateParentPagePointersAfterCompensation(pointers[1]);
                updateParentPagePointersAfterCompensation(pointers[2]);
                Page = page;
                return true;
            }
            else if (checkIfPageCanBeCompensated(rightNeighbourPtr, out var rightNeighbourPage))
            {
                if (!EvenOutKeys(ref parentPage, parentKeyIndex, ref page, ref rightNeighbourPage))
                    return false;
                var pointers = BTreeIO.WritePages(parentPage, page, rightNeighbourPage);
                Page = page;
                updateParentPagePointersAfterCompensation(pointers[1]);
                updateParentPagePointersAfterCompensation(pointers[2]);
                return true;
            }
            else
                return false;
        }


        private bool checkIfPageCanBeCompensated(IPagePointer<T> pointer, out IPage<T> page)
        {
            if (pointer.Equals(BTreePagePointer<T>.NullPointer) || pointer.PointsToPageType == PageType.NULL)
            {
                page = null;
                return false;
            }

            page = BTreeIO.GetPage(pointer);
            return page.PageType != PageType.NULL; //pageExistsAndIsNotFull(page);
        }

        private static bool pageExistsAndIsNotFull(IPage<T> page)
        {
            return page.PageType != PageType.NULL && page.KeysInPage < page.PageLength;
        }
    }
}