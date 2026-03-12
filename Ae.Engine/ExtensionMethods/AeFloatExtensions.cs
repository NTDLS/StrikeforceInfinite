using Ae.Engine.Mathematics;

namespace Ae.Engine.ExtensionMethods
{
    /// <summary>
    /// Provides extension methods for the <see cref="float"/> and <see cref="float"/> types to support common
    /// mathematical operations, value normalization, and range checks.
    /// </summary>
    /// <remarks>These methods simplify mathematical calculations and value handling for floating-point
    /// numbers, including conversion between degrees and radians, sign inversion, normalization, clamping, and range
    /// evaluation. All methods are static and intended for use as extension methods. Thread safety is guaranteed as the
    /// methods do not modify shared state.</remarks>
    public static class AeFloatExtensions
    {
        /// <summary>
        /// Converts the given degrees to radians.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToRadians(this float value) => AeMath.DegToRad(value);

        /// <summary>
        /// Converts the given radian to degrees.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToDegrees(this float value) => AeMath.RadToDeg(value);

        /// <summary>
        /// Multiplies the float by -1, inverting its sign, if the boolean is true.
        /// </summary>
        public static float Invert(this float value, bool shouldInvert) => shouldInvert ? value * -1 : value;

        /// <summary>
        /// Multiplies the float by -1, inverting its sign.
        /// </summary>
        public static float Invert(this float value) => value * -1;

        /// <summary>
        /// Returns whether the value is near to zero.
        /// </summary>
        public static bool IsNearZero(this float value)
            => AeMath.IsNearZero(value);

        /// <summary>
        /// Returns whether the value is near to zero.
        /// </summary>
        public static bool IsNearZero(this float? value)
            => value == null ? true : AeMath.IsNearZero((float)value);

        /// <summary>
        /// Degrees 0-360 -> 0 to 180 (right) and 0 to -180 (left).
        /// </summary>
        public static float NormalizeDegrees(this float value)
        {
            return (value + 180) % 360 - 180;
        }

        /// <summary>
        /// Degrees 0-Infinite -> 0 to 360
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float DenormalizeDegrees(this float value)
        {
            return ((dynamic)value + 360) % 360;
        }

        /// <summary>
        /// Determines whether the specified value is outside the range defined by the minimum and maximum values.
        /// </summary>
        /// <param name="value">The value to evaluate for inclusion within the specified range.</param>
        /// <param name="minValue">The inclusive lower bound of the range.</param>
        /// <param name="maxValue">The inclusive upper bound of the range.</param>
        /// <returns>Returns <see langword="true"/> if the value is less than the minimum value or greater than the maximum
        /// value; otherwise, <see langword="false"/>.</returns>
        public static bool IsNotBetween(this float value, float minValue, float maxValue)
        {
            return !value.IsBetween(minValue, maxValue);
        }

        /// <summary>
        /// Determines whether the specified nullable float value is outside the inclusive range defined by the minimum
        /// and maximum values.
        /// </summary>
        /// <param name="value">The nullable float value to evaluate. If null, the method returns <see langword="true"/>.</param>
        /// <param name="minValue">The inclusive lower bound of the range to compare against.</param>
        /// <param name="maxValue">The inclusive upper bound of the range to compare against.</param>
        /// <returns>Returns <see langword="true"/> if the value is less than the minimum, greater than the maximum, or null;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool IsNotBetween(this float? value, float minValue, float maxValue)
        {
            return !value.IsBetween(minValue, maxValue);
        }

        /// <summary>
        /// Determines whether the specified value falls within the range defined by the minimum and maximum values,
        /// inclusive.
        /// </summary>
        /// <remarks>If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>, the
        /// method treats the range as reversed and checks if <paramref name="value"/> is between <paramref
        /// name="maxValue"/> and <paramref name="minValue"/>.</remarks>
        /// <param name="value">The value to evaluate for inclusion within the specified range.</param>
        /// <param name="minValue">The lower bound of the range. If greater than <paramref name="maxValue"/>, the range is considered reversed.</param>
        /// <param name="maxValue">The upper bound of the range. If less than <paramref name="minValue"/>, the range is considered reversed.</param>
        /// <returns>A value indicating whether <paramref name="value"/> is between <paramref name="minValue"/> and <paramref
        /// name="maxValue"/>, inclusive. Returns <see langword="true"/> if the value is within the range; otherwise,
        /// <see langword="false"/>.</returns>
        public static bool IsBetween(this float value, float minValue, float maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        /// <summary>
        /// Determines whether the specified nullable float value falls within the inclusive range defined by the
        /// minimum and maximum values.
        /// </summary>
        /// <remarks>If minValue is greater than maxValue, the method treats the range as reversed and
        /// checks if the value is between maxValue and minValue. The method returns false if value is null.</remarks>
        /// <param name="value">The nullable float value to evaluate. If null, the method returns false.</param>
        /// <param name="minValue">The lower bound of the range, inclusive. If greater than <paramref name="maxValue"/>, the range is evaluated
        /// in reverse.</param>
        /// <param name="maxValue">The upper bound of the range, inclusive. If less than <paramref name="minValue"/>, the range is evaluated in
        /// reverse.</param>
        /// <returns>true if the value is within the inclusive range between minValue and maxValue; otherwise, false.</returns>
        public static bool IsBetween(this float? value, float minValue, float maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        public static float Clamp(this float value, float minValue, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }

        /// <summary>
        /// Clips a value to a max value.
        /// </summary>
        public static float Clamp(this float value, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else return value;
        }

        /// <summary>
        /// Take a value divides it by two and makes it negative if it over a given threshold
        /// </summary>
        public static float SplitToSigned(this float value, float halfwayPoint)
        {
            value /= 2.0f;

            if (value > halfwayPoint)
            {
                value *= -1;
            }

            return value;
        }
    }
}
