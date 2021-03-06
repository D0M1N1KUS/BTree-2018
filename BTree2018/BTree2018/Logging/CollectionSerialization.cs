using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTree2018.Interfaces.CustomCollection;

namespace BTree2018.Logging
{
    public static class CollectionSerialization
    {
        public static string Stringify<T>(T[] collection, string beginMarker = "[", string endMarker = "]", 
            string separator = ", ") where T : IComparable
        {
            var valueComponentsStringBuilder = new StringBuilder();
            valueComponentsStringBuilder.Append(beginMarker);
            for (var i = 0; i < collection.Length; i++)
            {
                valueComponentsStringBuilder.Append(collection[i]);
                if (i != collection.Length - 1) valueComponentsStringBuilder.Append(", ");
            }

            valueComponentsStringBuilder.Append("]");

            return valueComponentsStringBuilder.ToString();
        }

        public static string Stringify<T>(ICustomCollection<T> collection, string beginMarker = "[", string endMarker = "]",
            string separator = ", ") where T : IComparable
        {
            var valueComponentsStringBuilder = new StringBuilder();
            valueComponentsStringBuilder.Append(beginMarker);
            //var lastValue = collection[collection.Length - 1];
            for (var i = 0; i < collection.Length; i++)
            {
                valueComponentsStringBuilder.Append(collection[i]);
                if (i != collection.Length - 1) valueComponentsStringBuilder.Append(", ");
            }

            valueComponentsStringBuilder.Append("]");

            return valueComponentsStringBuilder.ToString();
        }
    }
}