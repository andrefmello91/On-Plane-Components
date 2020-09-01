namespace OnPlaneComponents
{
    public partial struct StressState
    {
        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (StressState left, StressState right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (StressState left, StressState right) => !left.Equals(right);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (StressState left, PrincipalStressState right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (StressState left, PrincipalStressState right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="StressState"/> object with summed components, in left argument's <see cref="Unit"/> and horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator + (StressState left, StressState right)
        {
	        // Transform to horizontal
	        StressState
		        lTrans = ToHorizontal(left),
		        rTrans = ToHorizontal(right);

            return new StressState(lTrans._sigmaX + rTrans._sigmaX, lTrans._sigmaY + rTrans._sigmaY, lTrans._tauXY + rTrans._tauXY, 0, left.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with subtracted components, in left argument's <see cref="Unit"/> and horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator - (StressState left, StressState right)
        {
	        // Transform to horizontal
	        StressState
		        lTrans = ToHorizontal(left),
		        rTrans = ToHorizontal(right);

	        return new StressState(lTrans._sigmaX - rTrans._sigmaX, lTrans._sigmaY - rTrans._sigmaY, lTrans._tauXY - rTrans._tauXY, 0, left.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with components multiplied by -1, in horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator - (StressState right)
        {
	        // Transform to horizontal
	        var rTrans = ToHorizontal(right);

	        return FromVector(-rTrans.AsVector(), 0, right.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with summed components, in left argument's <see cref="Unit"/> and horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator + (StressState left, PrincipalStressState right)
        {
	        // Transform to horizontal
	        StressState
		        lTrans = ToHorizontal(left),
		        rTrans = FromPrincipal(right);

            return new StressState(lTrans._sigmaX + rTrans._sigmaX, lTrans._sigmaY + rTrans._sigmaY, lTrans._tauXY + rTrans._tauXY, 0, left.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with subtracted components, in left argument's <see cref="Unit"/> and horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator - (StressState left, PrincipalStressState right)
        {
	        // Transform to horizontal
	        StressState
		        lTrans = ToHorizontal(left),
		        rTrans = FromPrincipal(right);

	        return new StressState(lTrans._sigmaX - rTrans._sigmaX, lTrans._sigmaY - rTrans._sigmaY, lTrans._tauXY - rTrans._tauXY, 0, left.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with summed components, in left argument's <see cref="Unit"/> and horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator + (PrincipalStressState left, StressState right)
        {
	        // Transform to horizontal
	        StressState
		        lTrans = FromPrincipal(left),
		        rTrans = ToHorizontal(right);

            return new StressState(lTrans._sigmaX + rTrans._sigmaX, lTrans._sigmaY + rTrans._sigmaY, lTrans._tauXY + rTrans._tauXY, 0, left.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with subtracted components, in left argument's <see cref="Unit"/> and horizontal axis (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static StressState operator - (PrincipalStressState left, StressState right)
        {
	        // Transform to horizontal
	        StressState
		        lTrans = FromPrincipal(left),
		        rTrans = ToHorizontal(right);

	        return new StressState(lTrans._sigmaX - rTrans._sigmaX, lTrans._sigmaY - rTrans._sigmaY, lTrans._tauXY - rTrans._tauXY, 0, left.Unit);
        }

        /// <summary>
        /// Returns a <see cref="StressState"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static StressState operator * (StressState stressState, double multiplier) => new StressState(multiplier * stressState._sigmaX, multiplier * stressState._sigmaY, multiplier * stressState._tauXY, stressState.ThetaX, stressState.Unit);

        /// <summary>
        /// Returns a <see cref="StressState"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static StressState operator * (double multiplier, StressState stressState) => stressState * multiplier;

        /// <summary>
        /// Returns a <see cref="StressState"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static StressState operator * (StressState stressState, int multiplier) => stressState * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="StressState"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static StressState operator * (int multiplier, StressState stressState) => stressState * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="StressState"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static StressState operator / (StressState stressState, double divider) => new StressState(stressState._sigmaX / divider, stressState._sigmaY / divider, stressState._tauXY / divider, stressState.ThetaX, stressState.Unit);

        /// <summary>
        /// Returns a <see cref="StressState"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static StressState operator / (StressState stressState, int divider) => stressState / (double)divider;
	}
}
