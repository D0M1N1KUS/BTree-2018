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
            var pageNullPointer = Substitute.For<IPageNullPointer<double>>();
            var recordPointer = Substitute.For<IRecordPointer<double>>();
            recordPointer.GetRecord().Returns(record);
            var key = new BTreeKeyBuilder<double>()
            {
                N = 1, Value = record, RecordPointer = recordPointer, LeftPage = pageNullPointer, 
                RightPage = pageNullPointer
            }.Build();
            
            Assert.AreEqual(1, key.N);
            Assert.AreEqual(record, key.Value);
            Assert.AreEqual(recordPointer, key.Record);
            Assert.AreEqual(pageNullPointer, key.LeftPagePointer);
            Assert.AreEqual(pageNullPointer, key.RightPagePointer);
        }

        [Test]
        public void makeKeyWithDoubleRecords_BuilderDoesNotReceiveEnoughValues_ShouldThrowException()
        {
            var keyBuilder = new BTreeKeyBuilder<IRecord<double>>();

            Assert.Throws<Exception>(() => keyBuilder.Build());
        }

        [Test]
        public void compareKeys_KeyValuesDiffer()
        {
            var key1 = new BTreeKey<int>()
            {
                N = 1, Value = new Record<int>() {Value = 0, ValueComponents = new int[] {0}},
            };
            var key2 = new BTreeKey<int>()
            {
                N = 2, Value = new Record<int>() {Value = 1, ValueComponents = new int[] {1}}
            };
            
            Assert.AreEqual((int)Comparison.LESS, key1.CompareTo(key2));
        }
    }
}