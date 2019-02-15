using System;
using System.IO;
using System.Linq;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeIOComponents.Basics
{
    public class FileIO : IFileIO
    {
        private IFileInput input;
        private IFileOutput output;
        private FileInfo fileInfo;

        /// <summary>
        /// Initialize FileIO with existing objects. File input and output must point to the same file
        /// </summary>
        /// <param name="fileInput">File input object</param>
        /// <param name="fileOutput">File output object</param>
        /// <param name="fileInfo">FileInfo object</param>
        /// <exception cref="Exception">Throw exception if files in fileInput and fileOutput differ</exception>
        public FileIO(IFileInput fileInput, IFileOutput fileOutput, FileInfo fileInfo)
        {
            if(fileInput.FilePath != fileOutput.FilePath || fileOutput.FilePath != fileInfo.FullName)
                throw new Exception("File paths of provided objects do not match! \nFileInput file: ["+ 
                                    fileInput.FilePath + "] \nFileOutput file: [" + fileOutput + 
                                    "] \nFileInfo [" + fileInfo.FullName + "].");
            input = fileInput;
            output = fileOutput;
            this.fileInfo = fileInfo;
        }

        /// <summary>
        /// Opens or creates a new file
        /// </summary>
        /// <param name="filePath">Path to existing or new file</param>
        public FileIO(string filePath)
        {
            input = new FileInput(filePath);
            output = new FileOutput(filePath);
            fileInfo = new FileInfo(filePath);
        }

        public long FileLength
        {
            get
            {
                fileInfo.Refresh();
                return fileInfo.Length;
            }
        }


        public byte[] GetBytes(long begin, long n)
        {
            return output.GetBytes(begin, n);
        }

        public byte GetByte(long index)
        {
            return output.GetBytes(index, 1)[0];
        }

        public void WriteBytes(byte[] bytes, long begin)
        {
            input.WriteBytes(bytes, begin);
        }

        public void WriteZeros(long begin, long n)
        {
            input.WriteBytes(Enumerable.Repeat((byte)0, (int)n).ToArray(), begin);
        }
    }
}