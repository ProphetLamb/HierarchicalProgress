using System;

namespace HierarchicalProgress.Events
{
    /// <summary>
    ///     Represents a change to the progress of a <see cref="IProgress{T}"/>.
    /// </summary>
    public abstract class ProgressChangedEventArgs<TProgressValue> : EventArgs where TProgressValue : IProgressReport
    {
        /// <summary>
        /// Instantiates a new <see cref="ProgressReportedEventArgs{TProgressValue}"/>.
        /// </summary>
        protected ProgressChangedEventArgs() { }

        /// <summary>
        /// Instantiates a new <see cref="ProgressReportedEventArgs{TProgressValue}"/>.
        /// </summary>
        /// <param name="previousProgress">The <see cref="IProgressReport"/> reported directly before <see cref="ReportedProgress"/>.</param>
        /// <param name="reportedProgress">The <see cref="IProgressReport"/> reported directly before <see cref="ReportedProgress"/>.</param>
        protected ProgressChangedEventArgs(TProgressValue? previousProgress, TProgressValue? reportedProgress)
        {
            PreviousProgress = previousProgress;
            ReportedProgress = reportedProgress;
        }

        /// <summary>The <see cref="IProgressReport"/> reported directly before <see cref="ReportedProgress"/>.</summary>
        /// <remarks>Null if the progress report ist the first.</remarks>
        public TProgressValue? PreviousProgress { get; set; }

        /// <summary>The <see cref="IProgressReport"/> reported with the progress change.</summary>
        /// <remarks>Not null when initiated properly.</remarks>
        public TProgressValue? ReportedProgress { get; set; }
    }
}
