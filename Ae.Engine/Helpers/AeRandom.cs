using Ae.Engine.Mathematics;
using Ae.Engine.Types;
using System;
using System.Collections.Generic;

namespace Ae.Engine.Helpers
{
    /// <summary>
    /// Provides utility methods for generating random values, including numbers, signs, selections from collections,
    /// and random vectors. Methods support both deterministic and probabilistic randomization scenarios.
    /// </summary>
    /// <remarks>The class exposes static methods for common random operations, such as generating random
    /// floats, integers, and selecting random elements from collections. It is thread-safe for typical usage, but
    /// callers should avoid modifying the shared Generator instance directly. Use these methods to simplify
    /// randomization logic in applications where reproducible or varied random values are needed.</remarks>
    public static class AeRandom
    {
        /// <summary>
        /// Provides a shared instance of the random number generator for generating random values.
        /// </summary>
        /// <remarks>This static field can be used to generate random numbers throughout the application.
        /// Using a shared instance avoids the overhead of creating multiple generators, but may result in less random
        /// sequences if accessed concurrently from multiple threads.</remarks>
        public static Random Generator { get; private set; } = new();

        /// <summary>
        /// Generates a random floating-point number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A single-precision floating-point number greater than or equal to 0.0 and less than 1.0.</returns>
        public static float NextFloat() => (float)Generator.NextDouble();

        /// <summary>
        /// Returns a random float value within a specified percentage variance of the original value.
        /// </summary>
        /// <remarks>Use this method to introduce controlled random variation to a value, such as for
        /// simulation or testing purposes. The result may be less than or greater than the original value, depending on
        /// the random selection.</remarks>
        /// <param name="value">The base value to which variance will be applied.</param>
        /// <param name="variancePercentDecimal">The percentage variance, expressed as a decimal (e.g., 0.1 for 10%), that determines the range of possible
        /// variation from the base value. Must be non-negative.</param>
        /// <returns>A float value randomly selected within the range defined by the base value plus or minus the specified
        /// percentage variance.</returns>
        public static float Variance(float value, float variancePercentDecimal)
        {
            float range = value * variancePercentDecimal;
            return value + Between(-range, range);
        }

        /// <summary>
        /// 50/50 chance to return a positive/negative of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float RandomSign(float value)
            => FlipCoin() ? value : -value;

        /// <summary>
        /// Selects and returns one value from the two provided options.
        /// </summary>
        /// <typeparam name="T">The type of the values to select from.</typeparam>
        /// <param name="one">The first value to consider for selection.</param>
        /// <param name="two">The second value to consider for selection.</param>
        /// <returns>A value of type T selected from the provided options.</returns>
        public static T OneOf<T>(T one, T two)
            => OneOf([one, two]);

        /// <summary>
        /// Returns one of the provided values of type T.
        /// </summary>
        /// <typeparam name="T">The type of the values to select from.</typeparam>
        /// <param name="one">The first value to consider for selection.</param>
        /// <param name="two">The second value to consider for selection.</param>
        /// <param name="three">The third value to consider for selection.</param>
        /// <returns>A value of type T selected from the provided arguments.</returns>
        public static T OneOf<T>(T one, T two, T three)
            => OneOf([one, two, three]);

        /// <summary>
        /// Selects and returns one value from the provided four options of type T.
        /// </summary>
        /// <typeparam name="T">The type of the values to choose from.</typeparam>
        /// <param name="one">The first candidate value to select.</param>
        /// <param name="two">The second candidate value to select.</param>
        /// <param name="three">The third candidate value to select.</param>
        /// <param name="four">The fourth candidate value to select.</param>
        /// <returns>A value of type T selected from the four provided candidates.</returns>
        public static T OneOf<T>(T one, T two, T three, T four)
            => OneOf([one, two, three, four]);

