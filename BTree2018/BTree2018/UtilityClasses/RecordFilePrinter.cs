using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using BTree2018.BTreeIOComponents;
using BTree2018.BTreeIOComponents.Basics;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;

namespace BTreeFileReader
{
    public class RecordFilePrinter<T> where T : IComparable
    {
        private const long MAX_VALUES_IN_RECORD = 15;
        private const long TYPE_STRING_PREAMBLE_SIZE = 64;

        public void PrintRecordFile(FileIO fileIO, int sizeOfType, bool outputBytes, FileMap map, RichTextBox textBox)
        {
            var valueType = TypeConverter<int>.TypeStringToType(fileIO.GetBytes(0, TYPE_STRING_PREAMBLE_SIZE));
            if (valueType != typeof(T))
                throw new Exception("The file type does not match the current type [" + valueType + " != " +
                                    typeof(T) + "]");
            var structureInfo = new BTreePageStructureInfo<T>(2, sizeOfType);
            sizeOfType = structureInfo.SizeOfType;
            var recordSize = 15 * structureInfo.SizeOfType;
            
            textBox.AppendText("Value type ["+typeof(T)+"]");
            textBox.AppendText(createHeader("FileMap"));
            new FileMapPrinter().printFileMap(map, 10);
            textBox.AppendText("\n");
            textBox.AppendText(createHeader("Records"));


            if (fileIO.FileLength == TYPE_STRING_PREAMBLE_SIZE)
            {
                textBox.AppendText("No records");
                return;
            }
            
            var recordBytes = new byte[0];
            for (var i = 0; i < (fileIO.FileLength - TYPE_STRING_PREAMBLE_SIZE / recordSize); i++)
            {
                try
                {
                    recordBytes = fileIO.GetBytes(TYPE_STRING_PREAMBLE_SIZE + i * recordSize, recordSize);
                }
                catch (EndOfStreamException eos)
                {
                    textBox.AppendText("End of file.");
                    return;
                }

                if (outputBytes)
                    textBox.AppendText("File at [" + (TYPE_STRING_PREAMBLE_SIZE + i * 15 * sizeOfType) +
                                       "]byte\n[ " + BitConverter.ToString(recordBytes).Replace("-", " ") +
                                       " ]\n");
                if (recordBytes.Length == recordSize)
                {
                    try
                    {
                        var record = ByteArrayToRecord<T>(recordBytes, sizeOfType, i);
                        var textBrush = map[i] ? Brushes.White : Brushes.Red;
                        var tr = new TextRange(textBox.Document.ContentEnd, textBox.Document.ContentEnd);
                        tr.Text = "Record:\t" + record + "\n\n";
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, textBrush);
                        textBox.AppendText("Record:\t"+record+"\n\n");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (Exception e)
                    {
                        textBox.AppendText("Record: \tError " + e.Message);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private string createHeader(string text)
        {
            if (text.Length >= 80) return text;
            if (text.Length % 2 == 1) text = string.Concat(text, "-");
            var separator = new string(Enumerable.Repeat('-', (Console.WindowWidth - text.Length) / 2).ToArray());
            return string.Concat(separator, text, separator);
        }
        
        public Record<T> ByteArrayToRecord<T>(byte[] bytes, int sizeOfType, long index) where T : IComparable
        {
            if (bytes.Length != sizeOfType * MAX_VALUES_IN_RECORD)
                throw new Exception("A byte array of " + bytes.Length + " values is not suitable for a record!");
            var valueComponents = new List<T>((int)MAX_VALUES_IN_RECORD);
            for (var i = 0; i < MAX_VALUES_IN_RECORD; i++)
            {
                var j = i * sizeOfType;
                var subBytes = new byte[sizeOfType];
                Array.Copy(bytes, j, subBytes, 0, sizeOfType);
                valueComponents.Add(TypeConverter<T>.ToValue(subBytes));
            }

            return new Record<T>(valueComponents.ToArray(),
                new RecordPointer<T>() {Index = index, PointerType = RecordPointerType.NOT_NULL});
        }
    }
}