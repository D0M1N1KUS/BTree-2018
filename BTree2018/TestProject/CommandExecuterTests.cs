using System;
using System.Collections.Generic;
using System.Linq;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.UtilityClasses;
using NSubstitute;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class CommandExecuterTests
    {
        [Test]
        public void testAddCommandInterpretation()
        {
            var commandExecuter = Substitute.For<ICommandExecuter<int>>();
            var bTree = Substitute.For<IBTree<int>>();
            var multiCommandExecuter = new MultipleOperationExecuter<int>(bTree) {Executer = commandExecuter};
            var commandString = "add 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15" + Environment.NewLine +
                                "add 15 14 13 12 11 10 9 8 7 6 5 4 3 2 1";
            var expectedCommands = new List<string[]>
            {
                new[] { "add", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" },
                new[] { "add", "15", "14", "13", "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" }
            };

            multiCommandExecuter.Execute(commandString);
            
            commandExecuter.Received().ExecuteCommands(Arg.Is<List<string[]>>(commands => CollectionsAreEqual(commands, expectedCommands)));
        }

        private static bool CollectionsAreEqual(List<string[]> a, List<string[]> b)
        {
            var collectionsAreEqual = true;
            try
            {
                for (var i = 0; i < a.Count; i++)
                {
                    for (var j = 0; j < a[i].Length; j++)
                    {
                        if (a[i][j].Equals(b[i][j])) continue;
                        Console.WriteLine("\"{0}\" != \"{1}\" at [{2}][{3}]",a[i][j], b[i][j], i, j);
                        collectionsAreEqual = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message + "\n\n" + e.StackTrace);
                return false;
            }

            return true;
        }
    }
}