using NTDLS.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ae.Engine.Types
{
    /// <summary>
    /// Represents a range defined by minimum and maximum values of a comparable value type.
    /// </summary>
    /// <remarks>The range is valid when the minimum value is less than or equal to the maximum value, and the
    /// maximum value is greater than the default value for the type. This class can be used to represent numeric
    /// intervals or other comparable value ranges.</remarks>
    /// <typeparam name="T">The value type used for the range boundaries. Must implement <see cref="IComparable{T}"/>.</typeparam>
    public class AeRange<T> where T
        : struct, IComparable<T>
    {
        /// <summary>
        /// Gets or sets the minimum allowable value for the range.
        /// </summary>
        public T Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum value allowed or used in the current context.
        /// </summary>
        public T Max { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeRange class.
        /// </summary>
        public AeRange() { }

        /// <summary>
        /// Initializes a new instance of the AeRange class with the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value of the range. Must be less than or equal to the maximum value.</param>
        /// <param name="max">The maximum value of the range. Must be greater than or equal to the minimum value.</param>
        public AeRange(T min, T max)
        {
            Min = min;
            Max = max;
            Validate();
        }

        /// <summary>
        /// Initializes a new instance of the AeRange class with the specified value as both the minimum and maximum
        /// bounds.
        /// </summary>
        /// <remarks>This constructor creates a range where the minimum and maximum are equal, effectively
        /// representing a single value range. The value is validated upon initialization.</remarks>
        /// <param name="value">The value to set as both the minimum and maximum of the range.</param>
        public AeRange(T value)
        {
            Min = value;
            Max = value;
            Validate();
        }

        /// <summary>
        /// Determines whether the current range is valid based on its minimum and maximum values.
        /// </summary>
        /// <remarks>This method uses the default comparer for the generic type parameter to evaluate the
        /// range. The validity check ensures that the range is logically consistent and that the maximum value is
        /// meaningful for the type.</remarks>
        /// <returns>true if the minimum value is less than or equal to the maximum value and the maximum value is greater than
        /// the default value for the type; otherwise, false.</returns>
        public bool IsValid()
            => Min.CompareTo(Max) <= 0
               && Comparer<T>.Default.Compare(Max, default) > 0;

        /// <summary>
        /// Validates that the range defined by Min and Max is logically correct.
        /// </summary>
        /// <remarks>Call this method to ensure that the range values are suitable for further operations.
        /// Validation checks include logical ordering and positivity constraints on Max.</remarks>
        /// <exception cref="ArgumentException">Thrown if Min is greater than Max, or if Max is less than or equal to zero.</exception>
        public void Validate()
        {
            if (Min.CompareTo(Max) > 0)
                throw new ArgumentException("Range invalid: Min must be <= Max.");

            if (Comparer<T>.Default.Compare(Max, default) < 0)
                throw new ArgumentException("Range invalid: Max must be > 0.");
        }

        /// <summary>
        /// Returns a string representation of the range using invariant culture formatting.
        /// </summary>
        /// <remarks>The returned string uses invariant culture to ensure consistent formatting regardless
        /// of the current locale.</remarks>
        /// <returns>A string formatted as "Min:Max", where Min and Max are displayed with up to five decimal places and
        /// thousands separators.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture, "{0:#,##0.#####}:{1:#,##0.#####}", Min, Max);

        /// <summary>
        /// Parses the specified string into an instance of AeRange.
        /// </summary>
        /// <typeparam name="R">The value type used for the range boundaries. Must implement IComparable.</typeparam>
        /// <param name="text">The string representation of the range to parse. Must be in a valid format for AeRange.</param>
        /// <returns>An AeRange instance parsed from the specified string.</returns>
        /// <exception cref="FormatException">Thrown if the provided string is not in the correct format for parsing an AeRange.</exception>
        public static AeRange<R> Parse<R>(string text)
            where R : struct, IComparable<R>
        {
            if (TryParse<R>(text, out AeRange<R>? range))
            {
                return range;
            }
            throw new FormatException($"The provided string '{text}' is not in the correct format for parsing an SiRange.");
        }

        /// <summary>
        /// Attempts to parse a string representation of a range into an <see cref="AeRange{R}"/> instance.
        /// </summary>
        /// <remarks>The method expects the input string to contain two values separated by a colon,
        /// representing the minimum and maximum of the range. Both values are converted.</remarks>
        public static bool TryParse<R>(string text, [NotNullWhen(true)] out AeRange<R>? range)
            where R : struct, IComparable<R>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                range = default;
                return false;
            }

            var parts = text.Split(':');
            if (parts.Length != 2)
            {
                range = default;
                return false;
            }

            var min = Converters.ConvertTo<R>(parts[0].Trim());
            var max = Converters.ConvertTo<R>(parts[1].Trim());

            range = new AeRange<R>(min, max);

            return true;
        }
    }
}
