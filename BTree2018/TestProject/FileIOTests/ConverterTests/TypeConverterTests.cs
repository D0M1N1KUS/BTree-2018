using System;
using System.Linq;
using System.Text;
using BTree2018.BTreeIOComponents;
using NUnit.Framework;

namespace UnitTests.FileIOTests
{
    [TestFixture]
    public class TypeConverterTests
    {
        [Test]
        public void getTypeString_testingAllNumericTypes()
        {
            var expectedInt16String = to64ByteArray(typeof(short).ToString());
            var expectedInt32String = to64ByteArray(typeof(int).ToString());
            var expectedInt64String = to64ByteArray(typeof(long).ToString());
            var expectedFloatString = to64ByteArray(typeof(float).ToString());
            var expectedDoubleString = to64ByteArray(typeof(double).ToString());

            var actualInt16String = TypeConverter<short>.TypeTo64ByteString();
            var actualInt32String = TypeConverter<int>.TypeTo64ByteString();
            var actualInt64String = TypeConverter<long>.TypeTo64ByteString();
            var actualFloatString = TypeConverter<float>.TypeTo64ByteString();
            var actualDoubleString = TypeConverter<double>.TypeTo64ByteString();
            
            CollectionAssert.AreEqual(expectedInt16String, actualInt16String);
            CollectionAssert.AreEqual(expectedInt32String, actualInt32String);
            CollectionAssert.AreEqual(expectedInt64String, actualInt64String);
            CollectionAssert.AreEqual(expectedFloatString, actualFloatString);
            CollectionAssert.AreEqual(expectedDoubleString, actualDoubleString);
        }

        [Test]
        public void typeStringToTypeTest_testingAllNumericTypes()
        {
            Assert.AreEqual(typeof(short), 
                TypeConverter<int>.TypeStringToType(to64CharacterString(typeof(short).ToString())));
            Assert.AreEqual(typeof(int),
                TypeConverter<int>.TypeStringToType(to64CharacterString(typeof(int).ToString())));
            Assert.AreEqual(typeof(long), 
                TypeConverter<int>.TypeStringToType(to64CharacterString(typeof(long).ToString())));
            Assert.AreEqual(typeof(float), 
                TypeConverter<int>.TypeStringToType(to64CharacterString(typeof(float).ToString())));
            Assert.AreEqual(typeof(double), 
                TypeConverter<int>.TypeStringToType(to64CharacterString(typeof(double).ToString())));
        }

        private byte[] to64ByteArray(string typeString)
        {
            return Encoding.ASCII.GetBytes(to64CharacterString(typeString));
        }

        private string to64CharacterString(string typeString)
        {
            var stringBuilder = new StringBuilder(64);
            stringBuilder.Append(typeString);
            while(stringBuilder.Length < 64)
            {
                stringBuilder.Append((char) 0);
            }

            return stringBuilder.ToString();
        }

        [Test]
        public void typeToStringTest()
        {
            var expectedByteArray = Enumerable.Repeat((byte) 0, 64).ToArray();
            Encoding.ASCII.GetBytes(typeof(int).ToString()).CopyTo(expectedByteArray, 0);

            var actualByteArray = TypeConverter<int>.TypeTo64ByteString();
            
            CollectionAssert.AreEqual(expectedByteArray, actualByteArray);
        }
    }
}