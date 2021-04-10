namespace andrefmello91.OnPlaneComponents
{
	public partial struct StrainState
	{
		/// <summary>
		///     Returns true if components are equal.
		/// </summary>
		public static bool operator ==(StrainState left, StrainState right) => left.Equals(right);

		/// <summary>
		///     Returns true if components are different.
		/// </summary>
		public static bool operator !=(StrainState left, StrainState right) => !left.Equals(right);

		/// <summary>
		///     Returns true if components are equal.
		/// </summary>
		public static bool operator ==(StrainState left, PrincipalStrainState right) => left.Equals(right);

		/// <summary>
		///     Returns true if components are different.
		/// </summary>
		public static bool operator !=(StrainState left, PrincipalStrainState right) => !left.Equals(right);

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with summed components, in horizontal direction (<see cref="ThetaX" />
		///     = 0).
		/// </summary>
		public static StrainState operator +(StrainState left, StrainState right)
		{
			// Transform to horizontal
			StrainState
				lTrans = ToHorizontal(left),
				rTrans = ToHorizontal(right);

			return FromVector(lTrans.AsVector() + rTrans.AsVector());
		}

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with subtracted components, in horizontal direction (
		///     <see cref="ThetaX" /> = 0).
		/// </summary>
		public static StrainState operator -(StrainState left, StrainState right)
		{
			// Transform to horizontal
			StrainState
				lTrans = ToHorizontal(left),
				rTrans = ToHorizontal(right);

			return FromVector(lTrans.AsVector() - rTrans.AsVector());
		}

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with summed components, in horizontal direction (<see cref="ThetaX" />
		///     = 0).
		/// </summary>
		public static StrainState operator +(StrainState left, PrincipalStrainState right)
		{
			// Transform to horizontal
			StrainState
				lTrans = ToHorizontal(left),
				rTrans = FromPrincipal(right);

			return FromVector(lTrans.AsVector() + rTrans.AsVector());
		}

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with subtracted components, in horizontal direction (
		///     <see cref="ThetaX" /> = 0).
		/// </summary>
		public static StrainState operator -(StrainState left, PrincipalStrainState right)
		{
			// Transform to horizontal
			StrainState
				lTrans = ToHorizontal(left),
				rTrans = FromPrincipal(right);

			return FromVector(lTrans.AsVector() - rTrans.AsVector());
		}

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with summed components, in horizontal direction (<see cref="ThetaX" />
		///     = 0).
		/// </summary>
		public static StrainState operator +(PrincipalStrainState left, StrainState right)
		{
			// Transform to horizontal
			StrainState
				lTrans = FromPrincipal(left),
				rTrans = ToHorizontal(right);

			return FromVector(lTrans.AsVector() + rTrans.AsVector());
		}

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with subtracted components, in horizontal direction (
		///     <see cref="ThetaX" /> = 0).
		/// </summary>
		public static StrainState operator -(PrincipalStrainState left, StrainState right)
		{
			// Transform to horizontal
			StrainState
				lTrans = FromPrincipal(left),
				rTrans = ToHorizontal(right);

			return FromVector(lTrans.AsVector() - rTrans.AsVector());
		}

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with multiplied components by a <see cref="double" />.
		/// </summary>
		public static StrainState operator *(StrainState strainState, double multiplier) => FromVector(multiplier * strainState.AsVector(), strainState.ThetaX);

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with multiplied components by a <see cref="double" />.
		/// </summary>
		public static StrainState operator *(double multiplier, StrainState strainState) => strainState * multiplier;

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with multiplied components by an <see cref="int" />.
		/// </summary>
		public static StrainState operator *(StrainState strainState, int multiplier) => strainState * (double) multiplier;

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with multiplied components by an <see cref="int" />.
		/// </summary>
		public static StrainState operator *(int multiplier, StrainState strainState) => strainState * (double) multiplier;

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with components divided by a <see cref="double" />.
		/// </summary>
		public static StrainState operator /(StrainState strainState, double divider) => FromVector(strainState.AsVector() / divider, strainState.ThetaX);

		/// <summary>
		///     Returns a <see cref="StrainState" /> object with components divided by an <see cref="int" />.
		/// </summary>
		public static StrainState operator /(StrainState strainState, int divider) => strainState / (double) divider;
		
		/// <summary>
		///		Convert a <see cref="PrincipalStrainState"/> into a <see cref="StrainState"/>.
		/// </summary>
		/// <remarks>
		///		See: <see cref="PrincipalStrainState.AsStrainState"/>.
		/// </remarks>
		public static explicit operator StrainState(PrincipalStrainState principalStressState) => principalStressState.AsStrainState();

	}
}