using System;
using System.Runtime.Serialization;

namespace HierarchicalProgress.Exceptions
{
    public class InvalidProgressStateException : Exception
    {
        protected InvalidProgressStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue(nameof(ActualState), Enum.GetName(typeof(ProgressChange), ActualState));
            if (ExpectedState != null)
                info.AddValue(nameof(ExpectedState), Enum.GetName(typeof(ProgressChange), ExpectedState));
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

        public ProgressChange ActualState { get; set; }
        public ProgressChange? ExpectedState { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            ActualState = Enum.Parse<ProgressChange>(info.GetString(nameof(ActualState)));
            ExpectedState = TryParseEnum<ProgressChange>(info, nameof(ExpectedState));
        }

        private static TEnum? TryParseEnum<TEnum>(SerializationInfo info, string propertyName) where TEnum : struct
        {
            
            string? expected = null;
            try { expected = info.GetString(nameof(ExpectedState)); }
            catch (Exception)
            { }

            return !String.IsNullOrEmpty(expected) ? Enum.Parse<TEnum>(expected) : null;
        }
    }
}
