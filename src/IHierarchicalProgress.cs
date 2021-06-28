using System;

using GenericRange;

using HierarchicalProgress.Events;
using HierarchicalProgress.Exceptions;

namespace HierarchicalProgress
{
    /// <summary>
    ///     Defines a strongly typed hierarchical provider for progress updates.
    /// </summary>
    /// <typeparam name="TProgressReport">The <see cref="IProgressReport"/> type of the change.</typeparam>
    public interface IHierarchicalProgress<TProgressReport> : IProgress<TProgressReport>, IObservable<TProgressReport> where TProgressReport : IProgressReport, new()
    {
        event EventHandler<ProgressResetEventArgs<TProgressReport>> Reset;

        event EventHandler<ProgressCompletedEventArgs<TProgressReport>> Completed;

        event EventHandler<ProgressReportedEventArgs<TProgressReport>> Reported;

        /// <summary>Represents minimum and maximum progress value.</summary>
        /// <remarks><see cref="Index{T}.IsFromEnd"/> has to be <see langword="false"/>.</remarks>
        Range<decimal> ProgressBoundaries { get; }

        /// <summary>Represents the current progress value in the range of <see cref="ProgressBoundaries"/>.</summary>
        /// <remarks>
        ///     Any reported progress value will be mapped from <see cref="ReportBoundaries"/> to <see cref="ProgressBoundaries"/> to assign a value to <see cref="Progress"/>.
        ///     <br/>
        ///     <see cref="Index{T}.IsFromEnd"/> has to be <see langword="false"/>.
        /// </remarks>
        Index<decimal> Progress { get; }

        /// <summary>Represents the portion of the <see cref="ProgressBoundaries"/> reserved for slices of the progress.</summary>
        decimal AllocatedProgress { get; }

        /// <summary>Represents the minimum and maximum progress values allowed to be reported.</summary>
        /// <remarks>
        ///     The upper limit is negatively influenced by <see cref="AllocatedProgress"/> in proportion to the percentage of <see cref="ProgressBoundaries"/>.
        ///     If <see cref="AllocatedProgress"/> equals the length of <see cref="ProgressBoundaries"/> the range will be empty.
        ///     <br/>
        ///     <see cref="Index{T}.IsFromEnd"/> has to be <see langword="false"/>.
        /// </remarks>
        Range<decimal> ReportBoundaries { get; }

        /// <summary>Indicates whether the progress is completed or not.</summary>
        /// <remarks>Once <see langword="true"/> subsequent progress reports will throw a <see cref="InvalidProgressStateException"/>.</remarks>
        bool IsCompleted { get; }

        /// <summary>Represents the latest reported progress.</summary>
        TProgressReport? LatestReport { get; }

        /// <summary>Creates a new <see cref="IHierarchicalProgress{TProgressValue}"/>.</summary>
        /// <param name="reportBoundaries">The value assigned to <see cref="ReportProgressBoundaries"/> of the slice.</param>
        /// <param name="allocateProgress">The amount of the progress of this instance to allocate to the slice. Directly added to <see cref="AllocatedProgress"/>.</param>
        /// <returns>A new <see cref="IHierarchicalProgress{TProgressValue}"/> representing a protion of this progress.</returns>
        /// <remarks>Observes changed to the created <see cref="IHierarchicalProgress{TProgressValue}"/> and reports them.</remarks>
        IHierarchicalProgress<TProgressReport> Slice(Range<decimal> reportBoundaries, decimal allocateProgress);

        /// <summary>Reports the progress provider as complete.</summary>
        /// <param name="report">The report that completes the progress.</param>
        /// <remarks>Once completed subsequent progress reports will throw a <see cref="InvalidProgressStateException"/>.</remarks>
        void ReportComplete(TProgressReport report);
    }
}
