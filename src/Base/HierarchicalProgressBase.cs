using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using GenericRange;

using HierarchicalProgress.DebugViews;
using HierarchicalProgress.Events;

namespace HierarchicalProgress.Base
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    [DebuggerTypeProxy(typeof(EnumerableDebugView<>))]
    public abstract class HierarchicalProgressBase<TProgressReport> : NotifyPropertyChangedBase, IHierarchicalProgress<TProgressReport>, IEnumerable<TProgressReport>
        where TProgressReport : IProgressReport, new()
    {
#region Ctor

        protected HierarchicalProgressBase(Range<decimal> progressBoundaries, Range<decimal> reportBoundaries)
        {
            // Asserts that neither range has indices from end.
            _ = progressBoundaries.GetOffsetAndLength();
            _ = reportBoundaries.GetOffsetAndLength();

            ProgressBoundaries = progressBoundaries;
            OriginReportBoundaries = _reportBoundaries = reportBoundaries;
        }

#endregion

#region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Index<decimal> _progress;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private decimal _allocatedProgress;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Range<decimal> _reportBoundaries;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private (ProgressChange Type, decimal Delta) _latestChange;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<TProgressReport> m_progressReports = new();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TProgressReport? _latestReport;

#endregion

#region Properties

        public event EventHandler<ProgressResetEventArgs<TProgressReport>>? Reset;

        public event EventHandler<ProgressCompletedEventArgs<TProgressReport>>? Completed;

        public event EventHandler<ProgressReportedEventArgs<TProgressReport>>? Reported;

        public Range<decimal> ProgressBoundaries { get; }

        public Index<decimal> Progress
        {
            get => _progress;
            protected set => Set(ref _progress, value);
        }

        public decimal AllocatedProgress
        {
            get => _allocatedProgress;
            protected set => Set(ref _allocatedProgress, value);
        }

        public Range<decimal> ReportBoundaries
        {
            get => _reportBoundaries;
            protected set => Set(ref _reportBoundaries, value);
        }

        /// <summary>The report boundaries with which the <see cref="HierarchicalProgress{TProgressReport}"/> was initialized.</summary>
        public Range<decimal> OriginReportBoundaries { get; }

        /// <summary>Represents the manner in which the latest report changed the progress and the value by witch the progress was changed.</summary>
        /// <remarks>A negative <see cref="LatestChange.Delta"/> value indicates a decrease in progress, and a positive value an increase in progress.</remarks>
        public (ProgressChange Type, decimal Delta) LatestChange
        {
            get => _latestChange;
            protected set
            {
                Set(ref _latestChange, value);
                if (value.Type == ProgressChange.Completed)
                    OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public TProgressReport? LatestReport
        {
            get => _latestReport;
            protected set => Set(ref _latestReport, value);
        }

        public bool IsCompleted => LatestChange.Type == ProgressChange.Completed;

#endregion

#region Public members

        public void Report(TProgressReport value)
        {
            InternalReport(value, GetReportProgressChange(LatestReport, value));
        }

        public void ReportComplete(TProgressReport? report)
        {
            InternalReport(report, ProgressChange.Completed);
        }

        /// <summary>Creates a new <see cref="IHierarchicalProgress{TProgressValue}"/>.</summary>
        /// <param name="allocateProgress">The amount of the progress of this instance to allocate to the slice. Directly added to <see cref="AllocatedProgress"/>.</param>
        /// <returns>A new <see cref="IHierarchicalProgress{TProgressValue}"/> representing a protion of this progress.</returns>
        /// <remarks>
        ///     Observes changed to the created <see cref="IHierarchicalProgress{TProgressValue}"/> and reports them.
        ///     No strong reference to the slice is maintained.
        /// </remarks>
        public IHierarchicalProgress<TProgressReport> Slice(decimal allocateProgress) => Slice(OriginReportBoundaries, allocateProgress);

        // Implement strongly typed. Use explicit implementation for the trade interface.
        /// <inheritdoc cref="IHierarchicalProgress{TProgressReport}.Slice(Range{decimal}, decimal)"/>
        public abstract HierarchicalProgress<TProgressReport> Slice(Range<decimal> reportBoundaries, decimal allocateProgress);

        IHierarchicalProgress<TProgressReport> IHierarchicalProgress<TProgressReport>.Slice(Range<decimal> reportBoundaries, decimal allocateProgress) => Slice(reportBoundaries, allocateProgress);

        public abstract IDisposable Subscribe(IObserver<TProgressReport> observer);

        public IEnumerator<TProgressReport> GetEnumerator() => ((IEnumerable<TProgressReport>)m_progressReports).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#endregion

#region Internal members

        protected abstract void InternalReport(TProgressReport? report, ProgressChange change);

        protected abstract ProgressChange GetReportProgressChange(TProgressReport? previous, TProgressReport report);

        protected virtual void OnReported(TProgressReport? previousProgress, TProgressReport reportedProgress, ProgressChange change)
        {
            Reported?.Invoke(this, new ProgressReportedEventArgs<TProgressReport>(previousProgress, reportedProgress, change, Progress, ProgressBoundaries));
        }

        protected virtual void OnReset(TProgressReport? previousProgress, TProgressReport? reportedProgress)
        {
            Reset?.Invoke(this, new ProgressResetEventArgs<TProgressReport>(previousProgress, reportedProgress));
        }

        protected virtual void OnCompleted(TProgressReport? previousProgress, TProgressReport? reportedProgress)
        {
            Completed?.Invoke(this, new ProgressCompletedEventArgs<TProgressReport>(previousProgress, reportedProgress));
        }

        private string GetDebuggerDisplay()
        {
            return $"Reports: {m_progressReports.Count}, {nameof(Progress)}: {Progress}, {nameof(ProgressBoundaries)}: {ProgressBoundaries}, {nameof(AllocatedProgress)}: {AllocatedProgress}, {nameof(ReportBoundaries)}: {ReportBoundaries}";
        }
        
#endregion
    }
}
