using System.Collections.Generic;

namespace Ae.Engine.AI
{
    public class AIParameterCollection
    {
        private Dictionary<string, object> _parameters = new();

        /// <summary>
        /// Overrides the internal parameters with the values from another collection and adds any new parameters from the other collection.
        /// </summary>
        /// <param name="other"></param>
        public void Merge(AIParameterCollection other)
        {
            foreach (var kvp in other._parameters)
            {
                _parameters[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Overrides the internal parameters with the values from another collection and adds any new parameters from the other collection.
        /// </summary>
        /// <param name="other"></param>
        public void Merge(Dictionary<string, object> other)
        {
            foreach (var kvp in other)
            {
                _parameters[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Sets a value in the collection. If the key already exists, it will be overridden with the new value. If the key does not exist, it will be added to the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            _parameters[key] = value;
        }

        public T Get<T>(string key)
        {
            if (_parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            throw new KeyNotFoundException($"Parameter with key '{key}' not found or is not of type {typeof(T).Name}.");
        }

        public T? Get<T>(string key, T? defaultValue = default)
        {
            if (_parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
    }
}
