using Ae.Engine.Menu;
using Ae.Engine.Types;
using NTDLS.Semaphore;
using System.Collections.Generic;
using static Ae.Engine.Types.AeDefermentEvent;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    /// <summary>
    /// Provides control and management for deferred event execution within the game engine, allowing events to be
    /// scheduled, triggered, and deleted based on timing and conditions.
    /// </summary>
    /// <remarks>EventTickController enables flexible scheduling of one-time or recurring events, supporting
    /// both synchronous and asynchronous execution models. Events can be added with custom callbacks and parameters,
    /// and are managed in a thread-safe collection. This controller is typically used to coordinate game logic that
    /// depends on timed actions or state changes. Thread safety is ensured for event collection operations. Use the
    /// provided factory methods to schedule events and manage their lifecycle.</remarks>
    public class EventTickController
        : UnvectoredTickControllerBase<AeDefermentEvent>
    {
        private readonly PessimisticCriticalResource<List<AeDefermentEvent>> _collection = new();

        /// <summary>
        /// Delegate for the event execution callback.
        /// </summary>
        /// <typeparam name="T">Type of the parameter for the event.</typeparam>
        /// <param name="parameter">An object passed by the user code</param>
        public delegate void SiDefermentSimpleExecuteCallbackT<T>(T parameter);

        /// <summary>
        /// Initializes a new instance of the EventTickController class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to coordinate event ticks. Cannot be null.</param>
        public EventTickController(AeEngine engine)
            : base(engine)
        {
        }

        /// <summary>
        /// Processes scheduled engine events for the current world clock tick, triggering events that are not queued
        /// for deletion.
        /// </summary>
        /// <remarks>This method iterates through the collection of engine events and checks each event
        /// for activation if it is not marked for deletion. It is typically called once per world clock tick to ensure
        /// timely event processing.</remarks>
        public override void ExecuteWorldClockTick()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    var engineEvent = o[i];
                    if (engineEvent.IsQueuedForDeletion == false)
                    {
                        engineEvent.CheckForTrigger();
                    }
                }
            });
        }

        /// <summary>
        /// We fire this event when the game is won.
        /// </summary>
        public void QueueTheDoorIsAjar()
        {
            Add(4, (sender, parameter) =>
            {
                Engine.Audio.DoorIsAjarSound?.Play();
                Engine.Menus.Show(new AeMenuStartNewGame(Engine));
            });
        }

        #region Factories.

        /// <summary>
        /// Creates a new deferment event that executes a callback once after a specified delay.
        /// </summary>
        /// <remarks>Use this method to schedule a single execution of a callback after a delay. The event
        /// is added to the internal collection and will be triggered according to the specified threading
        /// model.</remarks>
        /// <param name="delayMs">The delay, in milliseconds, before the callback is executed. Must be non-negative.</param>
        /// <param name="executionCallback">The callback to execute when the deferment event triggers. Receives the event instance and an optional
        /// reference object.</param>
        /// <param name="threadModel">Specifies the threading model to use for event execution. Defaults to Synchronous if not specified.</param>
        /// <returns>An instance of AeDefermentEvent representing the scheduled one-time execution.</returns>
        public AeDefermentEvent Once(int delayMs, SiDefermentExecuteCallback executionCallback,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(delayMs, (AeDefermentEvent sender, object? refObj) => executionCallback(sender, refObj));
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Schedules a single deferred event to execute the specified callback after a delay.
        /// </summary>
        /// <remarks>The event will execute only once after the specified delay. Use the returned
        /// AeDefermentEvent to manage or cancel the scheduled event as needed.</remarks>
        /// <param name="delayMs">The delay, in milliseconds, before the callback is executed. Must be non-negative.</param>
        /// <param name="executionCallback">The callback to execute when the deferred event is triggered.</param>
        /// <param name="threadModel">Specifies the threading model to use for event execution. Defaults to Synchronous.</param>
        /// <returns>An instance of AeDefermentEvent representing the scheduled deferred event.</returns>
        public AeDefermentEvent Once(int delayMs, SiDefermentSimpleExecuteCallback executionCallback,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(delayMs, (AeDefermentEvent sender, object? refObj) => executionCallback());
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new event. This can be a recurring event, single event, synchronous, asynchronous and can be passed parameters.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <param name="threadModel">Whether the event callback is run synchronous or asynchronous.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback, object? parameter = null,
            SiDefermentEventMode eventMode = SiDefermentEventMode.OneTime,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds, parameter, executionCallback, eventMode, threadModel);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new event. This can be a recurring event, single event, synchronous or asynchronous.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <param name="threadModel">Whether the event callback is run synchronous or asynchronous.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback,
            SiDefermentEventMode eventMode = SiDefermentEventMode.OneTime,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds, null, executionCallback, eventMode, threadModel);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event. This can be a recurring event, single event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback,
            SiDefermentEventMode eventMode = SiDefermentEventMode.OneTime)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds, null, executionCallback, eventMode);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(int timeoutMilliseconds, object parameter, SiDefermentExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds, parameter, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event and passes a parameter of the given type T.
        /// </summary>
        /// <typeparam name="T">Type of the parameter for the event.</typeparam>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="parameter">An object passed by the user code</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public AeDefermentEvent Add<T>(int timeoutMilliseconds, T parameter, SiDefermentSimpleExecuteCallbackT<T> executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds,
                    (AeDefermentEvent sender, object? refObj) =>
                {
                    executionCallback(parameter);
                });
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded, single-fire event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(int timeoutMilliseconds, SiDefermentSimpleExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(timeoutMilliseconds, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded, single-fire event.
        /// </summary>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public AeDefermentEvent Add(SiDefermentSimpleExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(0, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Adds an existing even to the collection.
        /// </summary>
        public AeDefermentEvent Add(AeDefermentEvent obj)
        {
            return _collection.Use(o =>
            {
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Deletes an event from the collection.
        /// </summary>
        /// <param name="obj"></param>
        public void HardDelete(AeDefermentEvent obj)
        {
            _collection.Use(o =>
            {
                o.Remove(obj);
            });
        }

        /// <summary>
        /// Queues an event for deletion from the collection.
        /// </summary>
        public void CleanupQueuedForDeletion()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    if (o[i].IsQueuedForDeletion)
                    {
                        o.Remove(o[i]);
                    }
                }
            });
        }

        #endregion
    }
}
