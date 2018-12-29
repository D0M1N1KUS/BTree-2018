using System;
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
            var parentPage = BTreePageNeighbours.ParentPage;
            if (!BTreePageNeighbours.GetNeighbours(overfilledPage, out var leftNeighbourPtr, out var rightNeighbourPtr, 
                out var parentKey, out var parentKeyIndex))
                return false;
            if (checkIfPageCanBeCompensated(leftNeighbourPtr, out var leftNeighbourPage))
            {
                EvenOutKeys(ref parentPage, parentKeyIndex, ref leftNeighbourPage, ref overfilledPage);
                return true;
            }
            else if (checkIfPageCanBeCompensated(rightNeighbourPtr, out var rightNeighbourPage))
            {
                EvenOutKeys(ref parentPage, parentKeyIndex, ref overfilledPage, ref rightNeighbourPage);
                return true;
            }
            else
                return false;
        }

        private bool checkIfPageCanBeCompensated(IPagePointer<T> pointer, out IPage<T> page)
        {
            if (pointer == null || pointer.PointsToPageType == PageType.NULL)
            {
                page = null;
                return false;
            }

            page = BTreeIO.GetPage(pointer);
            if (pageExistsAndIsNotFull(page)) return true;
            return false;
        }

        private static bool pageExistsAndIsNotFull(IPage<T> page)
        {
            return page.PageType != PageType.NULL && page.KeysInPage < page.PageLength;
        }
    }
}