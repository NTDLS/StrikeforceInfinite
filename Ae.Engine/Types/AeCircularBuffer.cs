using System;
using System.Collections.Generic;

namespace Ae.Engine.Types
{
    public class AeCircularBuffer<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _capacity;

        public AeCircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("capacity", "Capacity must be greater than 0.");

            _capacity = capacity;
            _queue = new Queue<T>(capacity);
        }

        public void Push(T item)
        {
            if (_queue.Count == _capacity)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(item);
        }

        public int Count => _queue.Count;

        public IEnumerable<T> Items => _queue;
    }
}
