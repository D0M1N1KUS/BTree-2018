using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using BTree2018.BTreeIOComponents;
using BTree2018.BTreeIOComponents.Basics;
using BTree2018.BTreeIOComponents.Converters;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;

namespace BTreeFileReader
{
    public class PageFilePrinter<T> where T : IComparable
    {
        private const long TYPE_STRING_PREAMBLE_SIZE = 64;
        private const long SIZE_OF_LONG = sizeof(long);
        
        public void PrintRecordFile(FileIO fileIO, int sizeOfType, bool outputBytes, FileMap map, RichTextBox textBox)
        {
            var valueType = TypeConverter<T>.TypeStringToType(fileIO.GetBytes(0, TYPE_STRING_PREAMBLE_SIZE));
            var d = BitConverter.ToInt64(fileIO.GetBytes(TYPE_STRING_PREAMBLE_SIZE + SIZE_OF_LONG, SIZE_OF_LONG), 0);
            var structureInfo = new BTreePageStructureInfo<T>(d, sizeOfType);
            var pageConverter = new BTreePageConverter<T>(d, sizeOfType);

            if (fileIO.FileLength <= structureInfo.LengthOfPreamble)
            {
                Console.WriteLine("File is invalid!");
                return;
            }
            
            var h = BitConverter.ToInt64(fileIO.GetBytes(structureInfo.LocationOfTreeHeight, SIZE_OF_LONG), 0);
            var rootPointer = pageConverter.PagePointerConverter.ConvertToPointer(
                fileIO.GetBytes(structureInfo.LocationOfRootPagePointer, structureInfo.SizeOfPagePointer));

            Console.WriteLine("Value type\t[{0}]\nH\t\t[{1}]\n\nD\t\t[{2}]\nRoot pointer [{3}]\n", 
                valueType, h, d, rootPointer);
            Console.WriteLine(createHeader("FileMap"));
            new FileMapPrinter().printFileMap(map, 10);
            Console.WriteLine();
            Console.WriteLine(createHeader("Pages"));

            var pageBytes = new byte[0];
            for (var i = 0; i < (fileIO.FileLength - structureInfo.LengthOfPreamble) / structureInfo.PageSize; i++)
            {
                try
                {
                    pageBytes = fileIO.GetBytes(structureInfo.LengthOfPreamble + i * structureInfo.PageSize,
                        structureInfo.PageSize);
                }
                catch (EndOfStreamException eos)
                {
                    Console.WriteLine("End of file.");
                    return;
                }
                
                if(outputBytes)
                    Console.WriteLine("File at {0}\n[ {1} ]\n", i, BitConverter.ToString(pageBytes)
                        .Replace("-", " "));
                if (pageBytes.Length == structureInfo.PageSize)
                {
                    try
                    {
                        var page = pageConverter.ConvertToPage(pageBytes, new BTreePagePointer<T>()
                        {
                            Index = i, PointsToPageType = PageType.NULL
                        });

                        Console.ForegroundColor = map[i] ? ConsoleColor.White : ConsoleColor.Red;
                        Console.WriteLine("Page {0}\n{1}", i, page);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return;
                    }
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("End of file");
        }
        
        private string createHeader(string text)
        {
            if (text.Length >= Console.WindowWidth - 1) return text;
            if (text.Length % 2 == 1) text = string.Concat(text, "-");
            var separator = new string(Enumerable.Repeat('-', (Console.WindowWidth - text.Length) / 2).ToArray());
            return string.Concat(separator, text, separator);
        }

    }
}