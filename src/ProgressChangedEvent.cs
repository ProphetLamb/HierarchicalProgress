using System;

using GenericRange;

namespace HierarchicalProgress
{
    /// <summary>
    ///     Represents increment or decrement of progress of a <see cref="IProgress{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IProgressValue"/> type of the change.</typeparam>
    public class ProgressChangedEventArgs<T> : EventArgs where T : IProgressValue
    {
        /// <summary>
        ///     Initiates a new instance of <see cref="ProgressChangedEventArgs{T}"/> with a specific <paramref name="value"/> and range.
        /// </summary>
        /// <param name="value">The <see cref="IProgressValue"/> of the progress change.</param>
        /// <param name="progressRange"></param>
        public ProgressChangedEventArgs(T value, Range<double> progressRange)
        {
            Value = value;
            ProgressRange = progressRange;
        }
        
        /// <summary>
        ///     The <see cref="IProgressValue"/> of the progress change.
        /// </summary>
        public T Value { get; }
        
        /// <summary>
        ///     The range of the <see cref="IHierarchicalProgress{T}"/> that created the event.
        /// </summary>
        public Range<double> ProgressRange { get; }
    }
}
