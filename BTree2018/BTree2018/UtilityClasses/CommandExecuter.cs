using System;
using System.Collections.Generic;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;

namespace BTree2018.UtilityClasses
{
    public class CommandExecuter<T> : ICommandExecuter<T> where T : IComparable
    {
        public IBTree<T> bTree;
        public void ExecuteCommands(List<string[]> commands)
        {
            for (var i = 0; i < commands.Count; i++)
            {
                try
                {
                    tryToExecute(commands, i);
                }
                catch (Exception e)
                {
                    Logger.Log("Failed to execute command line " + (i + 1));
                    Logger.Log(e);
                }
            }
        }

        private void tryToExecute(List<string[]> commands, int i)
        {
            var command = commands[i];
            var currentCommand = command[0];
            if (currentCommand.ToLower().Equals("add"))
            {
                addRecord(command, i);
            }
            else if (currentCommand.ToLower().Equals("rem"))
            {
                removeRecord(command);
            }
            else if (currentCommand.ToLower().Equals("alt"))
            {
                replaceRecord(command, i);
            }
            else if(currentCommand == string.Empty || currentCommand.Length < 3) { /*ignore empty lines*/ }
            else
            {
                Logger.Log("Unknown command: " + currentCommand);
            }
        }

        private void removeRecord(string[] command)
        {
            var key = TextInputConverter.ConvertToKey<T>(command[1]);
            bTree.Remove(key);
            Logger.Log("Successfully removed record with value " + key.Value);
        }

        private void addRecord(string[] command, int i)
        {
            var record = buildRecordFromParams(command, i);
            bTree.Add(record);
            Logger.Log("Successfully added record " + record);
        }

        private void replaceRecord(string[] command, int i)
        {
            var record = buildRecordFromParams(command, i, 2);
            var key = TextInputConverter.ConvertToKey<T>(command[1]);
            bTree.Replace(key, record);
            Logger.Log("Successfully replaced " + key.Value + " with " + record.Value);
        }
        
        private IRecord<T> buildRecordFromParams(string[] command, int i, int begin = 1)
        {
            if(command.Length - begin != 15)
                throw new Exception("Invalid command parameters in line " + (i + 1));
            var valueComponents = new string[15];
            Array.Copy(command, begin, valueComponents, 0, 15);
            return TextInputConverter.ConvertToRecord<T>(valueComponents);
        }
    }
}