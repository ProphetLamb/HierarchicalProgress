using System;

using NUnit.Framework;

using HierarchicalProgress.Exceptions;
using System.Collections.Generic;

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

        [Test]
        public void TestObservable()
        {
            HierarchicalProgress<Report> progressProvider = new(new(0, 1), new(0, 100));
            int completedCount́ = 0;
            List<Exception> errors = new();
            List<Report> reports = new();
            IDisposable unsub = progressProvider.Subscribe(new DelegateSubscriber<Report>(
                () => completedCount́++,
                e => errors.Add(e),
                r => reports.Add(r)
            ));
            
        }
    }
}
