using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using BTree2018.Bisection;
using BTree2018.BTreeIOComponents;
using BTree2018.BTreeIOComponents.Basics;
using BTree2018.BTreeIOComponents.BTreeFileClasses;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeOperations;
using BTree2018.BTreeOperations.BTreeSplitting;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeOperations;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Interfaces.FileIO;

namespace BTree2018
{
    public static class BTreeBuilder<T> where T : IComparable
    {
        private static BTreeIO<T> BTreeIO;
        private static BTreeAdder<T> Adder;
        private static BTreeKeyRemover<T> Remover;
        private static BTreeSearcher<T> Searcher;

        private static BTreeReorganizer<T> Reorganizer;

        private static BTreeCompensation<T> Compensator;

        private static BTreePageSplitter<T> Splitter;

        private static BTreeLeafKeyRemover<T> LeafRemover;
        private static BTreeMerger<T> Merger;

        public static IBTree<T> New(int sizeOfType, long d, string pageFilePath, string recordFilePath,
            string pageFileMapPath, string recordFileMapPath)
        {
            BTreeIO = new BTreeIO<T>()
            {
                BTreePageFile = createNewPageFile(sizeOfType, d, pageFilePath, pageFileMapPath),
                RecordFile = createNewRecordFile(sizeOfType, recordFilePath, recordFileMapPath)
            };
            return Build();
        }

        private static IRecordFile<T> createNewRecordFile(int sizeOfType, string recordFilePath, string recordFileMapPath)
        {
            var recordFile = new RecordFile<T>(sizeOfType)
            {
                FileIO = new FileIO(recordFilePath),
                FileMap = new FileMap(recordFileMapPath)
            };
            recordFile.WriteInitialValuesToFile();
            return recordFile;
        }

        private static IBTreePageFile<T> createNewPageFile(int sizeOfType, long d, string pageFilePath, 
            string pageFileMapPath)
        {
            var pageFile = new BTreePageFile<T>(sizeOfType, d, pageFilePath, pageFileMapPath);
            return pageFile;
        }
        
        public static IBTree<T> Open(int sizeOfType, string pageFilePath, string recordFilePath,
            string pageFileMapPath, string recordFileMapPath)
        {
            var pageFileIO = new FileIO(new FileInput(pageFilePath), new FileOutput(pageFilePath),
                new FileInfo(pageFilePath));
            var recordFileIO = new FileIO(new FileInput(recordFilePath), new FileOutput(recordFilePath), 
                new FileInfo(recordFilePath));
            
            var pageFileMap = new FileMap(new FileIO(pageFileMapPath));
            var recordFileMap = new FileMap(new FileIO(recordFileMapPath));

            BTreeIO = new BTreeIO<T>()
            {
                BTreePageFile = new BTreePageFile<T>(pageFileIO, pageFileMap, sizeOfType),
                RecordFile = new RecordFile<T>(recordFileIO, recordFileMap, sizeOfType)
            };
            return Build();
        }
        
        private static IBTree<T> Build()
        {
            initializeSearcher();

            Adder = new BTreeAdder<T>()
            {
                BTreeIO = BTreeIO,
                BTreeSearching = Searcher
                //compensation, splitter added later
            };

            Remover = new BTreeKeyRemover<T>()
            {
                BTreeIO = BTreeIO,
                BTreeSearching = Searcher
                //reoganizer and leafKetRemover added later
            };
            
            initializeCompensator();
            initializeMerger();
            initializeSplitter();

            Adder.BTreeSplitting = Splitter;
            Adder.BTreeCompensation = Compensator;

            initializeLeafRemover();
            initializeReorganizer();

            Remover.LeafKeyRemoval = LeafRemover;
            Remover.BTreeReorganizer = Reorganizer;

            return new BTree<T>()
            {
                Adder = Adder, Remover = Remover, Searcher = Searcher, BTreeIO = BTreeIO
            };
        }

        private static void initializeReorganizer()
        {
            Reorganizer = new BTreeReorganizer<T>()
            {
                BTreeCompensation = Compensator,
                BTreeMerger = Merger
            };
        }

        private static void initializeLeafRemover()
        {
            LeafRemover = new BTreeLeafKeyRemover<T>()
            {
                BTreeIO = BTreeIO
            };
        }

        private static void initializeSplitter()
        {
            Splitter = new BTreePageSplitter<T>()
            {
                BTreeAdding = Adder,
                BTreeIO = BTreeIO
            };
        }

        private static void initializeMerger()
        {
            Merger = new BTreeMerger<T>()
            {
                BTreeIO = BTreeIO,
                BTreePageNeighbours = new BTreePageNeighbours<T>()
                {
                    BTreeIO = BTreeIO
                }
            };
        }

        private static void initializeCompensator()
        {
            Compensator = new BTreeCompensation<T>()
            {
                BTreePageNeighbours = new BTreePageNeighbours<T>()
                {
                    BTreeIO = BTreeIO
                },
                BTreeIO = BTreeIO,
                BTreeAdding = Adder
            };
        }

        private static void initializeSearcher()
        {
            Searcher = new BTreeSearcher<T>()
            {
                BisectSearch = new BisectSearch<T>(),
                BTreeIO = BTreeIO
            };
        }
    }
}