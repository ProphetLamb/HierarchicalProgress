using System;

using GenericRange;

namespace HierarchicalProgress.Events
{
    /// <inheritdoc/>
    public class ProgressReportedEventArgs<TProgressValue> : ProgressChangedEventArgs<TProgressValue> where TProgressValue : IProgressReport
    {
        /// <inheritdoc/>
        public ProgressReportedEventArgs() { }

        /// <summary>
        /// Instantiates a new <see cref="ProgressReportedEventArgs{TProgressValue}"/>.
        /// </summary>
        /// <param name="previousProgress">The <see cref="IProgressReport"/> reported directly before <see cref="ReportedProgress"/>.</param>
        /// <param name="reportedProgress">The <see cref="IProgressReport"/> reported with the progress change.</param>
        /// <param name="change">The manner in which the report has affected the progress.</param>
        /// <param name="currentProgress">Represents the current progress value in the range of <see cref="ProgressBoundaries"/>.</param>
        /// <param name="progressBoundaries">Represents the minimum and maximum progress values allowed to be reported.</param>
        public ProgressReportedEventArgs(TProgressValue? previousProgress, TProgressValue reportedProgress, ProgressChange change, Index<decimal> currentProgress, Range<decimal> progressBoundaries)
            : base(previousProgress, reportedProgress)
        {
            Change = change;
            CurrentProgress = currentProgress;
            ProgressBoundaries = progressBoundaries;
        }
        
        /// <summary>The manner in which the report has affected the progress.</summary>
        public ProgressChange Change { get; set; }
        
        /// <summary>Represents the current progress value in the range of <see cref="ProgressBoundaries"/>.</summary>
        /// <remarks><see cref="Index{T}.IsFromEnd"/> has to be <see langword="false"/>.</remarks>
        public Index<decimal> CurrentProgress { get; set; }
        
        /// <summary>Represents the minimum and maximum progress values allowed to be reported.</summary>
        /// <remarks><see cref="Index{T}.IsFromEnd"/> has to be <see langword="false"/>.</remarks>
        public Range<decimal> ProgressBoundaries { get; set; }
    }
}
