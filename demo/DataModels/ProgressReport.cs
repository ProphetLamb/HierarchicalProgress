namespace HierarchicalProgress.Demo.DataModels
{
    public class ProgressReport : IProgressReport
    {
        public double ReportProgress { get; set; }
        
        public string? Message { get; set; }

        public IProgressReport? Inner { get; set; }
    }
}
