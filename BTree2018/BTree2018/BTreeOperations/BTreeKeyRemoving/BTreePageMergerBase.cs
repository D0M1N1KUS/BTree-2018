using System;
using BTree2018.Builders;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.BTreeOperations
{
    public class BTreePageMergerBase<T> where T : IComparable
    {
        public static IPage<T> MergePagesAndKey(IPage<T> leftPage, IKey<T> parentKey, IPage<T> rightPage, 
            PageType expectedPageType = PageType.NULL)
        {
            var mergedPageBuilder = new BTreePageBuilder<T>((int) leftPage.PageLength)
                .SetPageType(expectedPageType != PageType.NULL ? expectedPageType : leftPage.PageType)
                .SetParentPagePointer(leftPage.ParentPage)
                .SetPagePointer(leftPage.PagePointer)

                .AddPointer(leftPage.PointerAt(0));

            for (var i = 0; i < leftPage.KeysInPage; i++)
            {
                mergedPageBuilder.AddKey(leftPage.KeyAt(i));
                mergedPageBuilder.AddPointer(leftPage.PointerAt(i + 1));
            }

            mergedPageBuilder.AddKey(parentKey);

            mergedPageBuilder.AddPointer(rightPage.PointerAt(0));
            for (var i = 0; i < rightPage.KeysInPage; i++)
            {
                mergedPageBuilder.AddKey(rightPage.KeyAt(i));
                mergedPageBuilder.AddPointer(rightPage.PointerAt(i + 1));
            }

            return mergedPageBuilder.Build();
        }

        public static IPage<T> RemoveParentPageKeyAndInsetNewPointer(IPage<T> parentPage, long keyIndex,
            IPagePointer<T> newPagePointer)
        {
            var newParentPageBuilder = new BTreePageBuilder<T>((int)parentPage.PageLength)
                .CreateEmptyCloneFromPage(parentPage);
            
            for (var i = 0; i < keyIndex; i++)
            {
                newParentPageBuilder.AddKey(parentPage.KeyAt(i));
                newParentPageBuilder.AddPointer(parentPage.PointerAt(i));
            }

            newParentPageBuilder.AddPointer(newPagePointer);

            for (var i = keyIndex + 1; i < parentPage.KeysInPage; i++)
            {
                newParentPageBuilder.AddKey(parentPage.KeyAt(i));
                newParentPageBuilder.AddPointer(parentPage.PointerAt(i + 1));
            }

            return newParentPageBuilder.Build();
        }
    }
}