using System;

namespace HierarchicalProgress
{
    /// <summary>
    ///     Transport interface for a <see cref="IProgress{T}"/> with a 64-bit floating-point <see cref="ReportProgress"/> value.
    /// </summary>
    public interface IProgressReport
    {
        /// <summary>
        ///     The <see langword="double"/> indicating the progress reported by the <see cref="IProgressReport"/>.
        /// </summary>
        public double ReportProgress { get; set; }
        
        public IProgressReport? Inner { get; set; }
    }
}
