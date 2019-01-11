using System.Linq;
using NSubstitute;
using NUnit.Framework;
using UnitTests.FileIOTests.FileClassesTests;

namespace UnitTests
{
    [TestFixture]
    public class NSubstituteTests
    {
        [Test]
        public void substituteReceivedCallForMethodWithCollectionTest()
        {
            var testSubstitute = Substitute.For<ITestInterface>();
            var testByteArray = new byte[] {1, 2, 3, 4};
            var testByteArray2 = new byte[] {1, 2, 3, 4};
            
            testSubstitute.MethodWithCollectionParameter(testByteArray);
            
            testSubstitute.Received().MethodWithCollectionParameter(testByteArray);
            Assert.Throws<NSubstitute.Exceptions.ReceivedCallsException>(
                () => testSubstitute.Received().MethodWithCollectionParameter(testByteArray2));
            testSubstitute.Received().MethodWithCollectionParameter(Arg.Is<byte[]>(b => b.SequenceEqual(testByteArray2)));
        }
    }
}