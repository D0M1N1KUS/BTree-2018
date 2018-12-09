using System;
using BTree2018.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public void addMessagesToLogClass()
        {
            Logger.Log("Som Ting Wong");
            Logger.Log("Gon Na Krash");
            Logger.Log("Out Ta Luck");
            
            Assert.AreEqual(3, Logger.Messages);
            Console.Write(Logger.GetLog());
            Assert.AreEqual(0, Logger.Messages);
        }
    }
}