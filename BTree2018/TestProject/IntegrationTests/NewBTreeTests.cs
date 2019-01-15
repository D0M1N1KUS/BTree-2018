using System;
using BTree2018;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Logging;
using NUnit.Framework;

namespace TestProject.IntegrationTests
{
    [TestFixture]
    public class NewBTreeTests
    {
        private const string pageFilePath = "D:\\Pages";
        private const string pageMapFilePath = "D:\\PageMap";
        private const string recordFilePath = "D:\\Records";
        private const string recordMapFilePath = "D:\\RecordMap";

        [Test]
        public void addKeysToRootAndRootSplit()
        {
            try
            {
                var bTree = BTreeBuilder<int>.New(sizeof(int), 2, pageFilePath, recordFilePath, pageMapFilePath,
                    recordMapFilePath);
                var expectedRecord = new Record<int>(new int[] {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    RecordPointer<int>.NullPointer);
                bTree.Add(new Record<int>(new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    RecordPointer<int>.NullPointer));
                bTree.Add(new Record<int>(new int[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    RecordPointer<int>.NullPointer));
                bTree.Add(new Record<int>(new int[] {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    RecordPointer<int>.NullPointer));
                bTree.Add(expectedRecord);
                bTree.Add(new Record<int>(new int[] {5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    RecordPointer<int>.NullPointer)); //split
            
                var actualRecord = bTree.Get(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4});
            
                Assert.AreEqual(expectedRecord, actualRecord);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Assert.Fail(Logger.GetLog());
            }
        }

        private IRecord<int> getNewRecord(int value)
        {
            return new Record<int>(new int[] {value, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                RecordPointer<int>.NullPointer);
        }
    }
}