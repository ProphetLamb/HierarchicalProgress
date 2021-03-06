using System;
using System.Diagnostics.CodeAnalysis;

using GenericRange;
using GenericRange.Extensions;
using HierarchicalProgress.Exceptions;

namespace HierarchicalProgress
{
    public partial class HierarchicalProgress<TProgressReport> where TProgressReport : IProgressReport, new()
    {
        internal sealed class SliceObserver : IObserver<TProgressReport>
        {
            private readonly WeakReference<HierarchicalProgress<TProgressReport>> _route;
            private readonly WeakReference<HierarchicalProgress<TProgressReport>> _slice;

            public SliceObserver(HierarchicalProgress<TProgressReport> route, HierarchicalProgress<TProgressReport> slice)
            {
                _route = new WeakReference<HierarchicalProgress<TProgressReport>>(route);
                _slice = new WeakReference<HierarchicalProgress<TProgressReport>>(slice);
            }

            public IDisposable? Unsubscriber { get; set; }

            public void OnCompleted()
            {
                // Once the progress is completed the observer is no longer needed
                Unsubscribe();
            }

            public void OnError(Exception error)
            {
                ThrowHelper.ThrowNotSupportedException_OnError(error);
            }

            public void OnNext(TProgressReport value)
            {
                if (TryGetOrUnsubscribe(out var slice, out var route))
                {
                    decimal sliceProgress = slice.ReportBoundaries.Map(slice.ProgressBoundaries, value.ReportProgress);

                    ThrowHelper.ThrowIndexOutOfRangeException_IfNotContained(slice.ProgressBoundaries, sliceProgress);

                    lock (route!._sliceObserverSyncLock)
                    {
                        Index<decimal> progress = route.Progress.Value + slice.LatestChange.Delta;
                        TProgressReport routed = route.Route(progress, value);
                        route.Report(routed);
                    }
                }
            }

            private bool TryGetOrUnsubscribe([NotNullWhen(true)] out HierarchicalProgress<TProgressReport>? slice, [NotNullWhen(true)] out HierarchicalProgress<TProgressReport>? route)
            {
                if (Unsubscriber != null)
                {
                    if (_slice.TryGetTarget(out slice) && _route.TryGetTarget(out route))
                    {
                        return true;
                    }

                    Unsubscribe();

                    slice = null;
                    route = null;
                    return false;
                }

                slice = null;
                route = null;
                return false;
            }

            private void Unsubscribe()
            {
                if (Unsubscriber == null)
                    return;
                IDisposable unsubscriber = Unsubscriber!;
                Unsubscriber = null;
                unsubscriber.Dispose();
            }
        }
    }
}
