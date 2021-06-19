namespace HierarchicalProgress.Tests
{
    public class Foo : IProgressValue
    {
        public double Progress { get; set; }
        
        public string? Message { get; set; }

        public object Clone() => new Foo { Progress = Progress, Message = Message };
    }
}
