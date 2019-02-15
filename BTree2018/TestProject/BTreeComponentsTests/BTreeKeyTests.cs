using System;
using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Enums;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace UnitTests.BTreeComponentsTests
{
    [TestFixture]
    public class BTreeKeyTests
    {
        [Test]
        public void makeKeyWithDoubleValuesRecords()
        {
            var record = Substitute.For<IRecord<double>>();
            record.Value.Returns(0.123);
            record.ValueComponents.Returns(new double[] {0.0, 0.0, 0.123, 0.0123});
            var recordPointer = Substitute.For<IRecordPointer<double>>();
            var key = new BTreeKeyBuilder<double>()
            {
                Value = record.Value, RecordPointer = recordPointer
            }.Build();
            
            Assert.AreEqual(record.Value, key.Value);
            Assert.AreEqual(recordPointer, key.RecordPointer);
        }

        [Test]
        public void makeKeyWithDoubleRecords_BuilderDoesNotReceiveEnoughValues_ShouldThrowException()
        {
            var keyBuilder = new BTreeKeyBuilder<IRecord<double>>();

            Assert.Throws<Exception>(() => keyBuilder.Build());
        }

        [Test]
        public void compareKeys_KeysAreDifferent()
        {
            var key1 = new BTreeKey<int>() {Value = 0};
            var key2 = new BTreeKey<int>() {Value = 1};
            
            Assert.IsTrue(key1 < key2);
            Assert.IsTrue(key1 <= key2);
            Assert.IsTrue(key1 != key2);
            Assert.IsFalse(key1 > key2);
            Assert.IsFalse(key1 >= key2);
            Assert.IsFalse(key1 == key2);
        }

        [Test]
        public void compareKey_KeysAreEqual()
        {
            var key1 = new BTreeKey<int>() {Value = 0};
            var key2 = new BTreeKey<int>() {Value = 0};
            
            Assert.IsTrue(key1 == key2);
            Assert.IsTrue(key1 <= key2);
            Assert.IsTrue(key1 >= key2);
            Assert.IsFalse(key1 != key2);
            Assert.IsFalse(key1 > key2);
            Assert.IsFalse(key1 < key2);
        }

        [Test]
        public void compareKeys_SanityCheck()
        {
            var key1 = new BTreeKey<int>() {Value = 0};
            IKey<int> key2s = new BTreeKey<int>() {Value = 1};

            IKey<int> key1s = new BTreeKey<int>() {Value = 0};
            var key2 = new BTreeKey<int>() {Value = 1};
            
            var val1 = key1.CompareTo(key2s);
            var val2 = key1s.CompareTo(key2);
            
            Assert.IsTrue(val1 == val2);
            Assert.IsTrue(key1s.CompareTo(key2s) == (int)Comparison.LESS);
        }
    }
}