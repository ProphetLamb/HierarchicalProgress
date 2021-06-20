namespace HierarchicalProgress
{
    /// <summary>
    /// Describes the manner in which the progress has changed between reports.
    /// </summary>
    public enum ProgressChange : byte
    {
        /// <summary>The progress has not changed.</summary>
        None,
        /// <summary>The progress has progressed.</summary>
        Increment,
        /// <summary>The progress has regressed.</summary>
        Decrement,
        /// <summary>The progress is reset to the initial progress. Usually zero.</summary>
        Reset,
        /// <summary>The progress is completed.</summary>
        Completed
    }
}