        /// <summary>
        /// Selects and returns a random element from the specified array.
        /// </summary>
        /// <remarks>If the array is empty, the method may throw an exception. Ensure the array contains
        /// at least one element before calling this method.</remarks>
        /// <typeparam name="T">The type of elements contained in the array.</typeparam>
        /// <param name="values">An array of values to select from. Must not be null or empty.</param>
        /// <returns>A randomly chosen element from the input array.</returns>
        public static T OneOf<T>(T[] values)
            => values[Between(0, values.Length - 1)];

        /// <summary>
        /// Selects and returns a random element from the specified list.
        /// </summary>
        /// <remarks>This method is useful for scenarios where a random choice from a collection is
        /// needed, such as sampling or randomized testing. The selection is uniformly random across all elements in the
        /// list.</remarks>
        /// <typeparam name="T">The type of elements contained in the list.</typeparam>
        /// <param name="values">The list of values to select from. Cannot be null or empty.</param>
        /// <returns>A randomly selected element from the list.</returns>
        /// <exception cref="ArgumentException">Thrown if the list is null or contains no elements.</exception>
        public static T OneOf<T>(this IList<T> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("Collection cannot be empty.", nameof(values));

            return values[Between(0, values.Count - 1)];
        }

        /// <summary>
        /// Returns a randomly selected element from the specified list, or null if the list is null or empty.
        /// </summary>
        /// <remarks>This method is useful for scenarios where a random selection from a collection is
        /// needed, and gracefully handles null or empty lists by returning null. The randomness is determined by the
        /// internal selection logic.</remarks>
        /// <typeparam name="T">The type of elements in the list. Must be a reference or nullable value type.</typeparam>
        /// <param name="values">The list of values to select from. Can be null or empty.</param>
        /// <returns>A randomly chosen element from the list, or null if the list is null or contains no elements.</returns>
        public static T? OneOfNullable<T>(this IList<T>? values)
        {
            if (values == null || values.Count == 0)
            {
                return default;
            }
            return values[Between(0, values.Count - 1)];
        }

        /// <summary>
        /// Determines whether a random event occurs based on a specified chance out of a total number of possibilities.
        /// </summary>
        /// <param name="chanceIn">The number of successful outcomes for the event. Must be between 0 and the value of <paramref
        /// name="outOf"/>.</param>
        /// <param name="outOf">The total number of possible outcomes. Must be greater than 0.</param>
        /// <returns>A value indicating whether the event occurs. Returns <see langword="true"/> if the random outcome falls
        /// within the specified chance; otherwise, <see langword="false"/>.</returns>
        public static bool ChanceIn(int chanceIn, int outOf)
            => Generator.Next(1, outOf + 1) <= chanceIn;

        /// <summary>
        /// Determines whether a random event occurs based on the specified percentage chance.
        /// </summary>
        /// <remarks>This method uses a uniform random number generator to evaluate the chance. It is
        /// suitable for simple probability checks in games or simulations.</remarks>
        /// <param name="percentageWholeNumber">The probability of the event occurring, expressed as a whole number percentage from 0 to 100. Values outside
        /// this range may produce undefined results.</param>
        /// <returns>A value indicating whether the event occurs. Returns <see langword="true"/> if the random event occurs;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool PercentChance(int percentageWholeNumber)
            => ((float)Generator.NextDouble() * 100) <= percentageWholeNumber;

        /// <summary>
        /// Generates a random boolean value representing a coin flip.
        /// </summary>
        /// <remarks>This method can be used to simulate a fair binary outcome, such as heads or tails in
        /// a coin toss. Each call is independent and has an equal probability of returning <see langword="true"/> or
        /// <see langword="false"/>.</remarks>
        /// <returns>A random boolean value. <see langword="true"/> if the coin flip results in heads; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool FlipCoin() => (Generator.Next(2) == 0);

        /// <summary>
        /// Returns either -1 or 1.
        /// </summary>
        /// <returns></returns>
        public static float PositiveOrNegative() => (Generator.Next(2) == 0 ? 1 : -1);

