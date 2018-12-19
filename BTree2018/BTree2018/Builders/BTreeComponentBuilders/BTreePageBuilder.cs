using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;

namespace BTree2018.Builders
{
    public class BTreePageBuilder<T> where T : IComparable
    {
        private IPage<T> page;
        private List<IPagePointer<T>> pagePointers;
        private List<IKey<T>> keys;
        private int pageLength;
        
        private IPagePointer<T> ParentPage;
        private PageType PageType = PageType.NULL;

        public BTreePageBuilder(int expectedPageLength = -1)
        {
            pageLength = expectedPageLength;
            if (expectedPageLength == -1)
            {
                pagePointers = new List<IPagePointer<T>>();
                keys = new List<IKey<T>>();
            }
            else if(expectedPageLength > 0)
            {
                pagePointers = new List<IPagePointer<T>>(expectedPageLength + 1);
                keys = new List<IKey<T>>(expectedPageLength);
            }
            else
            {
                var e = new Exception("Invalid number of expected page length!");
                e.Data.Add("ExpectedPageLength", expectedPageLength);
                throw e;
            }
        }

        public static IPage<T> BuildNullPage()
        {
            return new BTreePage<T>();
        }

        public IPage<T> Build()
        {
            if (!checkIfAllNecessaryValuesInitialized())
                throw new Exception("No all necessary values have been initialized! (See error log)");
      
            page = new BTreePage<T>()
            {
                Keys = KeysToArray(),
                Pointers = PointersToArray(),
                ParentPage = ParentPage,
                PageType = PageType,
                KeysInPage = keys.Count
            };

            return page;
        }

        public BTreePageBuilder<T> ClonePage(IPage<T> page)
        {
            for (var i = 0; i < page.KeysInPage; i++)
            {
                keys.Add(page.KeyAt(i));
                pagePointers.Add(page.PointerAt(i));
            }
            pagePointers.Add(page.PointerAt(page.KeysInPage));
            CreateEmptyClone(page);
            
            return this;
        }

        public BTreePageBuilder<T> CreateEmptyClone(IPage<T> page)
        {
            ParentPage = page.ParentPage;
            PageType = page.PageType;
            return this;
        }

        public BTreePageBuilder<T> AddKey(IKey<T> key)
        {
            keys.Add(key);
            return this;
        }

        public BTreePageBuilder<T> AddPointer(IPagePointer<T> pagePointer)
        {
            pagePointers.Add(pagePointer);
            return this;
        }

        public BTreePageBuilder<T> AddKeyRange(IList<IKey<T>> range)
        {
            keys.AddRange(range);
            return this;
        }

        public BTreePageBuilder<T> AddPointerRange(IList<IPagePointer<T>> range)
        {
            pagePointers.AddRange(range);
            return this;
        }

        public BTreePageBuilder<T> SetParentPagePointer(IPagePointer<T> pointer)
        {
            ParentPage = pointer;
            return this;
        }

        public BTreePageBuilder<T> SetPageType(PageType type)
        {
            PageType = type;
            return this;
        }


        private IKey<T>[] KeysToArray()
        {
            if (pageLength <= 0) return keys.ToArray();
            if (keys.Count > 0)
            {
                var array = new IKey<T>[pageLength];
                for (var i = 0; i < pageLength; i++)
                {
                    array[i] = i < keys.Count ? keys[i] : null;
                }

                return array;
            }
            throw new Exception("No keys provided!");
        }

        private IPagePointer<T>[] PointersToArray()
        {
            if (pageLength <= 0) return pagePointers.ToArray();
            var array = new IPagePointer<T>[pageLength + 1];
            if (pagePointers.Count > 0)
            {
                for (var i = 0; i < pageLength + 1; i++)
                {
                    array[i] = i < pagePointers.Count ? pagePointers[i] : null;
                }

                return array;
            }
            
            throw new Exception("BTreePageBuilder: No pointers provided!");
        }

        private bool checkIfAllNecessaryValuesInitialized()
        {
            bool allNecessaryValuesInitialized = true;
            if (keys.Count != pagePointers.Count + 1)
            {
                Logger.Log("BTreePageBuilder warning: Inconsistent number of keys or pages detected! Keys: " + keys.Count +
                           " Pointers: " + pagePointers.Count);
            }

            if (keys.Count == 0)
            {
                Logger.Log("BTreePageBuilder warning: Building null page!");
            }
            
            if (keys.Count > 0 && PageType == PageType.NULL)
            {
                Logger.Log("BTreePageBuilder error: Inconsistent page type! Number of inserted keys is greater than zero!");
                allNecessaryValuesInitialized = false;
            }

            if (PageType != PageType.ROOT && ParentPage == null)
            {
                Logger.Log("BTreePageBuilder error: Page has no pointer to parent page!");
                allNecessaryValuesInitialized = false;
            }

            return allNecessaryValuesInitialized;
        }
        
    }
}