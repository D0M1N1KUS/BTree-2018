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
        private IPagePointer<T> pagePointer;
        private List<IPagePointer<T>> pagePointers;
        private List<IKey<T>> keys;
        private int pageLength;
        private int keysInPage = -1;
        
        private IPagePointer<T> ParentPage = BTreePagePointer<T>.NullPointer;
        private PageType PageType = PageType.NULL;

        private bool pageCloned = false;

        public BTreePageBuilder(int expectedPageLength = -1)
        {
            pageLength = expectedPageLength + 1;
            if (expectedPageLength == -1)
            {
                pagePointers = new List<IPagePointer<T>>();
                keys = new List<IKey<T>>();
            }
            else if(expectedPageLength > 0)
            {
                pagePointers = new List<IPagePointer<T>>(expectedPageLength + 2);
                keys = new List<IKey<T>>(expectedPageLength + 1);
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
                throw new Exception("Not all necessary values have been initialized! (See error log)");
      
            page = new BTreePage<T>()
            {
                Keys = KeysToArray(),
                Pointers = PointersToArray(),
                ParentPage = ParentPage,
                PageType = PageType,
                KeysInPage = keysInPage,
                PageLength = pageLength == 0 ? keys.Count : pageLength - 1,
                OverFlown = keys.Count == pageLength,
                PagePointer = pagePointer 
            };

            return page;
        }

        public BTreePageBuilder<T> ClonePage(IPage<T> page)
        {
            CreateEmptyCloneFromPage(page);
            for (var i = 0; i < page.KeysInPage; i++)
            {
                keys.Add(page.KeyAt(i));
                pagePointers.Add(page.PointerAt(i));
            }
            pagePointers.Add(page.PointerAt(page.KeysInPage));
            
            return this;
        }

        public BTreePageBuilder<T> CreateEmptyCloneFromPage(IPage<T> page)
        {
            if (!pageCloned) pageCloned = true;
            else throw new Exception("BTreePageBuilder: Page has already been cloned!");
            ParentPage = page.ParentPage;
            PageType = page.PageType;
            pagePointer = page.PagePointer;
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

        public BTreePageBuilder<T> ModifyKeyAt(int i, IKey<T> key)
        {
            if (i >= 0 && i < keys.Count)
                keys[i] = key;
            else
                throw new ArgumentOutOfRangeException("BTreePageBuilder: The list of keys has [" + keys.Count +
                                                  "] elements, but you access [" + i + "]");
            return this;
        }

        public BTreePageBuilder<T> SetPagePointer(IPagePointer<T> pointer)
        {
            pagePointer = pointer;
            return this;
        }


        private IKey<T>[] KeysToArray()
        {
            if (pageLength <= 0)
            {
                var numberOfNulls = 0;
                foreach (var key in keys)
                {
                    if (key == null)
                        numberOfNulls++;
                }

                keysInPage = keys.Count - numberOfNulls;
                return keys.ToArray();
            }
            if (keys.Count > 0)
            {
                var array = new IKey<T>[pageLength];
                keysInPage = 0;
                for (var i = 0; i < pageLength; i++)
                {
                    if (i < keys.Count)
                    {
                        array[i] = keys[i];
                        keysInPage++;
                    }
                    else
                        array[i] = null;

                }

                return array;
            }
            throw new Exception("No keys provided!");
        }

        private IPagePointer<T>[] PointersToArray()
        {
            if (pageLength <= 0) return pagePointers.ToArray();
            var array = new IPagePointer<T>[pageLength + 1];
            for (var i = 0; i < pageLength + 1; i++)
            {
                array[i] = i < pagePointers.Count ? pagePointers[i] : BTreePagePointer<T>.NullPointer;
            }

            return array;
        }

        private bool checkIfAllNecessaryValuesInitialized()
        {
            bool allNecessaryValuesInitialized = true;
            if (keys.Count + 1 != pagePointers.Count)
            {
                Logger.Log("BTreePageBuilder warning: Inconsistent number of keys or pointers detected! Keys: " + 
                           keys.Count + " Pointers: " + pagePointers.Count);
            }

            if (pagePointer == null)
            {
                Logger.Log("BTreePageBuilder warning: Pointer to self has not been set!");
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