        /// <summary>
        /// Generates a random floating-point number within the specified range.
        /// </summary>
        /// <remarks>The returned value is uniformly distributed between minValue (inclusive) and maxValue
        /// (exclusive). If minValue is greater than maxValue, the result may be unexpected.</remarks>
        /// <param name="minValue">The inclusive lower bound of the range to generate the random number.</param>
        /// <param name="maxValue">The exclusive upper bound of the range to generate the random number.</param>
        /// <returns>A random float greater than or equal to minValue and less than maxValue.</returns>
        public static float Between(float minValue, float maxValue)
            => minValue + (maxValue - minValue) * (float)Generator.NextDouble();

        /// <summary>
        /// Generates a random integer within the specified inclusive range.
        /// </summary>
        /// <param name="minValue">The lower bound of the range, inclusive. Must be less than or equal to <paramref name="maxValue"/>.</param>
        /// <param name="maxValue">The upper bound of the range, inclusive. Must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <returns>A random integer greater than or equal to <paramref name="minValue"/> and less than or equal to <paramref
        /// name="maxValue"/>.</returns>
        public static int Between(int minValue, int maxValue)
            => Generator.Next(minValue, maxValue + 1);

        /// <summary>
        /// Generates a random floating-point value within the specified range.
        /// </summary>
        /// <param name="range">The range of values to select from. The value will be greater than or equal to <paramref name="range.Min"/>
        /// and less than or equal to <paramref name="range.Max"/>.</param>
        /// <returns>A random float value between <paramref name="range.Min"/> and <paramref name="range.Max"/>.</returns>
        public static float Between(AeRange<float> range)
            => range.Min + (range.Max - range.Min) * (float)Generator.NextDouble();

        /// <summary>
        /// Generates a random integer within the specified inclusive range.
        /// </summary>
        /// <remarks>If <paramref name="range"/>.Min is greater than <paramref name="range"/>.Max, the
        /// method may throw an exception depending on the implementation of the random generator.</remarks>
        /// <param name="range">The range of integers to select from. The minimum and maximum values define the inclusive bounds.</param>
        /// <returns>An integer value greater than or equal to <paramref name="range"/>.Min and less than or equal to <paramref
        /// name="range"/>.Max.</returns>
        public static int Between(AeRange<int> range)
            => Generator.Next(range.Min, range.Max + 1);

        /// <summary>
        /// Generates a random floating-point value within the specified range, or returns the default value if the
        /// range is null.
        /// </summary>
        /// <remarks>The generated value is uniformly distributed between the range's minimum and maximum
        /// values. This method is useful for scenarios where a random value is needed, but a fallback value should be
        /// provided if the range is not specified.</remarks>
        /// <param name="range">The range within which to generate the random value. If null, the method returns the specified default
        /// value.</param>
        /// <param name="defaultValue">The value to return if the range is null.</param>
        /// <returns>A random float between the minimum and maximum values of the range, or the default value if the range is
        /// null.</returns>
        public static float Between(AeRange<float>? range, float defaultValue)
            => range == null ? defaultValue : range.Min + (range.Max - range.Min) * (float)Generator.NextDouble();

        /// <summary>
        /// Returns a random integer within the specified range, or a default value if the range is null.
        /// </summary>
        /// <remarks>The returned value is inclusive of the minimum and maximum bounds specified in the
        /// range. This method is useful for safely generating random numbers when the range may be optional.</remarks>
        /// <param name="range">The range of integers to select from. If null, the method returns the specified default value.</param>
        /// <param name="defaultValue">The value to return if the range parameter is null.</param>
        /// <returns>A random integer within the inclusive range defined by the Min and Max properties of the range parameter, or
        /// the default value if range is null.</returns>
        public static int Between(AeRange<int>? range, int defaultValue)
            => range == null ? defaultValue : Generator.Next(range.Min, range.Max + 1);

        /// <summary>
        /// Generates a vector representing a random orientation in degrees within the range 0 to 359.
        /// </summary>
        /// <returns>An instance of AeVector representing a randomly selected orientation. The orientation will be within the
        /// range of 0 to 359 degrees.</returns>
        public static AeVector RandomOrientationVector()
            => AeVector.FromUnsignedDegrees(Between(0, 359));
    }
}