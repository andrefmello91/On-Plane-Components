﻿namespace andrefmello91.OnPlaneComponents;

public partial struct PlaneDisplacement
{

	#region Operators

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with summed components, in left argument's unit.
	/// </summary>
	public static PlaneDisplacement operator +(PlaneDisplacement left, PlaneDisplacement right) => new(left.X + right.X.ToUnit(left.Unit), left.Y + right.Y.ToUnit(left.Unit));

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with components divided by a <see cref="double" />.
	/// </summary>
	public static PlaneDisplacement operator /(PlaneDisplacement displacement, double divider) => new(displacement.X / divider, displacement.Y / divider);

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with components divided by an <see cref="int" />.
	/// </summary>
	public static PlaneDisplacement operator /(PlaneDisplacement displacement, int divider) => displacement / (double) divider;
	/// <summary>
	///     Returns true if components are equal.
	/// </summary>
	public static bool operator ==(PlaneDisplacement left, PlaneDisplacement right) => left.Equals(right);

	/// <summary>
	///     Returns true if components are different.
	/// </summary>
	public static bool operator !=(PlaneDisplacement left, PlaneDisplacement right) => !left.Equals(right);

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with multiplied components by a <see cref="double" />.
	/// </summary>
	public static PlaneDisplacement operator *(PlaneDisplacement displacement, double multiplier) => new(multiplier * displacement.X, multiplier * displacement.Y);

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with multiplied components by a <see cref="double" />.
	/// </summary>
	public static PlaneDisplacement operator *(double multiplier, PlaneDisplacement displacement) => displacement * multiplier;

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with multiplied components by an <see cref="int" />.
	/// </summary>
	public static PlaneDisplacement operator *(PlaneDisplacement displacement, int multiplier) => displacement * (double) multiplier;

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with multiplied components by an <see cref="int" />.
	/// </summary>
	public static PlaneDisplacement operator *(int multiplier, PlaneDisplacement displacement) => displacement * (double) multiplier;

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with subtracted components, in left argument's unit.
	/// </summary>
	public static PlaneDisplacement operator -(PlaneDisplacement left, PlaneDisplacement right) => new(left.X - right.X.ToUnit(left.Unit), left.Y - right.Y.ToUnit(left.Unit));

	/// <summary>
	///     Returns a <see cref="PlaneDisplacement" /> object with negated components.
	/// </summary>
	public static PlaneDisplacement operator -(PlaneDisplacement displacement) => new(-displacement.X, -displacement.Y);

	#endregion

}