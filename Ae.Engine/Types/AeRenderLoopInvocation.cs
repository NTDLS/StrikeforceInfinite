using System;
using System.Threading;

namespace Ae.Engine.Types
{
    /// <summary>
    /// Represents a single invocation of a render loop action within the engine, providing synchronization and tracking
    /// for execution and completion.
    /// </summary>
    /// <remarks>Each instance encapsulates an action to be executed in the render loop, along with an
    /// associated engine and a unique identifier. The invocation can be waited upon for completion using the provided
    /// synchronization event. This class is typically used to coordinate render loop tasks and ensure they are executed
    /// and tracked reliably.</remarks>
    public class AeRenderLoopInvocation
    {
        /// <summary>
        /// Provides an auto-reset event used for signaling between threads.
        /// </summary>
        /// <remarks>The event is initialized in the non-signaled state. When signaled, it automatically
        /// resets after releasing a single waiting thread. Use this event to coordinate thread execution or implement
        /// synchronization scenarios where only one waiting thread should proceed per signal.</remarks>
        public AutoResetEvent Event = new(false);

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the delegate that defines the action to be performed.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Gets or sets the engine instance used for executing operations within the application.
        /// </summary>
        public AeEngine Engine { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeRenderLoopInvocation class with the specified engine and action to be
        /// invoked during the render loop.
        /// </summary>
        /// <param name="engine">The engine instance that will be associated with this render loop invocation. Cannot be null.</param>
        /// <param name="action">The action to execute during the render loop. Cannot be null.</param>
        public AeRenderLoopInvocation(AeEngine engine, Action action)
        {
            Id = Guid.NewGuid();
            Engine = engine;
            Action = action;
        }

        /// <summary>
        /// Executes the associated action and signals completion of the operation.
        /// </summary>
        /// <remarks>This method triggers the action, sets the completion event, and removes the
        /// invocation from the render loop. It is typically used to finalize asynchronous or scheduled operations
        /// within the rendering engine. Calling this method more than once may result in undefined behavior.</remarks>
        public void Execute()
        {
            Action();
            Event.Set();
            Engine.RemoveRenderLoopInvocation(this);
        }

        /// <summary>
        /// Blocks the calling thread until the associated event is signaled.
        /// </summary>
        /// <remarks>This method causes the current thread to wait indefinitely until the event is set.
        /// Use this method when synchronization between threads is required. If the event is never signaled, the thread
        /// will remain blocked.</remarks>
        public void Wait()
        {
            Event.WaitOne();
        }
    }
}
