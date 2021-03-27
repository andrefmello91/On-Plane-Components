using UnitsNet;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///		OnPlaneComponents extensions.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		///		Convert this <paramref name="planeComponent"/> to a <see cref="PlaneForce"/>.
		/// </summary>
		public static PlaneForce ToPlaneForce(this IPlaneComponent<Force> planeComponent) => planeComponent is PlaneForce planeForce
			? planeForce
			: new PlaneForce(planeComponent.X, planeComponent.Y);
		
		/// <summary>
		///		Convert this <paramref name="planeComponent"/> to a <see cref="PlaneDisplacement"/>.
		/// </summary>
		public static PlaneDisplacement ToPlaneDisplacement(this IPlaneComponent<Length> planeComponent) => planeComponent is PlaneDisplacement planeDisplacement
			? planeDisplacement
			: new PlaneDisplacement(planeComponent.X, planeComponent.Y);
		
		/// <summary>
		///		Convert this <paramref name="state"/> to a <see cref="StressState"/>.
		/// </summary>
		public static StressState ToStressState(this IState<Pressure> state) => state is StressState stressState
			? stressState
			: new StressState(state.X, state.Y, state.XY, state.ThetaX);
		
		/// <summary>
		///		Convert this <paramref name="principalState"/> to a <see cref="PrincipalStressState"/>.
		/// </summary>
		public static PrincipalStressState ToPrincipalStressState(this IPrincipalState<Pressure> principalState) => principalState is PrincipalStressState principalStressState
			? principalStressState
			: new PrincipalStressState(principalState.S1, principalState.S2, principalState.Theta1);
		
		/// <summary>
		///		Convert this <paramref name="state"/> to a <see cref="StrainState"/>.
		/// </summary>
		public static StrainState ToStrainState(this IState<double> state) => state is StrainState strainState
			? strainState
			: new StrainState(state.X, state.Y, state.XY, state.ThetaX);
		
		/// <summary>
		///		Convert this <paramref name="principalState"/> to a <see cref="PrincipalStrainState"/>.
		/// </summary>
		public static PrincipalStrainState ToPrincipalStrainState(this IPrincipalState<double> principalState) => principalState is PrincipalStrainState principalStrainState
			? principalStrainState
			: new PrincipalStrainState(principalState.S1, principalState.S2, principalState.Theta1);
	}
}