using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BTree2018.UtilityClasses
{
    public static class InputValidation
    {
        public static bool TryParseTextBoxCollection<T>(IReadOnlyList<TextBox> textBoxes, Action<TextBox> textBoxInvalidMethod,
            Action<TextBox> textBoxValidMethod) where T : IComparable
        {
            var allTextBoxedAreValid = true;
            foreach (var textBox in textBoxes)
            {
                if (!textBox.Text.Equals(string.Empty) && TryParse<T>(textBox.Text))
                    textBoxValidMethod(textBox);
                else
                {
                    textBoxInvalidMethod(textBox);
                    allTextBoxedAreValid = false;
                }
            }

            return allTextBoxedAreValid;
        }
        
        public static bool TryParse<T>(string value)
        {
            var type = typeof(T);

            if (type == typeof(short))
                return short.TryParse(value, out _);
            if (type == typeof(int))
                return int.TryParse(value, out _);
            if (type == typeof(long))
                return long.TryParse(value, out _);
            if (type == typeof(float))
                return float.TryParse(value, out _);
            if (type == typeof(double))
                return double.TryParse(value, out _);
            throw new Exception("Unsupported type: " + type);
        }
    }
}