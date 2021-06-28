using System;
using System.Reflection.Metadata;

namespace HierarchicalProgress.Tests
{
    public class DelegateSubscriber<T> : IObserver<T>
    {
        public Action CompletedHandler;
        public Action<Exception> ErrorHandler;
        public Action<T> NextHandler;

        public DelegateSubscriber(Action completedHandler, Action<Exception> errorHandler, Action<T> nextHandler)
        {
            CompletedHandler = completedHandler;
            ErrorHandler = errorHandler;
            NextHandler = nextHandler;
        }

        public void OnCompleted()
        {
            CompletedHandler();
        }

        public void OnError(Exception error)
        {
            ErrorHandler(error);
        }

        public void OnNext(T value)
        {
            NextHandler(value);
        }
    }
}
