using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;

namespace BTree2018.UtilityClasses
{
    public static class TextInputConverter
    {
//        public static IRecord<T> ConvertToRecord<T>(T[] valueComponents, IRecordPointer<T> pointer = null)
//            where T : IComparable
//        {
//            return new Record<T>(valueComponents, pointer ?? RecordPointer<T>.NullPointer);
//        }

        public static IRecord<T> ConvertToRecord<T>(string[] valueComponents, IRecordPointer<T> pointer = null)
            where T : IComparable
        {
            var type = typeof(T);
            
            if (type == typeof(short))
                return (IRecord<T>)(object)(new Record<short>(valueComponents.Select(short.Parse).ToArray(), 
                    (IRecordPointer<short>)(object)pointer));
            if (type == typeof(int))
                return (IRecord<T>)(object)(new Record<int>(valueComponents.Select(int.Parse).ToArray(), 
                    (IRecordPointer<int>)(object)pointer));
            if (type == typeof(long))
                return (IRecord<T>)(object)(new Record<long>(valueComponents.Select(long.Parse).ToArray(), 
                    (IRecordPointer<long>)(object)pointer));
            if (type == typeof(float))
                return (IRecord<T>)(object)(new Record<float>(valueComponents.Select(float.Parse).ToArray(), 
                    (IRecordPointer<float>)(object)pointer));
            if (type == typeof(double))
                return (IRecord<T>)(object)(new Record<double>(valueComponents.Select(double.Parse).ToArray(), 
                    (IRecordPointer<double>)(object)pointer));
            
            throw new Exception("Unsupported type: " + type);
        }

        public static IRecord<T> ConvertToRecord<T>(TextBox[] valueTextBoxes, IRecordPointer<T> pointer = null)
            where T : IComparable
        {
            return ConvertToRecord<T>(valueTextBoxes.Select(textBox => textBox.Text).ToArray(), pointer);
        }

        public static IKey<T> ConvertToKey<T>(TextBox valueTextBox, IRecordPointer<T> pointer = null)
            where T : IComparable
        {
            var type = typeof(T);
            
            if (type == typeof(short))
                return (IKey<T>)(object)(new BTreeKey<short>(){Value = short.Parse(valueTextBox.Text), 
                    RecordPointer = (IRecordPointer<short>)pointer ?? RecordPointer<short>.NullPointer});
            if (type == typeof(int))
                return (IKey<T>)(object)(new BTreeKey<int>(){Value = int.Parse(valueTextBox.Text), 
                    RecordPointer = (IRecordPointer<int>)pointer ?? RecordPointer<int>.NullPointer});
            if (type == typeof(long))
                return (IKey<T>)(object)(new BTreeKey<long>(){Value = long.Parse(valueTextBox.Text), 
                    RecordPointer = (IRecordPointer<long>)pointer ?? RecordPointer<long>.NullPointer});
            if (type == typeof(float))
                return (IKey<T>)(object)(new BTreeKey<float>(){Value = float.Parse(valueTextBox.Text), 
                    RecordPointer = (IRecordPointer<float>)pointer ?? RecordPointer<float>.NullPointer});
            if (type == typeof(double))
                return (IKey<T>)(object)(new BTreeKey<double>(){Value = double.Parse(valueTextBox.Text), 
                    RecordPointer = (IRecordPointer<double>)pointer ?? RecordPointer<double>.NullPointer});
            
            throw new Exception("Unsupported type: " + type);
        }
    }
}