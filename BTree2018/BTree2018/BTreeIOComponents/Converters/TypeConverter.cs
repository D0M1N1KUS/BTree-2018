using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Controls;

namespace BTree2018.BTreeIOComponents
{
    public static class TypeConverter<T> where T : IComparable
    {
        private static readonly long TYPE_STRING_LENGTH = 64;

        public static byte[] ToBytes(T value)
        {
            var type = typeof(T);

            if (type == typeof(int))
                return BitConverter.GetBytes((int) (object) value);
            if (type == typeof(double))
                return BitConverter.GetBytes((double) (object) value);
            if (type == typeof(float))
                return BitConverter.GetBytes((float) (object) value);
            if (type == typeof(short))
                return BitConverter.GetBytes((short) (object) value);
            if (type == typeof(long))
                return BitConverter.GetBytes((long) (object) value);

            throw TypeNotRecognizedException(type);
        }

        public static T ToValue(byte[] bytes)
        {
            var type = typeof(T);

            if (type == typeof(int))
                return (T) (object) BitConverter.ToInt32(bytes, 0);
            if (type == typeof(double))
                return (T) (object) BitConverter.ToDouble(bytes, 0);
            if (type == typeof(float))
                return (T) (object) BitConverter.ToSingle(bytes, 0);
            if (type == typeof(short))
                return (T) (object) BitConverter.ToInt16(bytes, 0);
            if (type == typeof(long))
                return (T) (object) BitConverter.ToInt64(bytes, 0);

            throw TypeNotRecognizedException(type);
        }

        private static Exception TypeNotRecognizedException(Type type)
        {
            return new Exception("The type of \"" + type + "\" is not supported!");
        }

        public static byte[] TypeTo64ByteString()
        {
            var bytes = new byte[64];
            var typeString = Encoding.ASCII.GetBytes(typeof(T).ToString());
            var restOfTypeString = typeString.Length < TYPE_STRING_LENGTH
                ? Enumerable.Repeat((byte) 0, (int) (TYPE_STRING_LENGTH - typeString.Length)).ToArray()
                : new byte[0];
            typeString.CopyTo(bytes, 0);
            restOfTypeString.CopyTo(bytes, typeString.Length);
            return bytes;
        }

        public static Type TypeStringToType(string typeSting)
        {
            if (typeSting.Length != TYPE_STRING_LENGTH)
                throw new Exception("Type string \"" + typeSting + "\" is not " + TYPE_STRING_LENGTH +
                                    "  characters long.");
            return Type.GetType(typeSting.Replace("\0", string.Empty));
        }

        public static Type TypeStringToType(byte[] bytes)
        {
            return TypeStringToType(System.Text.Encoding.Default.GetString(bytes));
        }
    }
}