using System;

namespace HierarchicalProgress.Events
{
    /// <inheritdoc/>
    public class ProgressResetEventArgs<TProgressValue> : ProgressChangedEventArgs<TProgressValue> where TProgressValue : IProgressReport
    {
        /// <inheritdoc/>
        public ProgressResetEventArgs() { }

        public ProgressResetEventArgs(TProgressValue? previousProgress, TProgressValue? reportedProgress)
            : base(previousProgress, reportedProgress) { }
    }
    }
