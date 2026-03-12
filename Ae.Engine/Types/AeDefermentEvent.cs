using System;
using System.Threading.Tasks;

namespace Ae.Engine.Types
{
    /// <summary>
    /// Allows for deferred events to be injected into the engine. We use this so that we can defer 
    /// tasks without sleeping and so we can inject into the sprites during the world clock logic.
    /// </summary>
    public class AeDefermentEvent
    {
        /// <summary>
        /// Gets or sets the name associated with the current instance.
        /// </summary>
        public string? Name { get; set; }
        private readonly object? _parameter = null;
        private readonly int _timeoutMilliseconds;
        private readonly SiDefermentExecuteCallback? _executionCallback = null;
        private readonly SiDefermentSimpleExecuteCallback? _simpleExecutionCallback = null;
        private readonly SiDefermentEventMode _eventMode = SiDefermentEventMode.OneTime;
        private readonly SiDefermentEventThreadModel _threadModel;
        private DateTime _eventTriggerBaseTime;

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid UID { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the item is scheduled to be deleted.
        /// </summary>
        public bool IsQueuedForDeletion { get; private set; } = false;

        /// <summary>
        /// Delegate for the event execution callback.
        /// </summary>
        /// <param name="sender">The event that is being triggered</param>
        /// <param name="parameter">An optional object passed by the user code</param>
        public delegate void SiDefermentExecuteCallback(AeDefermentEvent sender, object? parameter);

        /// <summary>
        /// Delegate for the event execution callback.
        /// </summary>
        public delegate void SiDefermentSimpleExecuteCallback();

        /// <summary>
        /// Specifies the mode for deferment events, indicating whether the event occurs one time or on a recurring
        /// basis.
        /// </summary>
        /// <remarks>Use this enumeration to distinguish between single-occurrence and repeated deferment
        /// event scheduling. The value affects how the event is processed and managed within the system.</remarks>
        public enum SiDefermentEventMode
        {
            /// <summary>
            /// Specifies that the associated operation or event occurs only once.
            /// </summary>
            OneTime,
            /// <summary>
            /// Specifies that the associated operation or event recurs.
            /// </summary>
            Recurring
        }

        /// <summary>
        /// Specifies the threading model used for deferment event processing.
        /// </summary>
        /// <remarks>Use this enumeration to indicate whether deferment events are handled synchronously
        /// or asynchronously. Selecting the appropriate threading model can affect responsiveness and concurrency in
        /// event handling scenarios.</remarks>
        public enum SiDefermentEventThreadModel
        {
            /// <summary>
            /// Gets or sets a value indicating whether operations are performed synchronously.
            /// </summary>
            Synchronous,
            /// <summary>
            /// Gets or sets a value indicating whether operations are performed asynchronously.
            /// </summary>
            Asynchronous
        }

        /// <summary>
        /// Creates a new event. This can be a recurring event, single event, synchronous, asynchronous and can be passed parameters.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <param name="threadModel">Whether the event callback is run synchronous or asynchronous.</param>
        public AeDefermentEvent(int timeoutMilliseconds, object? parameter, SiDefermentExecuteCallback executionCallback,
            SiDefermentEventMode eventMode = SiDefermentEventMode.OneTime,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            _parameter = parameter;
            _timeoutMilliseconds = timeoutMilliseconds;
            _executionCallback = executionCallback;
            _eventMode = eventMode;
            _threadModel = threadModel;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new one-time synchronous event that is passed a parameter.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        public AeDefermentEvent(int timeoutMilliseconds, object parameter, SiDefermentExecuteCallback executionCallback)
        {
            _parameter = parameter;
            _timeoutMilliseconds = timeoutMilliseconds;
            _executionCallback = executionCallback;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new one-time no-parameter synchronous event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        public AeDefermentEvent(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback)
        {
            _timeoutMilliseconds = timeoutMilliseconds;
            _executionCallback = executionCallback;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the AeDefermentEvent class with the specified timeout and execution callback.
        /// </summary>
        /// <param name="timeoutMilliseconds">The maximum duration, in milliseconds, to wait before the deferment event is triggered. Must be a
        /// non-negative value.</param>
        /// <param name="simpleExecutionCallback">The callback to execute when the deferment event is triggered. Cannot be null.</param>
        public AeDefermentEvent(int timeoutMilliseconds, SiDefermentSimpleExecuteCallback simpleExecutionCallback)
        {
            _timeoutMilliseconds = timeoutMilliseconds;
            _simpleExecutionCallback = simpleExecutionCallback;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Marks the current object as queued for deletion.
        /// </summary>
        public void QueueForDeletion()
        {
            IsQueuedForDeletion = true;
        }

        /// <summary>
        /// Checks whether the event trigger condition has been met and executes the associated callbacks if triggered.
        /// </summary>
        /// <remarks>This method is thread-safe and may queue the event for deletion if the event mode is
        /// one-time. For asynchronous thread models, callbacks are executed on a background thread. For recurring
        /// events, the trigger base time is reset after execution.</remarks>
        /// <returns>A value indicating whether the trigger condition was met and the callbacks were executed. Returns <see
        /// langword="true"/> if the event was triggered; otherwise, <see langword="false"/>.</returns>
        public bool CheckForTrigger()
        {
            lock (this)
            {
                bool result = false;

                if (IsQueuedForDeletion)
                {
                    return false;
                }

                if ((DateTime.UtcNow - _eventTriggerBaseTime).TotalMilliseconds >= _timeoutMilliseconds)
                {
                    result = true;

                    if (_eventMode == SiDefermentEventMode.OneTime)
                    {
                        IsQueuedForDeletion = true;
                    }

                    if (_threadModel == SiDefermentEventThreadModel.Asynchronous)
                    {
                        Task.Run(() =>
                        {
                            _executionCallback?.Invoke(this, _parameter);
                            _simpleExecutionCallback?.Invoke();
                        });
                    }
                    else
                    {
                        _executionCallback?.Invoke(this, _parameter);
                        _simpleExecutionCallback?.Invoke();
                    }

                    if (_eventMode == SiDefermentEventMode.Recurring)
                    {
                        _eventTriggerBaseTime = DateTime.UtcNow;
                    }
                }
                return result;
            }
        }
    }
}
