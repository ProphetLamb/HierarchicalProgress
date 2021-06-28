using System;
using System.Runtime.Serialization;

namespace HierarchicalProgress.Exceptions
{
    [Serializable]
    public class InvalidProgressStateException : Exception
    {
        protected InvalidProgressStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue(nameof(ActualState), ActualState, typeof(ProgressChange));
            if (ExpectedState != null)
                info.AddValue(nameof(ExpectedState), ExpectedState.Value, typeof(ProgressChange));
        }

        public InvalidProgressStateException(ProgressChange actualState, ProgressChange? expectedState)
        {
            ActualState = actualState;
            ExpectedState = expectedState;
        }

        public InvalidProgressStateException(string message, ProgressChange actualState, ProgressChange? expectedState)
            : base(message)
        {
            ActualState = actualState;
            ExpectedState = expectedState;
        }

        public InvalidProgressStateException(string message, ProgressChange actualState, ProgressChange? expectedState, Exception innerException)
            : base(message, innerException)
        {
            ActualState = actualState;
            ExpectedState = expectedState;
        }

        public InvalidProgressStateException() : base()
        {
        }

        public InvalidProgressStateException(string message) : base(message)
        {
        }

        public InvalidProgressStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ProgressChange ActualState { get; set; }
        public ProgressChange? ExpectedState { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            ActualState = (ProgressChange)info.GetValue(nameof(ActualState), typeof(ProgressChange))!;
            ExpectedState = (ProgressChange?)info.GetValue(nameof(ExpectedState), typeof(ProgressChange));
        }
    }
}
