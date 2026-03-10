using Ae.Engine.Menu;
using Ae.Engine.Types;
using NTDLS.Semaphore;
using System.Collections.Generic;
using static Ae.Engine.Types.AeDefermentEvent;

namespace Ae.Engine.TickController.UnvectoredTickController
{
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

        public EventTickController(AeEngine engine)
            : base(engine)
        {
        }

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
                Engine.Menus.Show(new MenuStartNewGame(Engine));
            });
        }

        #region Factories.

        public AeDefermentEvent Once(SiDefermentSimpleExecuteCallback executionCallback,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(0,
                    (AeDefermentEvent sender, object? refObj) =>
                    {
                        executionCallback();
                    });
                o.Add(obj);
                return obj;
            });
        }

        public AeDefermentEvent Once<T>(SiDefermentSimpleExecuteCallbackT<T> executionCallback, T parameter,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(0,
                    (AeDefermentEvent sender, object? refObj) =>
                    {
                        executionCallback(parameter);
                    });
                o.Add(obj);
                return obj;
            });
        }

        public AeDefermentEvent Once(SiDefermentExecuteCallback executionCallback, object? parameter = null,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new AeDefermentEvent(0, parameter, executionCallback, SiDefermentEventMode.OneTime, threadModel);
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
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
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
        /// <param name="SiDefermentEvent">An existing event to add.</param>
        /// <returns></returns>
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
