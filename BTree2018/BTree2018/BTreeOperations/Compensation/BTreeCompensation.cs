using System;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeOperations
{
    public class BTreeCompensation<T> : BTreeCompensationPageModifier<T>, IBTreeCompensation<T> where T : IComparable
    {
        public IBTreeIO<T> BTreeIO;
        public IBTreeAdding<T> BTreeAdding;
        public IBTreePageNeighbours<T> BTreePageNeighbours;
        
        public bool Compensate(IPage<T> page, IKey<T> keyToAdd)
        {
            var overfilledPage = BTreeAdding.InsertKeyIntoPage(page, keyToAdd);
            return Compensate(overfilledPage);
        }

        public bool Compensate(IPage<T> page)
        {
            if (!BTreePageNeighbours.GetNeighbours(page, out var leftNeighbourPtr,
                out var rightNeighbourPtr, out var parentKey, out var parentKeyIndex)) return false;
            var parentPage = BTreePageNeighbours.ParentPage;
            if (checkIfPageCanBeCompensated(leftNeighbourPtr, out var leftNeighbourPage))
            {
                if (!EvenOutKeys(ref parentPage, parentKeyIndex, ref leftNeighbourPage, ref page))
                    return false;
                BTreeIO.WritePages(parentPage, leftNeighbourPage, page);
                return true;
            }
            else if (checkIfPageCanBeCompensated(rightNeighbourPtr, out var rightNeighbourPage))
            {
                if (!EvenOutKeys(ref parentPage, parentKeyIndex, ref page, ref rightNeighbourPage))
                    return false;
                BTreeIO.WritePages(parentPage, page, rightNeighbourPage);
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
            return pageExistsAndIsNotFull(page);
        }

        private static bool pageExistsAndIsNotFull(IPage<T> page)
        {
            return page.PageType != PageType.NULL && page.KeysInPage < page.PageLength;
        }
    }
}