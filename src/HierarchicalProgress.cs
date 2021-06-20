using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using GenericRange;
using GenericRange.Extensions;

using HierarchicalProgress.Base;
using HierarchicalProgress.DebugViews;
using HierarchicalProgress.Exceptions;

namespace HierarchicalProgress
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    [DebuggerTypeProxy(typeof(EnumerableDebugView<>))]
    public partial class HierarchicalProgress<TProgressReport> : HierarchicalProgressBase<TProgressReport>
        where TProgressReport : IProgressReport, new()
    {
#region Ctor

        public HierarchicalProgress(Range<double> progressBoundaries, Range<double> reportBoundaries)
            : base(progressBoundaries, reportBoundaries)
        { }
        
#endregion
        
#region Fields

        private readonly List<IObserver<TProgressReport>> _observers = new();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _sliceObserverSyncLock = new();

#endregion

#region Public members
        
        public override HierarchicalProgress<TProgressReport> Slice(Range<double> reportBoundaries, double allocateProgress)
        {
            double totalAvailableProgress = ProgressBoundaries.GetOffsetAndLength().Length;
            double freeProgress = totalAvailableProgress - AllocatedProgress - allocateProgress;
            if (freeProgress < 0)
                throw new ArgumentOutOfRangeException(nameof(allocateProgress), "Insufficient progressValue to allocate.");
            
            // Calculate new ReportBoundaries
            double freeProgressPercent = freeProgress / totalAvailableProgress;
            (double reportOff, double reportLen) = OriginReportBoundaries.GetOffsetAndLength();
            
            ReportBoundaries = new Range<double>(reportOff, reportOff + reportLen * freeProgressPercent);
            AllocatedProgress -= allocateProgress;

            Range<double> sliceProgressBoundaries = new(ProgressBoundaries.Start.Value, ProgressBoundaries.Start.Value + allocateProgress);
            var slice = new HierarchicalProgress<TProgressReport>(sliceProgressBoundaries, reportBoundaries);
            
            SliceObserver observer = new(this, slice);
            observer.Unsubscriber = Subscribe(observer);

            return slice;
        }

        public override IDisposable Subscribe(IObserver<TProgressReport> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                foreach (TProgressReport report in m_progressReports)
                    observer.OnNext(report);
            }

            return new Unsubscriber(_observers, observer);
        }

#endregion

#region Internal members

        protected override void InternalReport(TProgressReport? report, ProgressChange change)
        {
            ThrowIfCompleted(IsCompleted, change);
            // Only completion can be done without a report.
            Debug.Assert(report != null || change == ProgressChange.Completed, "report != null || change == ProgressChange.Completed");
            
            double progress = change switch {
                ProgressChange.Completed => ProgressBoundaries.End.Value,
                ProgressChange.Reset => ProgressBoundaries.Start.Value,
                _ => ReportBoundaries.Map(ProgressBoundaries, report!.ReportProgress)
            };
            
            if (!ProgressBoundaries.Contains(progress))
                throw new IndexOutOfRangeException($"The index {progress} is outside of the range {ProgressBoundaries}.");
            
            TProgressReport? previous = LatestReport;
            LatestChange = (change, progress - Progress.Value);
            Progress = progress;
            LatestReport = report;
            OnChanged(previous, report, change);
        }

        /// <summary>Notifies all listeners when a progress changes is reported.</summary>
        protected virtual void OnChanged(TProgressReport? previousProgress, TProgressReport? reportedProgress, ProgressChange change)
        {
            // Only completion can be done without a report.
            Debug.Assert(reportedProgress != null || change == ProgressChange.Completed, "report != null || change == ProgressChange.Completed");

            foreach (IObserver<TProgressReport> observer in _observers)
            {
                if (reportedProgress != null)
                    observer.OnNext(reportedProgress);
                if (change == ProgressChange.Completed)
                    observer.OnCompleted(); // Clear the _observers list
            }

            if (reportedProgress != null)
            {
                m_progressReports.Add(reportedProgress);

                OnReported(previousProgress, reportedProgress, change);
            }
            
            switch (change)
            {
                case ProgressChange.Reset:
                    OnReset(previousProgress, reportedProgress);
                    break;
                case ProgressChange.Completed:
                    OnCompleted(previousProgress, reportedProgress);
                    break;
            }
        }

        protected override ProgressChange GetReportProgressChange(TProgressReport? previous, TProgressReport report)
        {
            ProgressChange changeType = previous?.ReportProgress.CompareTo(report.ReportProgress) switch {
                < 0 => ProgressChange.Increment,
                0 => ProgressChange.None,
                > 0 => ProgressChange.Decrement,
                null => ProgressChange.Increment
            };
            if (report.ReportProgress - Double.Epsilon <= ProgressBoundaries.Start.Value)
                changeType = ProgressChange.Reset;
            else if (report.ReportProgress >= ProgressBoundaries.End.Value - Double.Epsilon)
                changeType = ProgressChange.Completed;
            return changeType;
        }
        
        /// <summary>Routes the <see cref="TProgressReport"/> with the specified <paramref name="progressValue"/> thought the <see cref="HierarchicalProgress{TProgressReport}"/>.</summary>
        /// <param name="progressValue">The progress index within <see cref="HierarchicalProgressBase{TProgressReport}.ProgressBoundaries"/>.</param>
        /// <param name="inner">The report to route.</param>
        /// <returns>The routed report.</returns>
        /// <remarks>Called when a change in TProgressReport is observed in a slice.</remarks>
        protected virtual TProgressReport Route(Index<double> progressValue, TProgressReport inner)
        {
            TProgressReport routed = new() {
                ReportProgress = ReportBoundaries.Map(ProgressBoundaries, progressValue),
                Inner = inner
            };
            return routed;
        }

        private static void ThrowIfCompleted([DoesNotReturnIf(true)] bool isCompleted, ProgressChange reportChange)
        {
            if (isCompleted)
                throw new InvalidProgressStateException("A completed progress provider can no longer receive reports.", reportChange, null);
        }

#endregion
    }
}
