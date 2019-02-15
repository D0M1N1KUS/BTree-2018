using System;

namespace BTree2018.BTreeIOComponents
{
    public static class GenericArithmetic<T> where T : IComparable
    {
        public static T Sum(T value1, T value2)
        {
            var type = typeof(T);

            if (type == typeof(int))
                return (T)(object)(int)((int)(object)value1 + (int)(object)value2);
            if (type == typeof(double))
                return (T)(object)(double)((double)(object)value1 + (double)(object)value2);
            if (type == typeof(float))
                return (T)(object)(float)((float)(object)value1 + (float)(object)value2);
            if (type == typeof(short))
                return (T)(object)(short)((short)(object)value1 + (short)(object)value2);
            if (type == typeof(long))
                return (T)(object)(long)((long)(object)value1 + (long)(object)value2);

            throw TypeNotRecognizedException(type);
        }

        public static T ConvertToGeneric(object value)
        {
            var type = typeof(T);

            if (type == typeof(int))
                return (T)(object)Convert.ToInt32(value);
            if (type == typeof(double))
                return (T)(object)Convert.ToDouble(value);
            if (type == typeof(float))
                return (T)(object)Convert.ToSingle(value);
            if (type == typeof(short))
                return (T)(object)Convert.ToInt16(value);
            if (type == typeof(long))
                return (T)(object)Convert.ToInt64(value);

            throw TypeNotRecognizedException(type);
        }
        
        private static Exception TypeNotRecognizedException(Type type)
        {
            return new Exception("The type of \"" + type + "\" is not supported!");
        }
    }
}