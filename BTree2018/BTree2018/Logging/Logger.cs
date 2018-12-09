using System;
using System.Collections;
using System.Text;

namespace BTree2018.Logging
{
    public static class Logger
    {
        private static StringBuilder messageBuilder = new StringBuilder();
        public static int Messages { get; private set; } = 0;

        public static void Log(string message)
        {
            messageBuilder.Append(getCurrentTime());
            messageBuilder.Append(message);
            messageBuilder.Append(Environment.NewLine);
            Messages++;
        }

        public static void Log(Exception e)
        {
            messageBuilder.Append(getCurrentTime());
            messageBuilder.Append(e.Message);
            messageBuilder.Append(Environment.NewLine);
            messageBuilder.Append(e.StackTrace);
            if (e.Data.Count > 0) messageBuilder.Append(getExceptionData(e.Data));
            messageBuilder.Append(Environment.NewLine);
            Messages++;
        }

        private static string getCurrentTime()
        {
            var timeOfLog = DateTime.Now.ToString("HH:mm:ss tt zz");
            return string.Concat("[", timeOfLog, "] ");
        }

        private static string getExceptionData(IDictionary eData)
        {
            var exceptionDataBuilder = new StringBuilder();
            exceptionDataBuilder.Append(Environment.NewLine);
            exceptionDataBuilder.Append("\tException data dump:");
            foreach (DictionaryEntry entry in eData)
            {
                exceptionDataBuilder.Append(Environment.NewLine);
                exceptionDataBuilder.Append("\t\t" + entry.Key + "\t|\t" + entry.Value);
            }

            return exceptionDataBuilder.ToString();
        }

        public static string GetLog()
        {
            if(Messages == 0) return string.Empty;
            
            var currentLog = messageBuilder.ToString();
            messageBuilder.Clear();
            Messages = 0;
            
            return currentLog;
        }
    }
}