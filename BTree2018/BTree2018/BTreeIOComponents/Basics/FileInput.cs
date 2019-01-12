using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using BTree2018.Interfaces.FileIO;

namespace BTree2018.BTreeIOComponents.Basics
{
    public class FileInput : IFileInput
    {
        private readonly byte[] ZERO_BYTE = {0b0000_0000};
        
        private string filePath;
        private FileInfo fileInfo;

        /// <summary>
        /// Opens or creates file for writing
        /// </summary>
        /// <param name="filePath">Path to file</param>
        public FileInput(string filePath)
        {
            this.filePath = filePath;
            checkFilePath();
            fileInfo = new FileInfo(filePath);
        }

        private void checkFilePath()
        {
            if(File.Exists(filePath))
            {
                var permissionSet = new PermissionSet(PermissionState.None);
                var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, filePath);
                var appendPermission = new FileIOPermission(FileIOPermissionAccess.Append, filePath);
                permissionSet.AddPermission(writePermission);
                permissionSet.AddPermission(appendPermission);
                if(!permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                    throw new Exception("Cannot access file for writing and/or appending \"" + filePath + "\"");
            }
            else
            {
                if (new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]").IsMatch(filePath))
                    throw new FileLoadException("The provided file path is invalid \"" + filePath + "\"");
                File.Create(filePath);
            }
        }

        public long Length => fileInfo.Length;
        public string FilePath => filePath;

        public void WriteBytes(byte[] bytes, long begin)
        {
            if (bytes.Length == 0) return;
            if (begin + bytes.Length <= fileInfo.Length)
                overwrite(bytes, begin);
            else if (begin < fileInfo.Length && begin + bytes.Length > fileInfo.Length)
            {
                splitBytesArray(bytes, begin, out var bytesThatFit, out var bytesThatDontFit);
                overwrite(bytesThatFit, begin);
                append(bytesThatDontFit, fileInfo.Length);
            }
            else
                append(bytes, begin);
            
            fileInfo.Refresh();
        }

        private void splitBytesArray(byte[] bytes, long begin, out byte[] bytes1, out byte[] bytes2)
        {
            var bytesThatFit = fileInfo.Length - begin;
            var bytesThatDontFit = bytes.Length - bytesThatFit;
            bytes1 = new byte[bytesThatFit];
            bytes2 = new byte[bytesThatDontFit];
            Array.Copy(bytes, 0, bytes1, 0, bytesThatFit);
            Array.Copy(bytes, bytesThatFit, bytes2, 0, bytesThatDontFit);
        }

        private void append(byte[] bytes, long begin)
        {
            using (var stream = File.Open(filePath, FileMode.Append))
            {
                var currentFileLength = stream.Length;
                for (var i = 0; i < begin - currentFileLength; i++)
                    stream.Write(ZERO_BYTE, 0, ZERO_BYTE.Length);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        private void overwrite(byte[] bytes, long begin)
        {
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                stream.Position = begin;
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }
    }
}