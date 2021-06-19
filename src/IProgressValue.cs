using System;

namespace HierarchicalProgress
{
    /// <summary>
    ///     Transport interface for a <see cref="IProgress{T}"/> with a 64-bit floating-point <see cref="Progress"/> value.
    /// </summary>
    public interface IProgressValue : ICloneable
    {
        /// <summary>
        ///     The <see langword="double"/> indicating the progress expressed by the <see cref="IProgressValue"/>.
        /// </summary>
        public double Progress { get; set; }
    }
}
