#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HierarchicalProgress {
    /*
     * Copyright (c) Jaap Lamfers - jlamfers@com-xs.com
     * Licenced under the [CPOL 1.02](https://www.codeproject.com/info/cpol10.aspx)
     * Source: [Simple effective Weak Event Dispatcher in C#](https://www.codeproject.com/articles/178569/simple-effective-weak-event-dispatcher-in-c)
     * Used as is.
     */

    /// <summary>
    /// Listeners are registered with weak references to their instances. This allows listeners to get garbage collected 
    /// if such listeners are not referenced anywhere else anymore. Invokers are pre compiled and cached.
    /// This class is thread safe.
    /// </summary>
    /// <example>
    ///     public class Entity {
    ///         private readonly WeakEventDispatcher&lt;<see cref="System.EventArgs">System.EventArgs</see>> _changeNotificationDispatcher;
    /// 
    ///         public event EventHandler&lt;<see cref="System.EventArgs">System.EventArgs</see>> DataChanged {
    ///             add { _changeNotificationDispatcher += value; }
    ///             remove { _changeNotificationDispatcher -= value; }
    ///         }
    /// 
    ///         protected virtual void OnDataChanged(System.EventArgs e) {
    ///              if(_changeNotificationDispatcher!= null)
    ///                 _changeNotificationDispatcher.Invoke(this, e);
    ///         }
    ///     }
    /// </example>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    internal class WeakEventDispatcher<TEventArgs>
        where TEventArgs : EventArgs {

        #region Types
        /// <summary>
        /// This interface is a generic handler invoker interface meant to represent a compiled specific handler invoker
        /// </summary>
        interface IHandlerInvoker {
            void Invoke(object instance, object sender, TEventArgs e);
            IHandlerInvoker Init(MethodInfo method);
            string Name { get; }
        }

        /// <summary>
        /// Specific handler invoker (keeping a compiled invoker delegate inside) for any instance type and corresponding method
        /// </summary>
        /// <typeparam name="TInstance">The type of the corresponding instance.</typeparam>
        class HandlerInvoker<TInstance> : IHandlerInvoker {
            private static readonly Dictionary<string, Action<TInstance, object, TEventArgs>>
                _cachedHandlers = new Dictionary<string, Action<TInstance, object, TEventArgs>>();

            private Action<TInstance, object, TEventArgs>
                _compiledHandler;

            #region Implementation of IHandlerInvoker
            public void Invoke(object instance, object sender, TEventArgs e) {
                _compiledHandler.Invoke((TInstance)instance, sender, e);
            }

            public IHandlerInvoker Init(MethodInfo method) {
                Name = method.Name;
                lock (_cachedHandlers) {
                    if (_cachedHandlers.TryGetValue(Name, out _compiledHandler))
                        return this;
                }
                var instance = Expression.Parameter(typeof(TInstance));
                var sender = Expression.Parameter(typeof(object));
                var arg = Expression.Parameter(typeof(TEventArgs));
                var callExpression = Expression.Call(instance, method, sender, arg);
                var exp = Expression.Lambda<Action<TInstance, object, TEventArgs>>(callExpression, instance, sender, arg);
                _compiledHandler = exp.Compile();
                lock (_cachedHandlers) {
                    if (!_cachedHandlers.ContainsKey(Name))
                        _cachedHandlers.Add(Name, _compiledHandler);
                }
                return this;
            }

            public string Name { get; private set; }
            #endregion
        }

        /// <summary>
        /// Bucket for keeping an invoker and the corresponding weak referenced target together
        /// </summary>
        class HandlerBucket {
            private readonly WeakReference _weaklyReferencedTarget;
            private readonly IHandlerInvoker _invoker;

            public HandlerBucket(EventHandler<TEventArgs> handler) {
                _invoker = GetHandlerInvoker(handler.Method);
                _weaklyReferencedTarget = new WeakReference(handler.Target);
            }

            /// <summary>
            /// Invokes the included invoker if it is not garbage collected.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="TEventArgs"/> instance containing the event data.</param>
            public void Invoke(object sender, TEventArgs e) {
                // copy to local variable to prevent race condition
                var target = _weaklyReferencedTarget.Target;
                if (target != null)
                    _invoker.Invoke(target, sender, e);
            }
            public bool IsGarbageCollected {
                get { return _weaklyReferencedTarget.Target == null; }
            }
            public bool RepresentsHandler(EventHandler<TEventArgs> handler) {
                return ReferenceEquals(handler.Target, _weaklyReferencedTarget.Target) && handler.Method.Name.Equals(_invoker.Name);
            }
        }
        #endregion

        #region Fields

        private const int DefaultPurgeExecuteThreshold = 5;

        /// <summary>
        /// Threshold which determines after how many try purge calls a real purge (getting rid of garbage collected instance handlers) must be performed
        /// </summary>
        private readonly int _purgeExecuteThreshold;

        /// <summary>
        /// Counts TryPurge calls
        /// </summary>
        private int _tryPurgeCounter;

        /// <summary>
        /// The list of event handlers which need to be executed on each invoke
        /// </summary>
        private List<HandlerBucket>
            _eventHandlers = new List<HandlerBucket>();

        private readonly object
            _syncLock = new object();

        /// <summary>
        /// Cached invoker constructors 
        /// </summary>
        private static readonly Dictionary<MethodInfo, Func<IHandlerInvoker>>
            _handlerCtors = new Dictionary<MethodInfo, Func<IHandlerInvoker>>();

        /// <summary>
        /// Static event handlers are registered seperately. Such handlers are not bound to an instance en do not need to be purged.
        /// </summary>
        private EventHandler<TEventArgs>
            _staticEventHandlers;


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventDispatcher&lt;TEventArgs&gt;"/> class.
        /// </summary>
        public WeakEventDispatcher()
            : this(DefaultPurgeExecuteThreshold) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventDispatcher&lt;TEventArgs&gt;"/> class.
        /// </summary>
        /// <param name="purgeExecuteThreshold">The purge execute threshold i.e threshold which determines after how many trypurge calls the real purge must be performed.</param>
        public WeakEventDispatcher(int purgeExecuteThreshold) {
            _purgeExecuteThreshold = purgeExecuteThreshold;
            Enabled = true;
        }
        #endregion

        #region Operators += and -=
        /// <summary>
        /// Implements the operator + (and +=). Use this operator to add a handler to the event dispatcher, 
        /// even when the dispatcher is null (as Microsoft allows as well) since the dispatcher
        /// is instantiated if it is null.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static WeakEventDispatcher<TEventArgs> operator +(WeakEventDispatcher<TEventArgs> dispatcher, EventHandler<TEventArgs> handler) {
            if (dispatcher == null)
                dispatcher = new WeakEventDispatcher<TEventArgs>();

            dispatcher.Event += handler;
            return dispatcher;
        }

        /// <summary>
        /// Implements the operator - (and -=) to remove a handler from the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static WeakEventDispatcher<TEventArgs> operator -(WeakEventDispatcher<TEventArgs> dispatcher, EventHandler<TEventArgs> handler) {
            if (dispatcher == null)
                dispatcher = new WeakEventDispatcher<TEventArgs>();
            dispatcher.Event -= handler;
            return dispatcher;
        }
        #endregion

        /// <summary>
        /// Internal event representative.
        /// </summary>
        public event EventHandler<TEventArgs> Event {

            add {
                if (value == null) return;
                if (value.Target == null) {
                    _staticEventHandlers += value;
                }
                else
                    AddHandler(value);
            }
            remove {
                if (value == null) return;
                if (value.Target == null) {
                    _staticEventHandlers -= value;
                }
                else
                    RemoveHandler(value);
            }
        }

        /// <summary>
        /// Invokes all registered handlers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TEventArgs"/> instance containing the event data.</param>
        public void Invoke(object sender, TEventArgs e) {

            TryPurge();

            if (!Enabled)
                return;

            List<HandlerBucket> list;
            lock (_syncLock) {
                list = _eventHandlers.ToList();
            }

            // perform invoke outside critical section by a local reference
            list.ForEach(x => x.Invoke(sender, e)); // best performance (together with _eventHandlers.ToList()), twice as fast than foreach loop

            if (_staticEventHandlers != null)
                _staticEventHandlers.Invoke(sender, e);
        }

        /// <summary>
        /// Cleans up all garbage collected listener handlers.
        /// </summary>
        public void Purge() {
            lock (_syncLock) {
                _tryPurgeCounter = 0;
                _eventHandlers = _eventHandlers.Where(x => !x.IsGarbageCollected).ToList(); // best performance
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WeakEventDispatcher&lt;TEventArgs&gt;"/> is enabled. If it is not enabled then
        /// any invocation will never result into any handler execution.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the number of registered instance handlers. 
        /// Static handlers are not included.
        /// </summary>
        public int InstanceHandlerCount {
            get {
                lock (_eventHandlers)
                    return _eventHandlers.Count;
            }
        }


        /// <summary>
        /// Tries to purge. It performs the purge only if it is needed again as configured by the _purgeExecuteThreshold.
        /// TryPurge() is invoked on each AddHandler and Invoke. On RemoveHandler a purge is performed instantly.
        /// </summary>
        private void TryPurge() {
            if (_tryPurgeCounter++ < _purgeExecuteThreshold) return;
            Purge();
        }

        private void RemoveHandler(EventHandler<TEventArgs> handler) {
            lock (_syncLock) {
                // purged on each remove call
                _tryPurgeCounter = 0;
                _eventHandlers = _eventHandlers.Where(x => !x.IsGarbageCollected && !x.RepresentsHandler(handler)).ToList(); // best performance
            }
        }
        private void AddHandler(EventHandler<TEventArgs> handler) {
            lock (_syncLock) {
                TryPurge();
                _eventHandlers.Add(new HandlerBucket(handler));
            }
        }


        private static IHandlerInvoker GetHandlerInvoker(MethodInfo mi) {
            Func<IHandlerInvoker> ctor;
            lock (_handlerCtors) {
                _handlerCtors.TryGetValue(mi, out ctor);
            }
            if (ctor == null) {
                ctor = CreateHandlerInvokerConstructor(typeof(HandlerInvoker<>).MakeGenericType(typeof(TEventArgs), mi.DeclaringType));
                lock (_handlerCtors) {
                    _handlerCtors[mi] = ctor;
                }
            }
            return ctor().Init(mi);
        }

        private static Func<IHandlerInvoker> CreateHandlerInvokerConstructor(Type type) {
            var ctorInfo = type.GetConstructor(Type.EmptyTypes);
            var ctor = Expression.Convert(Expression.New(ctorInfo), typeof(IHandlerInvoker));
            var lambda = Expression.Lambda<Func<IHandlerInvoker>>(ctor);
            return lambda.Compile();
        }
    }

}
#nullable enable
