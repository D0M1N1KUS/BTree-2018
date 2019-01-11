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
        private string filePath;

        /// <summary>
        /// Opens or creates file for writing
        /// </summary>
        /// <param name="filePath">Path to file</param>
        public FileInput(string filePath)
        {
            this.filePath = filePath;
            checkFilePath();
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
        
        public void WriteBytes(byte[] bytes, long begin)
        {
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                if (begin + bytes.Length > stream.Length)
                {
                    for (var i = 0; i < begin - stream.Length; i++)
                        stream.WriteByte(0);
                    stream.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    stream.Position = begin;
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

//        private void append(byte[] bytes, long begin)
//        {
//            using (var stream = File.Open(filePath, FileMode.Append))
//            {
//                for (var i = 0; i < begin - stream.Length; i++)
//                    stream.WriteByte(0);
//                stream.Write(bytes, 0, bytes.Length);
//            }
//        }
//
//        private void overwrite(byte[] bytes, long begin)
//        {
//            using (var stream = File.Open(filePath, FileMode.Open))
//            {
//                stream.Position = begin;
//                stream.Write(bytes, 0, bytes.Length);
//            }
//        }
    }
}