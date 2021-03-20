using System;
using Extensions;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     On plane Point struct.
	/// </summary>
	public struct Point : IUnitConvertible<Point, LengthUnit>, IApproachable<Point, Length>, IEquatable<Point>, IComparable<Point>, ICloneable<Point>
	{
		#region Fields

		/// <summary>
		///     The tolerance to consider points equal.
		/// </summary>
		public static readonly Length Tolerance = Length.FromMillimeters(1E-3);

		/// <summary>
		///     The <see cref="Point" /> located at the origin of the coordinate system.
		/// </summary>
		public static readonly Point Origin = new Point(0, 0);

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the <see cref="LengthUnit" /> of this <see cref="Point" /> object.
		/// </summary>
		public LengthUnit Unit
		{
			get => X.Unit;
			set => ChangeUnit(value);
		}

		/// <summary>
		///     Returns true if this point is at <see cref="Origin" />.
		/// </summary>
		public bool IsAtOrigin => this == Origin;

		/// <summary>
		///     Returns true if <see cref="X" /> is nearly zero.
		/// </summary>
		public bool IsXZero => X.ApproxZero(Tolerance);

		/// <summary>
		///     Returns true if <see cref="Y" /> is nearly zero.
		/// </summary>
		public bool IsYZero => Y.ApproxZero(Tolerance);

		/// <summary>
		///     Get the X coordinate of this <see cref="Point" /> object.
		/// </summary>
		public Length X { get; private set; }

		/// <summary>
		///     Get the Y coordinate of this <see cref="Point" /> object.
		/// </summary>
		public Length Y { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		///     On plane <see cref="Point" /> object.
		/// </summary>
		/// <param name="x">The horizontal (x) coordinate.</param>
		/// <param name="y">The vertical (y) coordinate.</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of <paramref name="x" /> and <paramref name="y" /> coordinates.</param>
		public Point(double x, double y, LengthUnit unit = LengthUnit.Millimeter)
			: this (Length.From(x, unit), Length.From(y, unit))
		{
		}

		/// <inheritdoc cref="Point(double, double, LengthUnit)" />
		public Point(Length x, Length y)
		{
			X = x;
			Y = y.ToUnit(x.Unit);
		}

		#endregion

		#region

		/// <summary>
		///     Change the <see cref="LengthUnit" /> of this <see cref="Point" />.
		/// </summary>
		/// <param name="unit">The desired <see cref="LengthUnit" />.</param>
		public void ChangeUnit(LengthUnit unit)
		{
			if (unit == Unit)
				return;

			// Update values
			X = X.ToUnit(unit);
			Y = Y.ToUnit(unit);
		}

		/// <summary>
		///     Create a new <see cref="Point" /> with converted <see cref="LengthUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public Point Convert(LengthUnit unit) => Unit == unit ? this : new Point(X.ToUnit(unit), Y.ToUnit(unit));

		/// <summary>
		///     Get the horizontal distance, in <see cref="Unit" />, between this <see cref="Point" /> and
		///     <paramref name="other" />.
		/// </summary>
		/// <param name="other">The other <see cref="Point" /> to calculate distance.</param>
		public Length GetDistanceInX(Point other) => (X - other.X).ToUnit(Unit).Abs();

		/// <summary>
		///     Get the vertical distance, in <see cref="Unit" />, between this <see cref="Point" /> and <paramref name="other" />.
		/// </summary>
		/// <inheritdoc cref="GetDistanceInX" />
		public Length GetDistanceInY(Point other) => (Y - other.Y).ToUnit(Unit).Abs();

		/// <summary>
		///     Get the length, in <see cref="Unit" />, of a line connecting this <see cref="Point" /> to <paramref name="other" />
		///     .
		/// </summary>
		/// <inheritdoc cref="GetDistanceInX" />
		public Length GetDistance(Point other)
		{
			double
				x = GetDistanceInX(other).Value,
				y = GetDistanceInY(other).Value,
				h = (x * x + y * y).Sqrt();

			return
				Length.From(h, Unit);
		}

		/// <summary>
		///     Get the angle related to horizontal axis, in radians, of a line connecting this <see cref="Point" /> to
		///     <paramref name="other" />.
		/// </summary>
		/// <param name="other">The other <see cref="Point" /> to calculate the angle.</param>
		public double GetAngle(Point other)
		{
			Length
				x   = GetDistanceInX(other),
				y   = GetDistanceInY(other);

			if (x < Tolerance && y < Tolerance)
				return 0;

			if (y < Tolerance)
				return x > Length.Zero ? 0 : Constants.Pi;

			if (x < Tolerance)
				return y > Length.Zero ? Constants.PiOver2 : Constants.Pi3Over2;

			return
				(y / x).Atan();
		}

		/// <summary>
		///     Get the midpoint between this and <paramref name="other" /> <see cref="Point" />.
		/// </summary>
		/// <param name="other">The other <see cref="Point" />.</param>
		public Point MidPoint(Point other) => new Point(0.5 * (X + other.X), 0.5 * (Y + other.Y));

		/// <summary>
		///     Returns true if <paramref name="other" /> X coordinate is approximately equal to this object's X coordinate.
		/// </summary>
		/// <inheritdoc cref="Approaches" />
		public bool ApproxX(Point other, Length tolerance) => X.Approx(other.X, tolerance);

		/// <summary>
		///     Returns true if <paramref name="other" /> Y coordinate is approximately equal to this object's Y coordinate.
		/// </summary>
		/// <inheritdoc cref="Approaches" />
		public bool ApproxY(Point other, Length tolerance) => Y.Approx(other.Y, tolerance);

		/// <summary>
		///     Returns true if <paramref name="other" /> coordinates are approximately equal to this object's coordinates.
		/// </summary>
		/// <param name="other">The other <see cref="Point" /> to compare.</param>
		/// <param name="tolerance">The tolerance <see cref="Length" /> to consider coordinates equal.</param>
		public bool Approaches(Point other, Length tolerance) => ApproxX(other, tolerance) && ApproxY(other, tolerance);

		public Point Clone() => new Point(X, Y);

		/// <summary>
		///     Compare this to <paramref name="other" /> <see cref="Point" />.
		/// </summary>
		/// <remarks>
		///     If points are approximated, 0 is returned.
		///     <para>If this <see cref="Point" />'s Y coordinate is bigger, or Y is approximated and X is bigger, 1 is returned.</para>
		///     <para>
		///         If this <see cref="Point" />'s Y coordinate is smaller, or Y is approximated and X is smaller, -1 is
		///         returned.
		///     </para>
		/// </remarks>
		public int CompareTo(Point other)
		{
			// Points approximately equal
			if (Equals(other))
				return 0;

			// This point is bigger
			if (Y > other.Y || EqualsY(other) && X > other.X)
				return 1;

			// this Point is smaller
			return -1;
		}

		/// <inheritdoc cref="Approaches" />
		/// <remarks>
		///     Default <see cref="Tolerance" /> is considered.
		/// </remarks>
		public bool Equals(Point other) => Approaches(other, Tolerance);

		/// <inheritdoc cref="ApproxX" />
		/// <inheritdoc cref="Equals(Point)" />
		public bool EqualsX(Point other) => ApproxX(other, Tolerance);

		/// <inheritdoc cref="ApproxY" />
		/// <inheritdoc cref="EqualsX" />
		public bool EqualsY(Point other) => ApproxY(other, Tolerance);

		public override bool Equals(object obj) => obj is Point other && Equals(other);

		public override int GetHashCode() => (int) X.Value * (int) Y.Value;

		public override string ToString() => $"({X:0.00}, {Y:0.00})";

		#endregion

		#region Operators

		/// <summary>
		///     Returns true if <see cref="Point" />'s are equal.
		/// </summary>
		public static bool operator == (Point left, Point right) => left.Equals(right);

		/// <summary>
		///     Returns true if <see cref="Point" />'s are not equal.
		/// </summary>
		public static bool operator != (Point left, Point right) => !left.Equals(right);

		/// <summary>
		///     Returns true if <paramref name="left" />'s position is above or right to <paramref name="right" />.
		/// </summary>
		public static bool operator > (Point left, Point right) => left.CompareTo(right) == 1;

		/// <summary>
		///     Returns true if <paramref name="left" />'s position is below or left to <paramref name="right" />.
		/// </summary>
		public static bool operator < (Point left, Point right) => left.CompareTo(right) == -1;

		/// <summary>
		///     Returns true if <paramref name="left" />'s position is equal, above or right to <paramref name="right" />.
		/// </summary>
		public static bool operator >= (Point left, Point right) => left.CompareTo(right) >= 0;

		/// <summary>
		///     Returns true if <paramref name="left" />'s position is equal, below or left to <paramref name="right" />.
		/// </summary>
		public static bool operator <= (Point left, Point right) => left.CompareTo(right) <= 0;

		#endregion
	}
}