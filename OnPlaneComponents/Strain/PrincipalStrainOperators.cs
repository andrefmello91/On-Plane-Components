namespace OnPlaneComponents
{
	public partial struct PrincipalStrainState
    {
        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStrainState left, PrincipalStrainState right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStrainState left, PrincipalStrainState right) => !left.Equals(right);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStrainState left, StrainState right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStrainState left, StrainState right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="StrainState"/> object with summed components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StrainState operator + (PrincipalStrainState left, PrincipalStrainState right) => StrainState.FromPrincipal(left) + StrainState.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="StrainState"/> object with subtracted components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StrainState operator - (PrincipalStrainState left, PrincipalStrainState right) => StrainState.FromPrincipal(left) - StrainState.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="PrincipalStrainState"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStrainState operator * (PrincipalStrainState principalStrainState, double multiplier) => new PrincipalStrainState(multiplier * principalStrainState.Epsilon1, multiplier * principalStrainState.Epsilon2, principalStrainState.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStrainState"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStrainState operator *(double multiplier, PrincipalStrainState strainState) => strainState * multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStrainState"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStrainState operator *(PrincipalStrainState principalStrainState, int multiplier) => principalStrainState * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStrainState"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStrainState operator *(int multiplier, PrincipalStrainState strainState) => strainState * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStrainState"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStrainState operator /(PrincipalStrainState principalStrainState, double divider) => new PrincipalStrainState(principalStrainState.Epsilon1 / divider, principalStrainState.Epsilon2 / divider, principalStrainState.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStrainState"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStrainState operator /(PrincipalStrainState principalStrainState, int divider) => principalStrainState / (double)divider;

    }
}
