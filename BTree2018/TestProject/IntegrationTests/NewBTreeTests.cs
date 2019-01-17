using System;
using System.IO;
using BTree2018;
using BTree2018.BTreeIOComponents.Basics;
using BTree2018.BTreeStructure;
using BTree2018.Exceptions;
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
        public void addKeysToRootAndRootSplit_CreateNewFiles()
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
            
                Console.WriteLine(Logger.GetLog());
                Assert.AreEqual(expectedRecord, actualRecord);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Assert.Fail(Logger.GetLog());
            }

            //cleanUp();
        }

        [Test]
        public void readBTreeFromExistingFile()
        {
            try
            {
                var bTree = BTreeBuilder<int>.Open(sizeof(int), pageFilePath, recordFilePath, pageMapFilePath,
                    recordMapFilePath);
                var expectedRecord = getNewRecord(4);

                var actualRecord =
                    bTree.Get(new BTreeKey<int>() {RecordPointer = RecordPointer<int>.NullPointer, Value = 4});
                
                Console.WriteLine(Logger.GetLog());
                Assert.AreEqual(expectedRecord, actualRecord);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Assert.Fail(Logger.GetLog());
            }
            //cleanUp();
        }

        [Test]
        public void compensationTest()//TODO: recordmap is not being saved properly
        {
            var bTree = BTreeBuilder<int>.Open(sizeof(int), pageFilePath, recordFilePath, pageMapFilePath,
                recordMapFilePath);

            Assert.Throws<DuplicateKeyException>(() => bTree.Add(getNewRecord(5)));
            
            bTree.Add(getNewRecord(6));
            bTree.Add(getNewRecord(7));
            bTree.Add(getNewRecord(8)); //Compensation
            bTree.Add(getNewRecord(9)); //Compensation
            bTree.Add(getNewRecord(10)); //Split

            for (var i = 0; i < 10; i++)
            {
                if(!bTree.HasKey(i+1))
                    Assert.Fail("Key [" + (i+1).ToString() + "] not found!");
            }
            
            Console.WriteLine(Logger.GetLog());
            Assert.AreEqual(2, bTree.H);
        }

        [Test]
        public void deletingRecordsTest()
        {
            try
            {
                var bTree = BTreeBuilder<int>.New(sizeof(int), 2, pageFilePath, recordFilePath, pageMapFilePath,
                    recordMapFilePath);
                bTree.Add(getNewRecord(1));
                bTree.Add(getNewRecord(2));
                bTree.Add(getNewRecord(3));
                bTree.Add(getNewRecord(4));
                bTree.Add(getNewRecord(5)); //split
                bTree.Add(getNewRecord(6));
                bTree.Add(getNewRecord(7));
                bTree.Add(getNewRecord(8)); //Compensation
                bTree.Add(getNewRecord(9)); //Compensation
                bTree.Add(getNewRecord(10)); //Split

                bTree.Remove(4); //Just delete

                bTree.Remove(8); //Gets key from leaf -> merge
                bTree.Add(getNewRecord(11));

                Console.WriteLine(Logger.GetLog());
                Assert.AreEqual(2, bTree.H);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Assert.Fail(Logger.GetLog());
            }
        }

        [Test]
        public void fillUpTreeAndEmptyItAgain()
        {
            try
            {
                var bTree = BTreeBuilder<int>.New(sizeof(int), 1, pageFilePath, recordFilePath, pageMapFilePath,
                    recordMapFilePath);

                for (var i = 0; i < 32; i++)
                {
                    bTree.Add(getNewRecord(i+1));
                }

                for (var i = 0; i < 32; i++)
                {
                    bTree.Remove(i+1);
                }
                
                Console.WriteLine(Logger.GetLog());
                Assert.IsTrue(true);
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
        
        private static void cleanUp()
        {
            File.Delete(pageFilePath);
            File.Delete(pageMapFilePath);
            File.Delete(recordFilePath);
            File.Delete(recordMapFilePath);
        }
    }
}