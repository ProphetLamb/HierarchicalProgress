using System;

using NUnit.Framework;

using HierarchicalProgress.Exceptions;

namespace HierarchicalProgress.Tests
{
    [TestFixture]
    public class CoreTests
    {
        [Test]
        public void TestReport()
        {
            HierarchicalProgress<Report> progressProvider = new(new(0, 1), new(0, 100));
            progressProvider.Report(new Report(0.0m, "No progress."));
            Assert.AreEqual(0.0m, progressProvider.Progress.Value);

            progressProvider.Report(new Report(50.0m, "Half progress."));
            Assert.AreEqual(0.5m, progressProvider.Progress.Value);

            progressProvider.Report(new Report(100.0m, "All progress."));
            Assert.AreEqual(1.0m, progressProvider.Progress.Value);

            Assert.Throws<InvalidProgressStateException>(() => progressProvider.Report(new Report(25.0m, "Quater progress.")));
        }
    }
}
