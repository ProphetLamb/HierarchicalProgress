using System;

namespace HierarchicalProgress.Events
{
    /// <inheritdoc/>
    public class ProgressCompletedEventArgs<TProgressValue> : ProgressChangedEventArgs<TProgressValue> where TProgressValue : IProgressReport
    {
        /// <inheritdoc/>
        public ProgressCompletedEventArgs() { }

        public ProgressCompletedEventArgs(TProgressValue? previousProgress, TProgressValue? reportedProgress)
            : base(previousProgress, reportedProgress) { }
    }
}
