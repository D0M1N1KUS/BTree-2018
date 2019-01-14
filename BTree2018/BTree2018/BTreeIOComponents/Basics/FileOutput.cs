using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using BTree2018.Interfaces.FileIO;
using BTree2018.Logging;

namespace BTree2018.BTreeIOComponents.Basics
{
    public class FileOutput : IFileOutput
    {
        private string filePath;
        private FileInfo fileInfo;
        
        public FileOutput(string filePath)
        {
            this.filePath = filePath;
            checkFilePath();
            fileInfo = new FileInfo(filePath);
        }

        private void checkFilePath()
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException("The file does not exist \"" + filePath + "\"");
            var permissionSet = new PermissionSet(PermissionState.None);
            var readPermission = new FileIOPermission(FileIOPermissionAccess.Read, filePath);
            permissionSet.AddPermission(readPermission);
            if(!permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                throw new Exception("Cannot access file for reading \"" + filePath + "\"");
        }
        
        public long Length => fileInfo.Length;
        public string FilePath => filePath;

        public byte[] GetBytes(long begin, long n)
        {
            var listOfBytes = new byte[n];
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                stream.Position = begin;
                var bytesRead = stream.Read(listOfBytes, 0, (int)n);
                if(bytesRead < n) 
                    throw new EndOfStreamException("End of stream reached after reading [" + bytesRead + 
                                                   " / " + n + "] bytes starting from [" + begin + "]. The file is [" + 
                                                   stream.Length + "] bytes long.");
            }
            
            fileInfo.Refresh();
            
            return listOfBytes.ToArray();
        }
    }
}