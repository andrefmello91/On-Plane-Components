using System;
using Extensions;

namespace OnPlaneComponents
{
    /// <summary>
    ///     Constraint direction enumeration.
    /// </summary>
	public enum ConstraintDirection
	{
        /// <summary>
        ///     Constrained in none direction (point is free).
        /// </summary>
        None,

        /// <summary>
        ///     Constrained only in X (horizontal) direction.
        /// </summary>
        X,

        /// <summary>
        ///     Constrained only in Y (vertical) direction.
        /// </summary>
        Y,

        /// <summary>
        ///     Constrained in both X (horizontal) and Y (vertical) directions.
        /// </summary>
        Both
    }
    /// <summary>
    ///     Constraint struct.
    /// </summary>
    public struct Constraint : IEquatable<Constraint>, ICloneable<Constraint>
    {
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
        /// <inheritdoc cref="X"/>
        public bool Y { get; set; }

        /// <summary>
        ///     Get the constraint direction.
        /// </summary>
	    public ConstraintDirection Direction =>
		    X switch
		    {
                false when !Y => ConstraintDirection.None,
                false when  Y => ConstraintDirection.Y,
                true when  !Y => ConstraintDirection.X,
                _             => ConstraintDirection.Both
		    };

        /// <summary>
        ///     Constraint constructor.
        /// </summary>
        /// <param name="x">The X (horizontal) constraint.<para>True if point is constrained in this direction.</para></param>
        /// <param name="y">The X (horizontal) constraint.<para>True if point is constrained in this direction.</para></param>
        public Constraint(bool x, bool y)
        {
	        X = x;
	        Y = y;
        }

        /// <summary>
        ///     Get a <see cref="Constraint"/> from a <see cref="ConstraintDirection"/>.
        /// </summary>
        /// <param name="direction">The <see cref="ConstraintDirection"/>.</param>
        public static Constraint FromDirection(ConstraintDirection direction) =>
	        direction switch
	        {
                ConstraintDirection.X    => XOnly,
                ConstraintDirection.Y    => YOnly,
                ConstraintDirection.Both => FullConstraint,
                _                        => Free
			};

        /// <summary>
        ///     A free constraint.
        /// </summary>
        public static readonly Constraint Free = new Constraint(false, false);

        /// <summary>
        ///     A constraint in X direction.
        /// </summary>
        public static readonly Constraint XOnly = new Constraint(true, false);

        /// <summary>
        ///     A constraint in Y direction.
        /// </summary>
        public static readonly Constraint YOnly = new Constraint(false, true);

        /// <summary>
        ///     A constraint in both directions.
        /// </summary>
        public static readonly Constraint FullConstraint = new Constraint(true, true);

        public bool Equals(Constraint other) => !(other is null) && Direction == other.Value.Direction;

        public Constraint Clone() => new Constraint(X, Y);

        /// <summary>
        ///     Returns true if objects are equal.
        /// </summary>
        public static bool operator == (Constraint left, Constraint right) => left.Equals(right);

        /// <summary>
        ///     Returns true if objects are not equal.
        /// </summary
        public static bool operator != (Constraint left, Constraint right) => !left.Equals(right);
    }
}
