using System;
using System.Collections.Generic;

namespace HierarchicalProgress
{
    partial class HierarchicalProgress<TProgressReport> where TProgressReport : IProgressReport, new()
    {
        internal sealed class Unsubscriber : IDisposable
        {
            private readonly IList<IObserver<TProgressReport>> _observers;
            private readonly IObserver<TProgressReport> _observer;

            internal Unsubscriber(IList<IObserver<TProgressReport>> observers, IObserver<TProgressReport> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
