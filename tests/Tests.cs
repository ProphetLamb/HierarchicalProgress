using NUnit.Framework;

namespace HierarchicalProgress.Tests
{
    public class Tests
    {
        [Test]
        public void TestInit()
        {
            HierarchicalProgress<Foo> main = new(100.0);
            (double offset, double length) = main.ProgressRange.GetOffsetAndLength(100.0);
            Assert.AreEqual(0.0, offset);
            Assert.AreEqual(100.0, length);
        }

        [Test]
        public void TestSlice()
        {
            HierarchicalProgress<Foo> main = new(100.0);
            var lo50 = main.Slice(new (0.0, 50.0));
            var hi50 = main.Slice(new(50.0, 100.0));
            Assert.IsTrue(main.ProgressRange.Encompasses(lo50.ProgressRange, 100.0));
            Assert.IsTrue(main.ProgressRange.Encompasses(hi50.ProgressRange, 100.0));
        }
    }
}