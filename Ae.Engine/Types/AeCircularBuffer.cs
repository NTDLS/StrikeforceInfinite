using System;
using System.Collections.Generic;

namespace Ae.Engine.Types
{
    /// <summary>
    /// Represents a fixed-size, generic circular buffer that stores elements in FIFO order. When the buffer reaches its
    /// capacity, adding a new element removes the oldest element.
    /// </summary>
    /// <remarks>The buffer maintains a maximum number of elements specified by the capacity. When a new
    /// element is added and the buffer is full, the oldest element is automatically removed to make room for the new
    /// one. This class is useful for scenarios where only the most recent items are needed, such as logging or
    /// streaming data. The buffer is not thread-safe.</remarks>
    /// <typeparam name="T">The type of elements stored in the buffer.</typeparam>
    public class AeCircularBuffer<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _capacity;

        /// <summary>
        /// Initializes a new instance of the AeCircularBuffer class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum number of elements that the buffer can hold. Must be greater than 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if capacity is less than or equal to 0.</exception>
        public AeCircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("capacity", "Capacity must be greater than 0.");

            _capacity = capacity;
            _queue = new Queue<T>(capacity);
        }

        /// <summary>
        /// Adds an item to the queue, removing the oldest item if the queue has reached its capacity.
        /// </summary>
        /// <remarks>If the queue is full, the oldest item is automatically removed to make room for the
        /// new item. This ensures the queue always contains at most the specified capacity of items.</remarks>
        /// <param name="item">The item to add to the queue.</param>
        public void Push(T item)
        {
            if (_queue.Count == _capacity)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the queue.
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// Gets the collection of items currently contained in the queue.
        /// </summary>
        public IEnumerable<T> Items => _queue;
    }
}
