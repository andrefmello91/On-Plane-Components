namespace andrefmello91.OnPlaneComponents.Stress
{
	public partial struct PrincipalStressState
    {
        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStressState left, PrincipalStressState right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStressState left, PrincipalStressState right) => !left.Equals(right);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStressState left, StressState right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStressState left, StressState right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="StressState"/> object with summed components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator + (PrincipalStressState left, PrincipalStressState right) => StressState.FromPrincipal(left) + StressState.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="StressState"/> object with subtracted components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator - (PrincipalStressState left, PrincipalStressState right) => StressState.FromPrincipal(left) - StressState.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="PrincipalStressState"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStressState operator * (PrincipalStressState principalStrain, double multiplier) => new PrincipalStressState(multiplier * principalStrain.Sigma1, multiplier * principalStrain.Sigma2, principalStrain.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStressState"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStressState operator *(double multiplier, PrincipalStressState stressState) => stressState * multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStressState"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStressState operator *(PrincipalStressState principalStrain, int multiplier) => principalStrain * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStressState"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStressState operator *(int multiplier, PrincipalStressState stressState) => stressState * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStressState"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStressState operator /(PrincipalStressState principalStrain, double divider) => new PrincipalStressState(principalStrain.Sigma1 / divider, principalStrain.Sigma2 / divider, principalStrain.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStressState"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStressState operator /(PrincipalStressState principalStrain, int divider) => principalStrain / (double)divider;

    }
}
