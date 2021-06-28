using System;

namespace HierarchicalProgress.Tests
{
    public class Report : IProgressReport
    {
        public Report()
        {
        }

        public Report(decimal reportProgress, string? message)
        {
            ReportProgress = reportProgress;
            Message = message;
        }

        public Report(decimal reportProgress, string? message, IProgressReport? inner)
        {
            ReportProgress = reportProgress;
            Message = message;
            Inner = inner;
        }

        public decimal ReportProgress { get; set; }

        public string? Message { get; set; }

        public IProgressReport? Inner { get; set; }
    }
}
