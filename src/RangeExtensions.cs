using System;

using GenericRange;

namespace HierarchicalProgress
{
    internal static class RangeExtensions
    {
        public static bool ContainsEndInclusive<T>(this Range<T> range, T value) where T : unmanaged, IComparable
        {
            return range.Start.Value.CompareTo(value) <= 0 && range.End.Value.CompareTo(value) >= 0;
        }
    }
}
