namespace andrefmello91.OnPlaneComponents;

public partial struct PrincipalStrainState
{

	#region Operators

	/// <summary>
	///     Returns a <see cref="StrainState" /> object with summed components, in horizontal direction (
	///     <see cref="StrainState.ThetaX" /> = 0).
	/// </summary>
	public static StrainState operator +(PrincipalStrainState left, IState<double>? right) =>
		right is not null
			? StrainState.FromPrincipal(left) + right.ToHorizontal()
			: (StrainState) left;

	/// <summary>
	///     Returns a <see cref="PrincipalStrainState" /> object with components divided by a <see cref="double" />.
	/// </summary>
	public static PrincipalStrainState operator /(PrincipalStrainState principalStrainState, double divider) => new(principalStrainState.Epsilon1 / divider, principalStrainState.Epsilon2 / divider, principalStrainState.Theta1);

	/// <summary>
	///     Returns a <see cref="PrincipalStrainState" /> object with components divided by an <see cref="int" />.
	/// </summary>
	public static PrincipalStrainState operator /(PrincipalStrainState principalStrainState, int divider) => principalStrainState / (double) divider;

	/// <summary>
	///     Returns true if components are equal.
	/// </summary>
	public static bool operator ==(PrincipalStrainState left, IState<double>? right) => left.Equals(right);

	/// <summary>
	///     Convert a <see cref="StrainState" /> into a <see cref="PrincipalStrainState" />.
	/// </summary>
	/// <remarks>
	///     See: <see cref="StrainState.ToPrincipal" />.
	/// </remarks>
	public static explicit operator PrincipalStrainState(StrainState state) => state.ToPrincipal();

	/// <summary>
	///     Returns true if components are different.
	/// </summary>
	public static bool operator !=(PrincipalStrainState left, IState<double>? right) => !left.Equals(right);

	/// <summary>
	///     Returns a <see cref="PrincipalStrainState" /> object with multiplied components by a <see cref="double" />.
	/// </summary>
	public static PrincipalStrainState operator *(PrincipalStrainState principalStrainState, double multiplier) => new(multiplier * principalStrainState.Epsilon1, multiplier * principalStrainState.Epsilon2, principalStrainState.Theta1);

	/// <summary>
	///     Returns a <see cref="PrincipalStrainState" /> object with multiplied components by a <see cref="double" />.
	/// </summary>
	public static PrincipalStrainState operator *(double multiplier, PrincipalStrainState strainState) => strainState * multiplier;

	/// <summary>
	///     Returns a <see cref="PrincipalStrainState" /> object with multiplied components by an <see cref="int" />.
	/// </summary>
	public static PrincipalStrainState operator *(PrincipalStrainState principalStrainState, int multiplier) => principalStrainState * (double) multiplier;

	/// <summary>
	///     Returns a <see cref="PrincipalStrainState" /> object with multiplied components by an <see cref="int" />.
	/// </summary>
	public static PrincipalStrainState operator *(int multiplier, PrincipalStrainState strainState) => strainState * (double) multiplier;

	/// <summary>
	///     Returns a <see cref="StrainState" /> object with subtracted components, in horizontal direction (
	///     <see cref="StrainState.ThetaX" /> = 0).
	/// </summary>
	public static StrainState operator -(PrincipalStrainState left, IState<double>? right) =>
		right is not null
			? StrainState.FromPrincipal(left) - right.ToHorizontal()
			: (StrainState) left;

	#endregion

}