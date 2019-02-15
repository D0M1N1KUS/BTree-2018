using System;

namespace BTree2018.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException()
        {
            
        }
        
        public DuplicateKeyException(string message) : base(message)
        {
            
        }
    }
}