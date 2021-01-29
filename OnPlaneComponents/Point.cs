using System;
using Extensions.Number;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
	/// <summary>
	///     On plane Point struct.
	/// </summary>
	public struct Point : IUnitConvertible<Point, LengthUnit>, IEquatable<Point>, IComparable<Point>
	{
		#region Fields

		/// <summary>
		///     The tolerance to consider points equal.
		/// </summary>
		public static readonly Length Tolerance = Length.FromMillimeters(1E-3);

		private Length _x, _y;

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the <see cref="LengthUnit" /> of this <see cref="Point" /> object.
		/// </summary>
		public LengthUnit Unit
		{
			get => _x.Unit;
			set => ChangeUnit(value);
		}

		/// <summary>
		///     Get the X coordinate of this <see cref="Point" /> object, in current <see cref="Unit" />.
		/// </summary>
		public double X => _x.Value;

		/// <summary>
		///     Get the Y coordinate of this <see cref="Point" /> object, in current <see cref="Unit" />.
		/// </summary>
		public double Y => _y.Value;

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
			_x = x;
			_y = y.ToUnit(x.Unit);
		}

		#endregion

		#region  Methods

		/// <summary>
		///     Change the <see cref="LengthUnit" /> of this <see cref="Point" />.
		/// </summary>
		/// <param name="unit">The desired <see cref="LengthUnit" />.</param>
		public void ChangeUnit(LengthUnit unit)
		{
			if (unit == Unit)
				return;

			// Update values
			_x = _x.ToUnit(unit);
			_y = _y.ToUnit(unit);
		}

		/// <summary>
		///     Create a new <see cref="Point" /> with converted <see cref="LengthUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public Point Convert(LengthUnit unit) => Unit == unit ? this : new Point(_x.ToUnit(unit), _y.ToUnit(unit));

		/// <summary>
		///     Create a copy of this <see cref="Point" /> object.
		/// </summary>
		public Point Copy() => new Point(_x, _y);

		/// <summary>
		///     Get the horizontal distance, in <see cref="Unit" />, between this <see cref="Point" /> and
		///     <paramref name="other" />.
		/// </summary>
		/// <param name="other">The other <see cref="Point" /> to calculate distance.</param>
		public double GetDistanceInX(Point other) => (X - other.Convert(Unit).X).Abs();

		/// <summary>
		///     Get the vertical distance, in <see cref="Unit" />, between this <see cref="Point" /> and <paramref name="other" />.
		/// </summary>
		/// <inheritdoc cref="GetDistanceInX" />
		public double GetDistanceInY(Point other) => (Y - other.Convert(Unit).Y).Abs();

		/// <summary>
		///     Get the length, in <see cref="Unit" />, of a line connecting this <see cref="Point" /> to <paramref name="other" />
		///     .
		/// </summary>
		/// <inheritdoc cref="GetDistanceInX" />
		public double GetDistance(Point other)
		{
			double
				x = GetDistanceInX(other),
				y = GetDistanceInY(other);

			return
				(x * x + y * y).Sqrt();
		}

		/// <summary>
		///     Get the angle related to horizontal axis, in radians, of a line connecting this <see cref="Point" /> to
		///     <paramref name="other" />.
		/// </summary>
		/// <param name="other">The other <see cref="Point" /> to calculate the angle.</param>
		public double GetAngle(Point other)
		{
			double
				x   = GetDistanceInX(other),
				y   = GetDistanceInY(other),
				tol = Tolerance.ToUnit(Unit).Value;

			if (x < tol && y < tol)
				return 0;

			if (y < tol)
				return x > 0 ? 0 : Constants.Pi;

			if (x < tol)
				return y > 0 ? Constants.PiOver2 : Constants.Pi3Over2;

			return
				(y / x).Atan();
		}

		/// <summary>
		///     Get the midpoint between this and <paramref name="other" /> <see cref="Point" />.
		/// </summary>
		/// <param name="other">The other <see cref="Point" />.</param>
		public Point MidPoint(Point other) => new Point(0.5 * (_x + other._x), 0.5 * (_y + other._y));

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
			if (_y > other._y || EqualsY(other) && _x > other._x)
				return 1;

			// this Point is smaller
			return -1;
		}

		/// <summary>
		///     <inheritdoc cref="Equals(Point, Length)" />
		///     <para><see cref="Tolerance" /> is considered.</para>
		/// </summary>
		/// <inheritdoc cref="Equals(Point, Length)" />
		public bool Equals(Point other) => Equals(other, Tolerance);

		/// <summary>
		///     <inheritdoc cref="EqualsX(Point, Length)" />
		///     <para><see cref="Tolerance" /> is considered.</para>
		/// </summary>
		/// <inheritdoc cref="Equals(Point, Length)" />
		public bool EqualsX(Point other) => EqualsX(other, Tolerance);

		/// <summary>
		///     Returns true if <paramref name="other" /> X coordinate is approximately equal to this object's X coordinate.
		/// </summary>
		/// <inheritdoc cref="Equals(Point, Length)" />
		public bool EqualsX(Point other, Length tolerance) => (_x - other._x).Millimeters.Abs() <= tolerance.Millimeters;

		/// <summary>
		///     <inheritdoc cref="EqualsX(Point, Length)" />
		///     <para><see cref="Tolerance" /> is considered.</para>
		/// </summary>
		/// <inheritdoc cref="Equals(Point, Length)" />
		public bool EqualsY(Point other) => EqualsX(other, Tolerance);

		/// <summary>
		///     Returns true if <paramref name="other" /> Y coordinate is approximately equal to this object's Y coordinate.
		/// </summary>
		/// <inheritdoc cref="Equals(Point, Length)" />
		public bool EqualsY(Point other, Length tolerance) => (_y - other._y).Millimeters.Abs() <= tolerance.Millimeters;

		/// <summary>
		///     Returns true if <paramref name="other" /> coordinates are approximately equal to this object's coordinates.
		/// </summary>
		/// <param name="other">The other <see cref="Point" /> to compare.</param>
		/// <param name="tolerance">The tolerance <see cref="Length" /> to consider coordinates equal.</param>
		public bool Equals(Point other, Length tolerance) => EqualsX(other, tolerance) && EqualsY(other, tolerance);

		public override bool Equals(object obj) => obj is Point other && Equals(other);

		public override int GetHashCode() => (int) X * (int) Y;

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