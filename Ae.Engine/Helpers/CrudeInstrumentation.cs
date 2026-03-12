#define EnableCrudeInstrumentation

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ae.Engine.Helpers
{
    /// <summary>
    /// Provides basic instrumentation utilities for measuring and recording execution metrics in code, such as
    /// operation counts and elapsed time.
    /// </summary>
    /// <remarks>The class exposes static methods and metrics for tracking performance data. Instrumentation
    /// can be enabled or disabled at compile time using the EnableCrudeInstrumentation directive. When enabled, metrics
    /// are collected and stored in the Metrics property, allowing analysis of operation durations and invocation
    /// counts. This class is intended for lightweight, ad-hoc instrumentation scenarios and is not thread-safe for
    /// concurrent metric access outside of the provided methods.</remarks>
    public static class CrudeInstrumentation
    {
        /// <summary>
        /// Gets the global instrumentation metrics instance used to track application performance and diagnostic data.
        /// </summary>
        /// <remarks>This property provides access to metrics collected throughout the application's
        /// lifetime. The instance is initialized once and shared across the application. Thread-safe access is ensured
        /// by the static nature of the property.</remarks>
        public static InstrumentationMetrics Metrics { get; private set; } = new();

        /// <summary>
        /// Represents a collection of instrumentation metrics used for monitoring and analysis purposes.
        /// </summary>
        /// <remarks>The metrics are stored in a dictionary, allowing efficient access by metric name. The
        /// class also provides an ordered view of the metrics based on their recorded duration, which can be useful for
        /// identifying the most time-consuming operations.</remarks>
        public class InstrumentationMetrics
        {
            /// <summary>
            /// Represents the collection of instrumentation metrics indexed by their names.
            /// </summary>
            /// <remarks>Use this dictionary to access or modify metrics by their string identifiers.
            /// Changes to the collection affect the set of available metrics for instrumentation purposes.</remarks>
            public Dictionary<string, InstrumentationMetric> Collection = new();

            /// <summary>
            /// Gets a list of instrumentation metrics ordered by descending duration in milliseconds.
            /// QuickWatch: Ae.Library.CrudeInstrumentation.Metrics.Ordered
            /// </summary>
            /// <remarks>Use this property to retrieve metrics sorted from longest to shortest
            /// duration. The returned list is a snapshot and does not reflect subsequent changes to the underlying
            /// collection.</remarks>
            public List<KeyValuePair<string, InstrumentationMetric>> Ordered
                => Collection.OrderByDescending(static o => o.Value.Milliseconds).ToList();
        }

        /// <summary>
        /// Represents a metric used for instrumentation, containing a count and a duration in milliseconds.
        /// </summary>
        /// <remarks>Use this class to track the number of occurrences and the total time spent for a
        /// specific operation or event. This is commonly used in performance monitoring and logging
        /// scenarios.</remarks>
        public class InstrumentationMetric
        {
            /// <summary>
            /// Gets or sets the total number of items in the collection.
            /// </summary>
            public ulong Count { get; set; }

            /// <summary>
            /// Gets or sets the duration, in milliseconds.
            /// </summary>
            public double Milliseconds { get; set; }
        }

        /// <summary>
        /// Represents a callback method used for crude instrumentation purposes.
        /// </summary>
        /// <remarks>Use this delegate to provide custom instrumentation logic, such as logging or
        /// performance tracking, at specific points in an application. The delegate does not accept parameters and does
        /// not return a value.</remarks>
        public delegate void CrudeInstrumentationProc();

        /// <summary>
        /// Represents a procedure that performs instrumentation and returns a value of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the instrumentation procedure.</typeparam>
        /// <returns>The value produced by the instrumentation procedure.</returns>
        public delegate T CrudeInstrumentationProc<T>();

        /// <summary>
        /// Represents a delegate that defines a procedure returning a nullable value of type T.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the procedure. Can be any value or reference type.</typeparam>
        /// <returns>A nullable value of type T produced by the procedure, or null if no value is returned.</returns>
        public delegate T? CrudeInstrumentationNullableProc<T>();

        private class MetricsTextItem
        {
            public string Milliseconds { get; set; } = string.Empty;
            public string Average { get; set; } = string.Empty;
            public string Count { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Records instrumentation metrics for the specified operation, associating them with a unique key.
        /// </summary>
        /// <remarks>This method measures the execution time of the provided operation and updates the
        /// associated metrics. Metrics are aggregated by the specified key, allowing performance tracking across
        /// multiple invocations. Thread safety is ensured when updating metrics.</remarks>
        /// <param name="key">The unique identifier used to group instrumentation metrics for the operation. Cannot be null or empty.</param>
        /// <param name="proc">The delegate representing the operation to be instrumented. Cannot be null.</param>
#if !EnableCrudeInstrumentation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Witness(CrudeInstrumentationProc proc) => proc();
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Witness(string key, CrudeInstrumentationProc proc)
        {
            var sw = Stopwatch.StartNew();
            proc();
            sw.Stop();

            lock (Metrics)
            {
                if (Metrics.Collection.TryGetValue(key, out var metrics))
                {
                    metrics.Count++;
                    metrics.Milliseconds += sw.ElapsedMilliseconds;
                }
                else
                {
                    metrics = new InstrumentationMetric()
                    {
                        Count = 1,
                        Milliseconds = sw.ElapsedMilliseconds,
                    };
                    Metrics.Collection.Add(key, metrics);
                }
            }
        }
#endif

        /// <summary>
        /// Executes the specified procedure and records instrumentation metrics for the operation.
        /// </summary>
        /// <remarks>Instrumentation metrics, including execution count and elapsed milliseconds, are
        /// aggregated under the specified key. This method is thread-safe and can be used to monitor performance of
        /// operations across multiple calls.</remarks>
        /// <typeparam name="T">The type of the value returned by the procedure.</typeparam>
        /// <param name="key">The unique key used to identify and aggregate instrumentation metrics for this operation.</param>
        /// <param name="proc">The procedure to execute. The procedure is invoked and its execution time is measured.</param>
        /// <returns>The result returned by the executed procedure.</returns>
#if !EnableCrudeInstrumentation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Witness<T>(CrudeInstrumentationProc<T> proc) => proc();
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Witness<T>(string key, CrudeInstrumentationProc<T> proc)
        {
            var sw = Stopwatch.StartNew();
            T result = proc();
            sw.Stop();

            lock (Metrics)
            {
                if (Metrics.Collection.TryGetValue(key, out var metrics))
                {
                    metrics.Count++;
                    metrics.Milliseconds += sw.ElapsedMilliseconds;
                }
                else
                {
                    metrics = new InstrumentationMetric()
                    {
                        Count = 1,
                        Milliseconds = sw.ElapsedMilliseconds,
                    };
                    Metrics.Collection.Add(key, metrics);
                }
            }

            return result;
        }
#endif

        /// <summary>
        /// Executes the specified procedure and records instrumentation metrics for the operation under the given key.
        /// </summary>
        /// <remarks>Instrumentation metrics, including execution count and elapsed time in milliseconds,
        /// are aggregated under the specified key. Metrics are updated in a thread-safe manner. Use this method to
        /// monitor performance and frequency of operations identified by unique keys.</remarks>
        /// <typeparam name="T">The type of the value returned by the procedure.</typeparam>
        /// <param name="key">The instrumentation key used to categorize and aggregate metrics for this operation.</param>
        /// <param name="proc">A delegate representing the operation to be executed and measured. The delegate should return a nullable
        /// value of type T.</param>
        /// <returns>The nullable result returned by the executed procedure.</returns>
#if !EnableCrudeInstrumentation
        public static T? Witness<T>(CrudeInstrumentationNullableProc<T?> proc) => proc();
#else
        public static T? Witness<T>(string key, CrudeInstrumentationNullableProc<T?> proc)
        {
            var sw = Stopwatch.StartNew();
            T? result = proc();
            sw.Stop();

            lock (Metrics)
            {
                if (Metrics.Collection.TryGetValue(key, out var metrics))
                {
                    metrics.Count++;
                    metrics.Milliseconds += sw.ElapsedMilliseconds;
                }
                else
                {
                    metrics = new InstrumentationMetric()
                    {
                        Count = 1,
                        Milliseconds = sw.ElapsedMilliseconds,
                    };
                    Metrics.Collection.Add(key, metrics);
                }
            }

            return result;
        }
#endif
    }
}
