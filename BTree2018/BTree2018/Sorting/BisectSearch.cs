using System;
using BTree2018.Interfaces.CustomCollection;

namespace BTree2018.Bisection
{
    public class BisectSearch<T> : IBisection<T> where T : IComparable
    {
        public long LastIndex { get; private set; }
        
        public long GetClosestIndexTo(ICustomCollection<T> collection, T searchedValue)
        {
            if (collection.Length <= 0) return -1;
            var lowerEnd = (long)0;
            var higherEnd = collection.Length;
            while (true)
            {
                var midpoint = (lowerEnd + higherEnd) / 2;
                var selectedValue = collection[midpoint];
                if (selectedValue.CompareTo(searchedValue) == 0 || (higherEnd - lowerEnd) / 2 == 0)
                {
                    LastIndex = midpoint;
                    return midpoint;
                }
                if (selectedValue.CompareTo(searchedValue) == 1) higherEnd = midpoint;
                else lowerEnd = midpoint;
            }
        }
    }
}