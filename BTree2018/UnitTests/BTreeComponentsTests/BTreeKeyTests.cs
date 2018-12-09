using BTree2018.BTreeStructure;
using BTree2018.Builders;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace UnitTests.BTreeComponentsTests
{
    [TestFixture]
    public class BTreeKeyTests
    {
        public void makeKeyWithDoubleValues()
        {
            var pageNullPointer = Substitute.For<IPageNullPointer<double>>();
            var record = Substitute.For<IRecord<double>>();
            record.Value.Returns(0.123);
            record.ValueComponents.Returns(new double[] {0.0, 0.0, 0.123, 0.0123});
//            var key = Substitute.For<IKey<double>>();
//            key.Value.Returns(0.123);
//            key.N.Returns(1);
//            key.Record.Returns(record);
//            key.LeftPagePointer.Returns(pageNullPointer);
//            key.RightPagePointer.Returns(pageNullPointer);
            var key = new BTreeKeyBuilder<double>()
            {
                N = 1, Record = record, LeftPage = pageNullPointer, RightPage = pageNullPointer
            }.Build();
        }
    }
}