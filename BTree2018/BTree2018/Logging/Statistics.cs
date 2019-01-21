using System;

namespace BTree2018.Logging
{
    public static class Statistics
    {
        private static long bytesRead = 0;
        private static long bytesWritten = 0;
        private static long pagesRead = 0;
        private static long pagesWritten = 0;
        
        public static void AddReadBytes(long numberOfReadBytes)
        {
            bytesRead += numberOfReadBytes;
        }

        public static void AddWrittenBytes(long numberOfWrittenBytes)
        {
            bytesWritten += numberOfWrittenBytes;
        }

        public static void AddReadPages(long numberOfReadPages)
        {
            pagesRead += numberOfReadPages;
        }

        public static void AddWrittenPages(long numberOfWrittenPages)
        {
            pagesWritten += numberOfWrittenPages;
        }

        public static Tuple<long, long, long, long> GetStatistics(bool clearStatistics = true)
        {
            var statistics = new Tuple<long, long, long, long>(bytesRead, bytesWritten, pagesRead, pagesWritten);
            if (clearStatistics) bytesRead = pagesRead = bytesWritten = pagesWritten = 0;
            return statistics;
        }
    }
}