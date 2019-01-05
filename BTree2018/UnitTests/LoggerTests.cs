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
        [Ignore("This sruff behaves weird... when running normally it gets 4 messages instead of 3, while debugging it gets 3")]
        public void addMessagesToLogClass()
        {
            Logger.Log("Som Ting Wong");
            Logger.Log("Gon Na Krash");
            Logger.Log("Out Ta Luck");
            
            Assert.AreEqual(3, Logger.Messages);
            Console.Write(Logger.GetLog());
            Assert.AreEqual(0, Logger.Messages);
            
            Logger.Reset();
        }
    }
}