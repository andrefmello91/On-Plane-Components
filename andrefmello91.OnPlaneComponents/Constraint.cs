using System;
using andrefmello91.Extensions;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Constraint struct.
	/// </summary>
	public struct Constraint : IPlaneComponent<bool>, IEquatable<Constraint>, ICloneable<Constraint>
	{
		/// <summary>
		///     A free constraint.
		/// </summary>
		public static Constraint Free { get; } = new(false, false);

		/// <summary>
		///     A constraint in X direction.
		/// </summary>
		public static Constraint XOnly { get; } = new(true, false);

		/// <summary>
		///     A constraint in Y direction.
		/// </summary>
		public static Constraint YOnly { get; } = new(false, true);

		/// <summary>
		///     A constraint in both directions.
		/// </summary>
		public static Constraint Full { get; } = new(true, true);

		/// <summary>
		///     Get/set the X (horizontal) constraint.
		/// </summary>
		/// <remarks>
		///     True if point is constrained in this direction.
		/// </remarks>
		public bool X { get; set; }

		/// <summary>
		///     Get/set the Y (vertical) constraint.
		/// </summary>
		/// <inheritdoc cref="X" />
		public bool Y { get; set; }

		/// <summary>
		///     Returns true if the displacement in X direction from this constraint will be zero.
		/// </summary>
		/// <remarks>
		///     In this case <see cref="Direction" /> is <see cref="ComponentDirection.Both" /> or
		///     <see cref="ComponentDirection.X" />.
		/// </remarks>
		public bool IsXZero => IsZero || Direction is ComponentDirection.X;

		/// <summary>
		///     Returns true if the displacement in Y direction from this constraint will be zero.
		/// </summary>
		/// <remarks>
		///     In this case <see cref="Direction" /> is <see cref="ComponentDirection.Both" /> or
		///     <see cref="ComponentDirection.Y" />.
		/// </remarks>
		public bool IsYZero => IsZero || Direction is ComponentDirection.Y;

		/// <summary>
		///     Returns true if the displacement from this constraint will be zero.
		/// </summary>
		/// <remarks>
		///     In this case <see cref="Direction" /> is <see cref="ComponentDirection.Both" />.
		/// </remarks>
		public bool IsZero => Direction is ComponentDirection.Both;

		/// <summary>
		///     Get the constraint direction.
		/// </summary>
		public ComponentDirection Direction =>
			X switch
			{
				false when !Y => ComponentDirection.None,
				false when Y  => ComponentDirection.Y,
				true when !Y  => ComponentDirection.X,
				_             => ComponentDirection.Both
			};

		/// <summary>
		///     Constraint constructor.
		/// </summary>
		/// <param name="x">
		///     The X (horizontal) constraint.
		///     <para>True if point is constrained in this direction.</para>
		/// </param>
		/// <param name="y">
		///     The X (horizontal) constraint.
		///     <para>True if point is constrained in this direction.</para>
		/// </param>
		public Constraint(bool x, bool y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///     Get a <see cref="Constraint" /> from a <see cref="ComponentDirection" />.
		/// </summary>
		/// <param name="direction">The <see cref="ComponentDirection" />.</param>
		public static Constraint FromDirection(ComponentDirection direction) =>
			direction switch
			{
				ComponentDirection.X    => XOnly,
				ComponentDirection.Y    => YOnly,
				ComponentDirection.Both => Full,
				_                       => Free
			};


		/// <inheritdoc />
		public bool Equals(Constraint other) => Direction == other.Direction;

		/// <inheritdoc />
		public Constraint Clone() => new(X, Y);

		/// <summary>
		///     Returns true if objects are equal.
		/// </summary>
		public static bool operator ==(Constraint left, Constraint right) => left.Equals(right);

		/// <summary>
		///     Returns true if objects are not equal.
		/// </summary>
		public static bool operator !=(Constraint left, Constraint right) => !left.Equals(right);

		/// <inheritdoc />
		public override string ToString() => $"Constraint direction: {Direction}";
	}
}