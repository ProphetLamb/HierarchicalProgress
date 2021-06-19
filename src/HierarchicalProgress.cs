using System;
using System.Diagnostics;

using GenericRange;

namespace HierarchicalProgress
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public class HierarchicalProgress<T> : IHierarchicalProgress<T> where T : IProgressValue
    {
#region Ctor

        public HierarchicalProgress(double maximumProgress)
            : this(new Range<double>(0, 0, true), maximumProgress) { }

        public HierarchicalProgress(in Range<double> progressRange, double maximumProgress)
        {
            if (progressRange.Start.GetOffset(maximumProgress) < 0)
                throw new ArgumentOutOfRangeException(nameof(progressRange), "Start value must be greater then or equal to zero.");
            if (progressRange.End.GetOffset(maximumProgress) > maximumProgress)
                throw new ArgumentOutOfRangeException(nameof(progressRange), "End value must be less then or equal to " + nameof(MaximumProgress));
            
            ProgressRange = progressRange;
            MaximumProgress = maximumProgress;
        }

#endregion
        
#region Fields
        
        private WeakEventDispatcher<ProgressChangedEventArgs<T>> _dispatcher = new();
        private T? _current;

#endregion

#region Properties

        public event EventHandler<ProgressChangedEventArgs<T>> ProgressChanged
        {
            add => _dispatcher += value;
            remove => _dispatcher -= value;
        }
        
        public Range<double> ProgressRange { get; }

        public double MaximumProgress { get; }
        
        /// <summary>
        ///     The latest <see cref="IProgress{T}.Report"/>ed value.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <see langword="null"/>.</exception>
        public virtual T? Current
        {
            get => _current;
            protected set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _current = value;
                OnProgressChanged(value);
            }
        }

        public bool ThrowOnSliceOverMaximum { get; set; } = true;

#endregion

#region Public members

        public virtual void Report(T value)
        {
            Range<double> virtualRange = GetVirtualRange();
            if (virtualRange.ContainsEndInclusive(value.Progress))
                throw new ArgumentOutOfRangeException(nameof(value), "Progress must be within the range " + virtualRange);
            Current = value;
        }

        public virtual IHierarchicalProgress<T> Slice(Range<double> range)
        {
            if (!ProgressRange.Encompasses(range, MaximumProgress))
                throw new ArgumentOutOfRangeException(nameof(range), "Value must be encompassed by the range " + ProgressRange);
            
            HierarchicalProgress<T> progress = new(range, MaximumProgress);
            progress.ProgressChanged += SlicedProgressChanged;
            return progress;
        }

        public override string ToString() => $"{nameof(HierarchicalProgress<T>)}: {{ {GetDebuggerDisplay()} }}";
        
#endregion

#region Internal members

        protected virtual void SlicedProgressChanged(object? sender, ProgressChangedEventArgs<T> args)
        {
            T sliceProgress = (T)args.Value.Clone();
            sliceProgress.Progress = EnsureProgressChangedArgs(args);
            Current = sliceProgress;
        }

        protected virtual void OnProgressChanged(T value) => _dispatcher.Invoke(this, new ProgressChangedEventArgs<T>(value, ProgressRange));

        private string GetDebuggerDisplay() => $"{nameof(ProgressRange)}: {ProgressRange}, {nameof(MaximumProgress)}: {MaximumProgress}, {nameof(Current)}: {Current}";

        private double EnsureProgressChangedArgs(ProgressChangedEventArgs<T> args)
        {
            double progress = args.ProgressRange.GetOffsetAndLength(MaximumProgress).Offset + args.Value.Progress;
            Range<double> virtualRange = GetVirtualRange();
            if (virtualRange.ContainsEndInclusive(progress))
                return progress;
            if (ThrowOnSliceOverMaximum)
                throw new ArgumentOutOfRangeException(nameof(args), "Value is not within the progress range " + virtualRange);
            return ProgressRange.GetOffsetAndLength(MaximumProgress).Length;
        }

        private Range<double> GetVirtualRange() => new(0, ProgressRange.GetOffsetAndLength(MaximumProgress).Length);

#endregion
    }
}
