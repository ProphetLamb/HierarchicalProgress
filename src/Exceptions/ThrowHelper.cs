using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using GenericRange;

namespace HierarchicalProgress.Exceptions
{
    internal static class ThrowHelper
    {
        private static readonly Dictionary<ExceptionArgument, string> _argumentNameMap = new();

        public static void ThrowIndexOutOfRangeException_IfNotContained<T>(Range<T> range, Index<T> index)
            where T : unmanaged, IComparable
        {
            if (!range.Contains(index))
                ThrowIndexOutOfRangeException(range, index);
        }

        public static void ThrowIndexOutOfRangeException_IfNotContained<T>(Range<T> range, Index<T> index, T length)
            where T : unmanaged, IComparable
        {
            if (!range.Contains(index, length))
                ThrowIndexOutOfRangeException(range, index);
        }

        [DoesNotReturn]
        public static void ThrowIndexOutOfRangeException<T>(Range<T> range, Index<T> index)
            where T : unmanaged, IComparable
        {
            throw new IndexOutOfRangeException($"The index {index} is outside of the range {range}.");
        }

        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException_InsufficientFreeProgress(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), "Insufficient progressValue to allocate.");
        }

        [DoesNotReturn]
        public static void ThrowNotSupportedException_OnError(Exception error)
        {
            throw new NotSupportedException("OnError is not supported.", error);
        }

        private static string GetArgumentName(ExceptionArgument argument)
        {
            if (_argumentNameMap.TryGetValue(argument, out string name))
                return name;
            name = Enum.GetName(typeof(ExceptionArgument), argument);
            _argumentNameMap.Add(argument, name);
            return name;
        }
    }

    internal enum ExceptionArgument
   {
        allocateProgress
    }
}
