using System;

using GenericRange;

namespace HierarchicalProgress
{
    /// <summary>
    ///     Defines a hierarchical provider for progress updates.
    /// </summary>
    /// <typeparam name="T">The <see cref="IProgressValue"/> type of the change.</typeparam>
    public interface IHierarchicalProgress<T> : IProgress<T> where T : IProgressValue
    {
        /// <summary>
        ///     Weak event handler reporting changes to the progress value. 
        /// </summary>
        event EventHandler<ProgressChangedEventArgs<T>> ProgressChanged;

        /// <summary>
        ///     The range associated with this instance of <see cref="IHierarchicalProgress{T}"/> within the <see cref="IProgress{T}"/> can report.
        /// </summary>
        public Range<double> ProgressRange { get; }
        
        /// <summary>
        ///     The maximum value of the top-most progress, used to determine the offset and length of the <see cref="ProgressRange"/>.
        /// </summary>
        /// <remarks>
        ///     Same throughout all <see cref="Slice"/>s of <see cref="IHierarchicalProgress{T}"/>.
        /// </remarks>
        public double MaximumProgress { get; }
        
        /// <summary>
        ///     Creates a <see cref="IHierarchicalProgress{T}"/> representing a slice of the progress of the current instance.
        /// </summary>
        /// <param name="range">The range of values represented by the slice.</param>
        /// <returns>A new instance of <see cref="IHierarchicalProgress{T}"/> representing a slice of the progress of the current instance.</returns>
        IHierarchicalProgress<T> Slice(Range<double> range);
    }
}
