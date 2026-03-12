using Ae.Engine.ExtensionMethods;
using Ae.Engine.Sprite;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ae.Engine.Mathematics
{
    /// <summary>
    /// 2d vector.
    /// Note that when the signed/unsigned is unspecified, it is unsigned (Degrees 0,360), Radians(0,2π), etc.
    /// </summary>
    public partial class AeVector
        : IComparable<AeVector>
    {
        /// <summary>
        /// Represents a method that is called when an AeVector changes.
        /// </summary>
        /// <remarks>Use this delegate to subscribe to change notifications for AeVector instances. The
        /// method assigned to this delegate will be invoked whenever the vector changes, allowing you to react to
        /// updates in its state.</remarks>
        /// <param name="vector">The AeVector instance that has changed. Cannot be null.</param>
        public delegate void OnChange(AeVector vector);
        /// <summary>
        /// Represents a method that is called when an AeVector changes.
        /// </summary>
        public event OnChange? OnChangeEvent;

        /// <summary>
        /// Returns a vector whose components are all zero.
        /// </summary>
        /// <returns>An instance of AeVector with all components set to zero.</returns>
        public static AeVector Zero() => new();

        /// <summary>
        /// Returns a unit vector pointing in the positive X direction.
        /// </summary>
        /// <returns>An instance of AeVector representing the unit vector (1, 0) along the X axis.</returns>
        public static AeVector UnitOfX() => new(1f, 0f);

        /// <summary>
        /// Returns a unit vector pointing in the positive Y direction.
        /// </summary>
        /// <remarks>Use this method to obtain a standardized vector for operations requiring a direction
        /// along the Y axis.</remarks>
        /// <returns>An instance of AeVector representing the unit vector (0, 1).</returns>
        public static AeVector UnitOfY() => new(0f, 1f);

        /// <summary>
        /// Creates a vector whose components are all set to one.
        /// </summary>
        /// <returns>A new instance of AeVector with both components equal to one.</returns>
        public static AeVector One() => new(1f, 1f);

        /// <summary>
        /// Gets or sets the X-coordinate value.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// Gets or sets the Y-coordinate value.
        /// </summary>
        public float Y { get; set; }

        #region ~Ctor. 

        /// <summary>
        /// Initializes a new instance of the AeVector class.
        /// </summary>
        public AeVector()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AeVector class using the specified angle in radians.
        /// </summary>
        /// <remarks>The X and Y components are computed as the cosine and sine of the specified angle,
        /// respectively. This constructor is useful for creating unit vectors pointing in a given direction.</remarks>
        /// <param name="radians">The angle, in radians, used to calculate the vector components. Represents the direction of the vector
        /// relative to the positive X-axis.</param>
        public AeVector(float radians)
        {
            X = (float)Math.Cos(radians);
            Y = (float)Math.Sin(radians);
        }

        /// <summary>
        /// Initializes a new instance of the AeVector structure with the specified X and Y components.
        /// </summary>
        /// <param name="x">The value to assign to the X component of the vector.</param>
        /// <param name="y">The value to assign to the Y component of the vector.</param>
        public AeVector(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the AeVector class by copying the values from the specified vector.
        /// </summary>
        /// <param name="p">The vector whose X and Y values are used to initialize the new instance. Cannot be null.</param>
        public AeVector(AeVector p)
        {
            X = p.X;
            Y = p.Y;
        }

        #endregion

        /// <summary>
        /// Returns a string representation of the object using invariant culture formatting for the X and Y values.
        /// </summary>
        /// <remarks>The returned string uses the invariant culture, ensuring consistent formatting
        /// regardless of the current locale.</remarks>
        /// <returns>A string formatted as "x{X}:y{Y}", where X and Y are displayed with up to five decimal places and thousands
        /// separators.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture, "x{0:#,##0.#####}:y{1:#,##0.#####}", X, Y);

        /// <summary>
        /// Parses a string representation of an AeVector and returns the corresponding AeVector instance.
        /// </summary>
        /// <param name="text">The string containing the AeVector to parse. Must be in the correct format; otherwise, a FormatException is
        /// thrown.</param>
        /// <returns>An AeVector instance parsed from the specified string.</returns>
        /// <exception cref="FormatException">Thrown if the provided string is not in the correct format for an AeVector.</exception>
        public static AeVector Parse(string text)
        {
            if (TryParse(text, out AeVector? vector))
            {
                return vector;
            }
            throw new FormatException($"The provided string '{text}' is not in the correct format for parsing an SiVector.");
        }

        /// <summary>
        /// Attempts to parse a string representation of a vector in the format "x{value}:y{value}".
        /// </summary>
        /// <remarks>The method expects the input string to use the invariant culture and to specify both
        /// x and y components, each prefixed with 'x' and 'y' respectively. Parsing fails if the format is incorrect or
        /// if the values are not valid floating-point numbers.</remarks>
        /// <param name="text">The input string containing the vector data to parse. Must be in the format "x{value}:y{value}" using
        /// invariant culture.</param>
        /// <param name="vector">When this method returns, contains the parsed vector if the operation succeeded; otherwise, contains <see
        /// langword="null"/>.</param>
        /// <returns>true if the string was successfully parsed into a vector; otherwise, false.</returns>
        public static bool TryParse(string text, [NotNullWhen(true)] out AeVector? vector)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                vector = default;
                return false;
            }

            var parts = text.Split(':');
            if (parts.Length != 2)
            {
                vector = default;
                return false;
            }

            var xPart = parts[0].Trim();
            var yPart = parts[1].Trim();

            if (!xPart.StartsWith("x") || !yPart.StartsWith("y"))
            {
                vector = default;
                return false;
            }

            xPart = xPart.Substring(1);
            yPart = yPart.Substring(1);

            var style = NumberStyles.Float | NumberStyles.AllowThousands;
            var culture = CultureInfo.InvariantCulture;

            if (!float.TryParse(xPart, style, culture, out float x))
            {
                vector = default;
                return false;
            }

            if (!float.TryParse(yPart, style, culture, out float y))
            {
                vector = default;
                return false;
            }

            vector = new AeVector(x, y);

            return true;
        }

        #region Valiatation helpers (not that I'mnot sure if these should use || or &&)

        /// <summary>
        /// Determines whether either coordinate of the current instance is not a number (NaN).
        /// </summary>
        /// <remarks>Use this method to check for invalid or uninitialized coordinate values before
        /// performing calculations that require valid numeric input.</remarks>
        /// <returns>true if either the X or Y coordinate is NaN; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNan()
                => float.IsNaN(X) || float.IsNaN(Y);

        /// <summary>
        /// Determines whether either coordinate of the current instance is infinite.
        /// </summary>
        /// <remarks>Use this method to check for infinite values in either coordinate, which may indicate
        /// invalid or unbounded results in mathematical operations.</remarks>
        /// <returns>true if either the X or Y coordinate is positive or negative infinity; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInfinity()
            => float.IsInfinity(X) || float.IsInfinity(Y);

        /// <summary>
        /// Determines whether any component of the vector is negative infinity.
        /// </summary>
        /// <remarks>Use this method to check for negative infinity values in vector components, which may
        /// indicate invalid or uninitialized data resulting from mathematical operations.</remarks>
        /// <returns>true if either the X or Y component is negative infinity; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNegativeInfinity()
            => float.IsNegativeInfinity(X) || float.IsNegativeInfinity(Y);

        /// <summary>
        /// Determines whether either component of the vector is considered near zero.
        /// </summary>
        /// <remarks>This method is useful for detecting vectors that are effectively aligned with an axis
        /// or have negligible magnitude in one direction. The definition of "near zero" depends on the implementation
        /// of the IsNearZero method for the component type.</remarks>
        /// <returns>true if either the X or Y component is near zero; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNearZero()
            => X.IsNearZero() || Y.IsNearZero();

        #endregion

        #region Converters.

        /// <summary>
        /// Creates a new RectangleF instance using the current X and Y coordinates, with the specified width and
        /// height.
        /// </summary>
        /// <param name="width">The width of the rectangle, in pixels. Must be a non-negative value.</param>
        /// <param name="height">The height of the rectangle, in pixels. Must be a non-negative value.</param>
        /// <returns>A RectangleF structure representing the rectangle at the current X and Y position with the specified width
        /// and height.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF(float width, float height)
            => new(X, Y, width, height);

        /// <summary>
        /// Creates a new RectangleF at the current X and Y coordinates with the specified size.
        /// </summary>
        /// <param name="size">The size, in floating-point units, to use for the width and height of the rectangle.</param>
        /// <returns>A RectangleF instance positioned at the current X and Y coordinates with the given width and height.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF(SizeF size)
            => new(X, Y, size.Width, size.Height);

        /// <summary>
        /// Creates a new rectangle at the current point with a width and height of 1.
        /// </summary>
        /// <remarks>This method is useful for representing a point as a rectangle with minimal area, such
        /// as for hit testing or drawing operations.</remarks>
        /// <returns>A <see cref="RectangleF"/> representing a unit rectangle positioned at the current coordinates.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF() => new(X, Y, 1f, 1f);


        /// <summary>
        /// Creates an instance of AeVector from a signed angle in degrees.
        /// </summary>
        /// <param name="angleInDegrees">The signed angle, in degrees, to convert. Positive values represent counterclockwise rotation; negative
        /// values represent clockwise rotation.</param>
        /// <returns>An AeVector representing the direction corresponding to the specified signed angle in degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector FromSignedDegrees(float angleInDegrees)
            => new(AeMath.DegToRad(AeMath.SignedDegreesToUnsigned(angleInDegrees)));

        /// <summary>
        /// Creates a new vector representing the specified unsigned angle in degrees.
        /// </summary>
        /// <param name="angleInDegrees">The angle, in degrees, to convert to a vector. Must be in the range [0, 360).</param>
        /// <returns>A vector corresponding to the direction of the given angle in degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector FromUnsignedDegrees(float angleInDegrees)
            => new(AeMath.DegToRad(angleInDegrees));

        /// <summary>
        /// Creates a new vector instance from an unsigned angle measured in radians.
        /// </summary>
        /// <param name="angleInRadians">The angle, in radians, representing the direction of the vector. Must be non-negative.</param>
        /// <returns>A vector corresponding to the specified unsigned angle in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector FromUnsignedRadians(float angleInRadians)
            => new(angleInRadians);


        /// <summary>
        /// Creates a new vector using the specified cardinal x and y components.
        /// </summary>
        /// <param name="x">The value of the x component of the vector.</param>
        /// <param name="y">The value of the y component of the vector.</param>
        /// <returns>A new AeVector instance with the specified x and y components.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector FromCardinal(float x, float y)
            => new(x, y);

        #endregion

        #region Operator Overloads: Float first.

        /// <summary>
        /// Subtracts each component of the specified vector from the given scalar and returns the resulting vector.
        /// </summary>
        /// <param name="scalar">The scalar value from which each component of the vector will be subtracted.</param>
        /// <param name="original">The vector whose components are subtracted from the scalar.</param>
        /// <returns>A new vector whose components are the result of subtracting each component of the original vector from the
        /// scalar.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(float scalar, AeVector original)
           => new AeVector(scalar - original.X, scalar - original.Y);

        /// <summary>
        /// Adds a scalar value to each component of the specified vector.
        /// </summary>
        /// <param name="scalar">The scalar value to add to each component of the vector.</param>
        /// <param name="original">The vector whose components will be incremented by the scalar value.</param>
        /// <returns>A new vector with each component increased by the specified scalar value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator +(float scalar, AeVector original)
            => new AeVector(original.X + scalar, original.Y + scalar);

        /// <summary>
        /// Scales the specified vector by the given scalar factor.
        /// </summary>
        /// <param name="scaleFactor">The scalar value by which to multiply each component of the vector.</param>
        /// <param name="original">The vector to be scaled.</param>
        /// <returns>A new vector whose components are the result of multiplying the original vector's components by the scale
        /// factor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator *(float scaleFactor, AeVector original)
            => new AeVector(original.X * scaleFactor, original.Y * scaleFactor);

        /// <summary>
        /// Divides each component of the specified vector by the given scale factor and returns the resulting vector.
        /// </summary>
        /// <param name="scaleFactor">The value by which each component of the vector is divided. If zero, the result is a zero vector.</param>
        /// <param name="original">The vector whose components are to be divided by the scale factor.</param>
        /// <returns>A new vector whose components are the result of dividing the scale factor by each component of the original
        /// vector. If the scale factor is zero, returns a vector with both components set to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator /(float scaleFactor, AeVector original)
        {
            if (scaleFactor == 0.0)
            {
                return new AeVector(0, 0);
            }
            return new AeVector(scaleFactor / original.X, scaleFactor / original.Y);
        }

        #endregion

        #region Operator Overloads: Float Second.

        /// <summary>
        /// Subtracts a scalar value from each component of the specified vector.
        /// </summary>
        /// <param name="original">The vector whose components will be reduced by the scalar value.</param>
        /// <param name="scalar">The scalar value to subtract from each component of the vector.</param>
        /// <returns>A new AeVector whose components are the result of subtracting the scalar value from the corresponding
        /// components of the original vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(AeVector original, float scalar)
           => new AeVector(original.X - scalar, original.Y - scalar);

        /// <summary>
        /// Adds a scalar value to each component of the specified vector.
        /// </summary>
        /// <param name="original">The vector whose components will be incremented by the scalar value.</param>
        /// <param name="scalar">The scalar value to add to each component of the vector.</param>
        /// <returns>A new vector with each component equal to the sum of the corresponding component in the original vector and
        /// the scalar value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator +(AeVector original, float scalar)
            => new AeVector(original.X + scalar, original.Y + scalar);

        /// <summary>
        /// Scales the specified vector by the given factor.
        /// </summary>
        /// <param name="original">The vector to be scaled.</param>
        /// <param name="scaleFactor">The factor by which to scale the vector components.</param>
        /// <returns>A new vector whose components are the original vector's components multiplied by the specified scale factor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator *(AeVector original, float scaleFactor)
            => new AeVector(original.X * scaleFactor, original.Y * scaleFactor);

        /// <summary>
        /// Divides the components of the specified vector by the given scale factor.
        /// </summary>
        /// <param name="original">The vector whose components are to be divided.</param>
        /// <param name="scaleFactor">The value by which each component of the vector is divided. If zero, the result is a zero vector.</param>
        /// <returns>A new vector whose components are the result of dividing the original vector's components by the scale
        /// factor. Returns a zero vector if the scale factor is zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator /(AeVector original, float scaleFactor)
            => scaleFactor == 0 ? Zero() : new AeVector(original.X / scaleFactor, original.Y / scaleFactor);

        #endregion

        #region Operator Overloads: SizeF.

        /// <summary>
        /// Subtracts the specified vector from the given size, producing a new vector representing the difference.
        /// </summary>
        /// <param name="modifier">The size whose width and height are used as the minuend values in the subtraction.</param>
        /// <param name="original">The vector whose X and Y components are subtracted from the size's width and height.</param>
        /// <returns>A new vector whose X component is the result of subtracting the vector's X from the size's width, and whose
        /// Y component is the result of subtracting the vector's Y from the negated size's height.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(SizeF modifier, AeVector original)
            => new AeVector(modifier.Width - original.X, -modifier.Height - original.Y);

        /// <summary>
        /// Subtracts the width and height of the specified modifier from the X and Y components of the original vector.
        /// </summary>
        /// <param name="original">The vector whose components are to be reduced.</param>
        /// <param name="modifier">The size whose width and height are subtracted from the original vector's X and Y components, respectively.</param>
        /// <returns>A new vector representing the result of subtracting the modifier's width and height from the original
        /// vector's X and Y components.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(AeVector original, SizeF modifier)
            => new AeVector(original.X - modifier.Width, original.Y - modifier.Height);

        #endregion

        #region Operator Overloads: Size.

        /// <summary>
        /// Subtracts the coordinates of the specified vector from the dimensions of the given size, returning a new
        /// vector representing the result.
        /// </summary>
        /// <param name="modifier">The size whose width and height are used as the minuend values in the subtraction.</param>
        /// <param name="original">The vector whose X and Y coordinates are subtracted from the size's width and height.</param>
        /// <returns>A new vector whose X and Y values are the result of subtracting the original vector's coordinates from the
        /// size's dimensions.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(Size modifier, AeVector original)
            => new AeVector(modifier.Width - original.X, modifier.Height - original.Y);

        /// <summary>
        /// Subtracts the width and height of a specified size from the X and Y components of the vector.
        /// </summary>
        /// <param name="original">The vector whose components are to be reduced.</param>
        /// <param name="modifier">The size whose width and height are subtracted from the vector's X and Y components, respectively.</param>
        /// <returns>A new vector with its X and Y components decreased by the width and height of the specified size.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(AeVector original, Size modifier)
            => new AeVector(original.X - modifier.Width, original.Y - modifier.Height);

        #endregion

        #region Operator Overloads: Vector -> Vector.

        /// <summary>
        /// Determines whether two AeVector instances are equal.
        /// </summary>
        /// <remarks>Equality is determined by comparing the values of the two instances. If both are
        /// null, they are considered equal.</remarks>
        /// <param name="left">The first AeVector instance to compare. Can be null.</param>
        /// <param name="right">The second AeVector instance to compare. Can be null.</param>
        /// <returns>true if the two AeVector instances are equal; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(AeVector? left, AeVector? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two AeVector instances are not equal.
        /// </summary>
        /// <param name="left">The first AeVector instance to compare. Can be null.</param>
        /// <param name="right">The second AeVector instance to compare. Can be null.</param>
        /// <returns>true if the specified AeVector instances are not equal; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(AeVector? left, AeVector? right)
            => !(left == right);

        /// <summary>
        /// Subtracts the components of one vector from another and returns the resulting vector.
        /// </summary>
        /// <param name="original">The vector whose components are to be subtracted from.</param>
        /// <param name="modifier">The vector whose components are subtracted from the original vector.</param>
        /// <returns>A new vector representing the difference between the original and modifier vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator -(AeVector original, AeVector modifier)
            => new AeVector(original.X - modifier.X, original.Y - modifier.Y);

        /// <summary>
        /// Adds two vectors and returns the result as a new vector.
        /// </summary>
        /// <param name="original">The first vector to add.</param>
        /// <param name="modifier">The second vector to add.</param>
        /// <returns>A new vector representing the sum of the two input vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator +(AeVector original, AeVector modifier)
            => new AeVector(original.X + modifier.X, original.Y + modifier.Y);

        /// <summary>
        /// Multiplies each component of the specified vector by the corresponding component of another vector.
        /// </summary>
        /// <param name="original">The vector whose components are to be multiplied.</param>
        /// <param name="scaleFactor">The vector providing the scale factors for each component.</param>
        /// <returns>A new vector whose components are the products of the corresponding components of the input vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator *(AeVector original, AeVector scaleFactor)
            => new AeVector(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        /// <summary>
        /// Determines whether the magnitude of the first vector is greater than the magnitude of the second vector.
        /// </summary>
        /// <remarks>This operator compares the lengths of the vectors, not their individual
        /// components.</remarks>
        /// <param name="v1">The first vector to compare.</param>
        /// <param name="v2">The second vector to compare.</param>
        /// <returns>true if the magnitude of v1 is greater than the magnitude of v2; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(AeVector v1, AeVector v2)
            => v1.Magnitude() > v2.Magnitude();

        /// <summary>
        /// Determines whether the magnitude of the first vector is less than the magnitude of the second vector.
        /// </summary>
        /// <remarks>This operator compares the lengths of the vectors, not their individual
        /// components.</remarks>
        /// <param name="v1">The first vector to compare.</param>
        /// <param name="v2">The second vector to compare.</param>
        /// <returns>true if the magnitude of v1 is less than the magnitude of v2; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(AeVector v1, AeVector v2)
            => v1.Magnitude() < v2.Magnitude();

        /// <summary>
        /// Divides each component of the specified vector by the corresponding component of another vector.
        /// </summary>
        /// <remarks>If both components of the scale factor are zero, the method returns a unit vector
        /// instead of performing division by zero. This behavior prevents exceptions and ensures a valid
        /// result.</remarks>
        /// <param name="original">The vector whose components are to be divided.</param>
        /// <param name="scaleFactor">The vector whose components are used as divisors for the corresponding components of the original vector.</param>
        /// <returns>A new vector containing the result of dividing each component of the original vector by the corresponding
        /// component of the scale factor vector. If both components of the scale factor are zero, returns a unit
        /// vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeVector operator /(AeVector original, AeVector scaleFactor)
            => scaleFactor.X == 0.0 && scaleFactor.Y == 0.0 ? One() :
                new AeVector(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        #endregion

        #region IComparible.

        /// <summary>
        /// Serves as the default hash function for the object.
        /// </summary>
        /// <remarks>The hash code is based on the string representation of the object. Use caution when
        /// relying on hash codes for objects whose string representation may change, as this can affect hash-based
        /// collections.</remarks>
        /// <returns>A 32-bit signed integer hash code representing the current object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>
        /// Determines whether the specified object is equal to the current vector, comparing the X and Y components
        /// rounded to four decimal places.
        /// </summary>
        /// <remarks>This method performs a comparison of the X and Y components after rounding them to
        /// four decimal places. This can help mitigate minor floating-point differences when determining
        /// equality.</remarks>
        /// <param name="o">The object to compare with the current vector. Can be null or an instance of AeVector.</param>
        /// <returns>true if the specified object is an AeVector and its X and Y components, rounded to four decimal places, are
        /// equal to those of the current vector; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? o)
            => Math.Round(((AeVector?)o)?.X ?? float.NaN, 4) == X && Math.Round(((AeVector?)o)?.Y ?? float.NaN, 4) == Y;

        /// <summary>
        /// Compares the magnitude of this vector to another vector and returns a value indicating their relative order.
        /// </summary>
        /// <remarks>Comparison is based on the Euclidean magnitude of each vector. This method can be
        /// used to sort vectors by their length.</remarks>
        /// <param name="other">The vector to compare with this instance. If null, this instance is considered greater.</param>
        /// <returns>A value less than zero if this vector is less than the other; zero if they are equal; greater than zero if
        /// this vector is greater than the other.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(AeVector? other)
        {
            if (other == null) return 1; // Consider this instance greater if other is null

            // Calculate the magnitudes
            var thisMagnitude = Math.Sqrt(X * X + Y * Y);
            var otherMagnitude = Math.Sqrt(other.X * other.X + other.Y * other.Y);

            // Use the magnitudes to determine ordering
            return thisMagnitude.CompareTo(otherMagnitude);
        }

        #endregion

        #region Direction.

        /// <summary>
        /// Angle in radians between [0,2π]
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public float Radians
        {
            get
            {
                var angle = RadiansSigned;
                if (angle < 0)
                {
                    angle += 2.0f * AeMath.Pi; // Convert negative angles to positive by adding 2π
                }
                return angle;
            }
            set
            {
                var radians = value > 0.0f ? value % AeMath.TwoPi : (value + AeMath.TwoPi) % AeMath.TwoPi;
                var cardinal = AeMath.RadToCardinal(radians);
                X = cardinal.X;
                Y = cardinal.Y;
                OnChangeEvent?.Invoke(this);
            }
        }

        /// <summary>
        /// Angle in radians between [−π,+π]
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public float RadiansSigned
        {
            get => AeMath.CardinalToRad(X, Y);
            set
            {
                var radians = value > 0.0f ? value % AeMath.TwoPi : (value + AeMath.TwoPi) % AeMath.TwoPi;
                var cardinal = AeMath.RadToCardinal(radians);
                X = cardinal.X;
                Y = cardinal.Y;
                OnChangeEvent?.Invoke(this);
            }
        }

        /// <summary>
        /// Angle in degrees between [−0,360]
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public float Degrees
        {
            get
            {
                float angleRadians = AeMath.CardinalToRad(X, Y);

                float angleDegrees = angleRadians * (180.0f / AeMath.Pi);
                if (angleDegrees < 0)
                {
                    angleDegrees += 360;
                }

                return angleDegrees;
            }
            set
            {
                var degrees = value > 0.0f ? value % 360.0f : (value + 360.0f) % 360.0f;
                var cardinal = AeMath.DegToCardinal(degrees);
                X = cardinal.X;
                Y = cardinal.Y;
                OnChangeEvent?.Invoke(this);
            }
        }

        /// <summary>
        /// Angle in degrees between [−180,180]
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public float DegreesSigned
        {
            get
            {
                float angleRadians = AeMath.CardinalToRad(X, Y);
                return angleRadians * (180.0f / AeMath.Pi);
            }
            set
            {
                var degrees = value > 0.0f ? value % 360.0f : (value + 360.0f) % 360.0f;
                var cardinal = AeMath.DegToCardinal(degrees);
                X = cardinal.X;
                Y = cardinal.Y;
                OnChangeEvent?.Invoke(this);
            }
        }

        #endregion

        /// <summary>
        /// Returns the clone of this vector.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeVector Clone()
            => new AeVector(X, Y);

        /// <summary>
        /// Rotates the vector by the given radians.
        /// </summary>
        /// <param name="angleRadians"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(float angleRadians)
        {
            // If orientation is invalid, reset it
            if (X == 0f && Y == 0f)
            {
                X = 1f;
                Y = 0f;
            }

            var cosTheta = (float)Math.Cos(angleRadians);
            var sinTheta = (float)Math.Sin(angleRadians);

            var x = X * cosTheta - Y * sinTheta;
            var y = X * sinTheta + Y * cosTheta;

            X = x;
            Y = y;

            OnChangeEvent?.Invoke(this);
        }

        /// <summary>
        /// Rotates the vector to the given radians while maintaining its length.
        /// </summary>
        /// <param name="angleRadians"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDirectionMaintainMagnitude(float angleRadians)
        {
            float magnitude = Magnitude();
            X = magnitude * (float)Math.Cos(angleRadians);
            Y = magnitude * (float)Math.Sin(angleRadians);
            OnChangeEvent?.Invoke(this);
        }

        /// <summary>
        /// Returns a normalized vector, with a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeVector Normalize()
        {
            var magnitude = (float)Math.Sqrt(X * X + Y * Y);
            return new AeVector(X / magnitude, Y / magnitude);
        }

        /// <summary>
        /// Calculates the orientation angle of the vector in radians relative to the positive X-axis.
        /// </summary>
        /// <remarks>The returned angle is measured in the counterclockwise direction from the positive
        /// X-axis. If both X and Y are zero, the result is zero.</remarks>
        /// <returns>A single-precision floating-point value representing the angle, in radians, between the vector and the
        /// positive X-axis. The value ranges from -π to π.</returns>
        public float OrientationInRadians()
            => (float)Math.Atan2(Y, X);

        /// <summary>
        /// Determines whether the vector is normalized.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNormalized()
            => AeMath.IsOne(X * X + Y * Y);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(AeVector vector)
             => X * vector.X + Y * vector.Y;

        /// <summary>
        /// Gets the length of the a vector. This represents the distance from its tail (starting point) to its head (end point) in the vector space.
        /// It provides a measure of how "long" the vector is in the specified direction.
        /// The length also serves as the vector magnitude.
        /// </summary>
        /// <altmember cref="LengthSquared"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude()
            => (float)Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// The length squared of a vector is the dot product of the vector with itself.
        /// This is useful for determining the angle between vectors or projecting one vector onto another.
        /// The length squared of a vector is the dot product of the vector with itself, and it's often used in optimizations where the actual
        /// distance (magnitude) isn't necessary. Calculating the square root (as in the magnitude) is computationally expensive, so using
        /// length squared can save resources when comparing distances or checking thresholds.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared()
            => X * X + Y * Y;

        /// <summary>
        /// Returns the X + Y;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Sum()
            => X + Y;

        /// <summary>
        /// Returns the Abs(X) + Abs(Y), useful for determining when a vector is non-zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SumAbs()
            => Math.Abs(X) + Math.Abs(Y);

        /// <summary>
        /// Calculates the Euclidean distance between two points in a 2D space (slower and precise, but not compatible with DistanceSquaredTo(...)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceTo(AeVector to)
        {
            var deltaX = Math.Pow(to.X - X, 2);
            var deltaY = Math.Pow(to.Y - Y, 2);
            return (float)Math.Sqrt(deltaY + deltaX);
        }

        /// <summary>
        /// Calculates the distance squared between two points in a 2D space (faster and but not compatible with DistanceTo(...)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceSquaredTo(AeVector to)
        {
            var deltaX = to.X - X;
            var deltaY = to.Y - Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        /// <summary>
        /// Returns a new vector with each component clamped to the specified minimum and maximum values.
        /// </summary>
        /// <remarks>The returned vector will have its X and Y components set to minValue if they are less
        /// than minValue, or to maxValue if they are greater than maxValue. The original vector instance is not
        /// modified.</remarks>
        /// <param name="minValue">The minimum value to which each component of the vector will be clamped.</param>
        /// <param name="maxValue">The maximum value to which each component of the vector will be clamped.</param>
        /// <returns>A new vector whose X and Y components are constrained within the specified range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeVector Clamp(float minValue, float maxValue)
        {
            var point = Clone();

            if (point.X < minValue)
            {
                point.X = minValue;
            }
            else if (point.X > maxValue)
            {
                point.X = maxValue;
            }

            if (point.Y < minValue)
            {
                point.Y = minValue;
            }
            else if (point.Y > maxValue)
            {
                point.Y = maxValue;
            }

            return point;
        }

        /// <summary>
        /// Returns the delta angle from this to another expressed in degrees from 180--180, positive
        /// figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object,
        /// positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngleInSignedDegrees(AeVector toLocation, float offsetAngle = 0f)
        {
            float fromAngle = AeMath.WrapDegreesUnsigned(Degrees + offsetAngle);
            float toAngle = this.AngleToInUnsignedDegrees(toLocation);
            return AeMath.WrapDegreesSigned(toAngle - fromAngle);
        }

        /// <summary>
        /// Returns the delta angle from this vector to another expressed in degrees from 0-360.
        /// </summary>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object,
        /// positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngleInUnsignedDegrees(AeVector toLocation, float offsetAngle = 0f)
        {
            float fromAngle = AeMath.WrapDegreesUnsigned(Degrees + offsetAngle);
            float toAngle = this.AngleToInUnsignedDegrees(toLocation);
            return AeMath.WrapDegreesUnsigned(toAngle - fromAngle);
        }

        #region Sprite Math.

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAway(IAeSprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngleInUnsignedDegrees(at));
            return deltaAngle < 180 + toleranceDegrees && deltaAngle > 180 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the object is pointing AWAY another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <param name="maxDistance"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAway(IAeSprite at, float toleranceDegrees, float maxDistance)
            => IsPointingAway(at, toleranceDegrees) && DistanceTo(at.Location) <= maxDistance;

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAt(IAeSprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngleInSignedDegrees(at));
            return deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees">The angle in degrees to consider the object to pointing at the other.</param>
        /// <param name="maxDistance">The distance in consider the object to pointing at the other.</param>
        /// <param name="offsetAngle">The offset in 0-360 degrees of the angle to calculate. For instance, 90 would tell if the right side of the object is pointing at the other.</param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAt(IAeSprite at, float toleranceDegrees, float maxDistance, float offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngleInUnsignedDegrees(at, offsetAngle));
            if (deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees)
            {
                return DistanceTo(at.Location) <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 180--180, positive figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngleInSignedDegrees(IAeSprite to, float offsetAngle = 0)
        {
            var angle = DeltaAngleInUnsignedDegrees(to, offsetAngle);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 0-360.
        /// </summary>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngleInUnsignedDegrees(IAeSprite to, float offsetAngle = 0)
        {
            float fromAngle = Degrees + offsetAngle;

            float angleTo = this.AngleToInUnsignedDegrees(to.Location);

            if (fromAngle < 0) fromAngle = 0 - fromAngle;
            if (angleTo < 0)
            {
                angleTo = 0 - angleTo;
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0f - Math.Abs(angleTo) % 360.0f;
            }

            return angleTo;
        }

        #endregion
    }
}
