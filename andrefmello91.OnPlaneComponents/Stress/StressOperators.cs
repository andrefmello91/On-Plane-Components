using UnitsNet;

namespace andrefmello91.OnPlaneComponents
{
	public partial struct StressState
	{
		/// <summary>
		///     Returns true if components are equal.
		/// </summary>
		public static bool operator ==(StressState left, IState<Pressure>? right) => left.Equals(right);

		/// <summary>
		///     Returns true if components are different.
		/// </summary>
		public static bool operator !=(StressState left, IState<Pressure>? right) => !left.Equals(right);

		/// <summary>
		///     Returns a <see cref="StressState" /> object with summed components, in left argument's <see cref="Unit" /> and
		///     horizontal axis (<see cref="ThetaX" /> = 0).
		/// </summary>
		public static StressState operator +(StressState left, IState<Pressure>? right)
		{
			if (right is null)
				return left;
			
			// Transform to horizontal
			StressState
				lTrans = ToHorizontal(left),
				rTrans = ToHorizontal(right);

			return new StressState(lTrans.SigmaX + rTrans.SigmaX.ToUnit(left.Unit), lTrans.SigmaY + rTrans.SigmaY.ToUnit(left.Unit), lTrans.TauXY + rTrans.TauXY.ToUnit(left.Unit));
		}

		/// <summary>
		///     Returns a <see cref="StressState" /> object with subtracted components, in left argument's <see cref="Unit" /> and
		///     horizontal axis (<see cref="ThetaX" /> = 0).
		/// </summary>
		public static StressState operator -(StressState left, IState<Pressure>? right)
		{
			if (right is null)
				return left;

			// Transform to horizontal
			StressState
				lTrans = ToHorizontal(left),
				rTrans = ToHorizontal(right);

			return new StressState(lTrans.SigmaX - rTrans.SigmaX.ToUnit(left.Unit), lTrans.SigmaY - rTrans.SigmaY.ToUnit(left.Unit), lTrans.TauXY - rTrans.TauXY.ToUnit(left.Unit));
		}

		/// <summary>
		///     Returns a <see cref="StressState" /> object with components multiplied by -1, in horizontal axis (
		///     <see cref="ThetaX" /> = 0).
		/// </summary>
		public static StressState operator -(StressState right)
		{
			// Transform to horizontal
			var rTrans = ToHorizontal(right);

			return FromVector(-rTrans.AsVector(), 0, right.Unit);
		}

		/// <summary>
		///     Returns a <see cref="StressState" /> object with multiplied components by a <see cref="double" />.
		/// </summary>
		public static StressState operator *(StressState stressState, double multiplier) => new(multiplier * stressState.SigmaX, multiplier * stressState.SigmaY, multiplier * stressState.TauXY, stressState.ThetaX);

		/// <summary>
		///     Returns a <see cref="StressState" /> object with multiplied components by a <see cref="double" />.
		/// </summary>
		public static StressState operator *(double multiplier, StressState stressState) => stressState * multiplier;

		/// <summary>
		///     Returns a <see cref="StressState" /> object with multiplied components by an <see cref="int" />.
		/// </summary>
		public static StressState operator *(StressState stressState, int multiplier) => stressState * (double) multiplier;

		/// <summary>
		///     Returns a <see cref="StressState" /> object with multiplied components by an <see cref="int" />.
		/// </summary>
		public static StressState operator *(int multiplier, StressState stressState) => stressState * (double) multiplier;

		/// <summary>
		///     Returns a <see cref="StressState" /> object with components divided by a <see cref="double" />.
		/// </summary>
		public static StressState operator /(StressState stressState, double divider) => new(stressState.SigmaX / divider, stressState.SigmaY / divider, stressState.TauXY / divider, stressState.ThetaX);

		/// <summary>
		///     Returns a <see cref="StressState" /> object with components divided by an <see cref="int" />.
		/// </summary>
		public static StressState operator /(StressState stressState, int divider) => stressState / (double) divider;

		/// <summary>
		///     Convert a <see cref="PrincipalStressState" /> into a <see cref="StressState" />.
		/// </summary>
		/// <remarks>
		///     See: <see cref="PrincipalStressState.AsStressState" />.
		/// </remarks>
		public static explicit operator StressState(PrincipalStressState principalStressState) => principalStressState.AsStressState();

	}
}