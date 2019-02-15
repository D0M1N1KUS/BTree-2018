using System;
using System.Collections.Generic;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;

namespace BTree2018.UtilityClasses
{
    public class MultipleOperationExecuter<T> where T : IComparable
    {
        public ICommandExecuter<T> Executer;

        public MultipleOperationExecuter(IBTree<T> bTree)
        {
            Executer = new CommandExecuter<T>()
            {
                bTree = bTree
            };
        }

        public void Execute(string operationString)
        {
            var lines = operationString.Split('\n');
            var commands = getCommands(lines);

            Executer.ExecuteCommands(commands);
        }



        private List<string[]> getCommands(string[] lines)
        {
            var commands = new List<string[]>(lines.Length);
            foreach (var line in lines)
            {
                var cleanLine = line[line.Length - 1].Equals('\r') 
                    ? line.Substring(0, line.Length - 1) 
                    : line;
                commands.Add(line.Split(' '));
            }

            return commands;
        }

        
    }
}