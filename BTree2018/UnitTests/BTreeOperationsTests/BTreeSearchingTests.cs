using BTree2018.BTreeStructure;
using BTree2018.Interfaces.BTreeStructure;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.BTreeOperationsTests
{
    [TestFixture]
    public class BTreeSearchingTests
    {
        [Test]
        public void findKeyRecordPair()
        {
            var rootPage = new BTreePage<int>()
            {
                Keys = new IKey<int>[]
                {
                    new BTreeKey<int>()
                    {
                        LeftPagePointer = Substitute.For<IPagePointer<int>>(), 
                    }
                }
            };
        }
    }
}