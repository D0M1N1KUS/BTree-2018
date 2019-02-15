using System;
using System.Collections.Generic;

namespace BTree2018.Interfaces
{
    public interface ICommandExecuter<T> where T : IComparable
    {
        void ExecuteCommands(List<string[]> commands);
    }
}