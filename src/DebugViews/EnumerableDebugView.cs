using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HierarchicalProgress.DebugViews
{
    internal sealed class EnumerableDebugView<T>
    {
        private IEnumerable<T> _sequence; 
        
        public EnumerableDebugView(IEnumerable<T> sequence) {
            _sequence = sequence;
        }
       
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _sequence.ToArray();
    }
}
