using System;
using System.Linq;
using andrefmello91.Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;
using static andrefmello91.OnPlaneComponents.StressRelations;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Stress object for XY components.
	/// </summary>
	public partial struct StressState : IState<Pressure>, IUnitConvertible<PressureUnit>, IApproachable<IState<Pressure>, Pressure>, IEquatable<IState<Pressure>>, ICloneable<StressState>
	{

		#region Fields

		/// <summary>
		///     Get a <see cref="StressState" /> with zero elements.
		/// </summary>
		public static StressState Zero { get; } = new(0, 0, 0);

		/// <summary>
		///     The default tolerance for stresses.
		/// </summary>
		public static Pressure Tolerance { get; } = Pressure.FromPascals(1E-3);

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the stress unit (<see cref="PressureUnit" />).
		/// </summary>
		public PressureUnit Unit
		{
			get => SigmaX.Unit;
			set => ChangeUnit(value);
		}

		/// <inheritdoc />
		public bool IsHorizontal => ThetaX.ApproxZero() || ThetaX.Approx(Constants.Pi);

		/// <inheritdoc />
		public bool IsVertical => ThetaX.Approx(Constants.PiOver2) || ThetaX.Approx(Constants.Pi3Over2);

		/// <inheritdoc />
		public bool IsPrincipal => !IsXZero && !IsYZero && IsXYZero;

		/// <inheritdoc />
		public bool IsPureShear => IsXZero && IsYZero && !IsXYZero;

		/// <inheritdoc />
		public bool IsXZero => SigmaX.ApproxZero(Tolerance);

		/// <inheritdoc />
		public bool IsYZero => SigmaY.ApproxZero(Tolerance);

		/// <inheritdoc />
		public bool IsXYZero => TauXY.ApproxZero(Tolerance);

		Pressure IState<Pressure>.X => SigmaX;

		Pressure IState<Pressure>.Y => SigmaY;

		Pressure IState<Pressure>.XY => TauXY;

		/// <inheritdoc />
		public bool IsZero => IsXZero && IsYZero && IsXYZero;

		/// <inheritdoc />
		public double ThetaX { get; }

		/// <inheritdoc />
		public double ThetaY => ThetaX + Constants.PiOver2;

		/// <inheritdoc />
		public Matrix<double> TransformationMatrix { get; }

		/// <summary>
		///     Get the normal stress in X direction.
		/// </summary>
		public Pressure SigmaX { get; private set; }

		/// <summary>
		///     Get the normal stress in Y direction.
		/// </summary>
		public Pressure SigmaY { get; private set; }

		/// <summary>
		///     Get the shear stress.
		/// </summary>
		public Pressure TauXY { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		///     Stress object for XY components.
		/// </summary>
		/// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
		/// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
		/// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
		/// <param name="thetaX">
		///     The angle of <paramref name="sigmaX" />, related to horizontal axis (positive to
		///     counterclockwise).
		/// </param>
		/// <param name="unit">The <see cref="PressureUnit" /> of stresses (default: <see cref="PressureUnit.Megapascal" />).</param>
		public StressState(double sigmaX, double sigmaY, double tauXY, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
			: this((Pressure) sigmaX.As(unit), (Pressure) sigmaY.As(unit), (Pressure) tauXY.As(unit), thetaX)
		{
		}

		/// <inheritdoc cref="StressState(double, double, double, double, PressureUnit)" />
		public StressState(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, double thetaX = 0)
		{
			SigmaX               = sigmaX;
			SigmaY               = sigmaY.ToUnit(sigmaX.Unit);
			TauXY                = tauXY.ToUnit(sigmaX.Unit);
			ThetaX               = thetaX.AsFinite();
			TransformationMatrix = TransformationMatrix(thetaX);
		}

		private StressState(IState<Pressure> state)
			: this(state.X, state.Y, state.XY, state.ThetaX)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Get a <see cref="StressState" /> from a <see cref="Vector" />.
		/// </summary>
		/// <param name="stressVector">
		///     The vector of stresses, in considered <paramref name="unit" />.
		///     <para>{SigmaX, SigmaY, TauXY}</para>
		/// </param>
		/// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
		/// <param name="unit">The <see cref="PressureUnit" /> of stresses (default: <see cref="PressureUnit.Megapascal" />).</param>
		public static StressState FromVector(Vector<double> stressVector, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal) =>
			new(stressVector[0], stressVector[1], stressVector[2], thetaX, unit);

		/// <summary>
		///     Get a <see cref="StressState" /> from a <see cref="StrainState" />.
		/// </summary>
		/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
		/// <param name="stiffnessMatrix">
		///     The stiffness <see cref="Matrix" /> (3 x 3), related to <paramref name="strainState" />
		///     direction.
		/// </param>
		public static StressState FromStrains(StrainState strainState, Matrix<double> stiffnessMatrix) => strainState.IsZero ? Zero : FromVector(stiffnessMatrix * strainState.AsVector(), strainState.ThetaX);

		/// <summary>
		///     Get <see cref="StressState" /> transformed to horizontal direction (<see cref="ThetaX" /> = 0).
		/// </summary>
		/// <param name="stressState">The <see cref="StressState" /> to transform.</param>
		public static StressState ToHorizontal(IState<Pressure> stressState) =>
			Transform(stressState, -stressState.ThetaX);
		
		/// <summary>
		///     Get <see cref="StressState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="stressState">The <see cref="StressState" /> to transform.</param>
		/// <inheritdoc cref="Transform(double)" />
		public static StressState Transform(IState<Pressure> stressState, double theta)
		{
			var state = stressState is StressState stresses
				? stresses
				: new StressState(stressState);
			
			if (theta.ApproxZero(1E-6))
				return state;

			// Get the strain vector transformed
			var sVec = StressRelations.Transform(state.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, state.ThetaX + theta, state.Unit);
		}

		/// <summary>
		///     Get <see cref="StressState" /> from a <see cref="PrincipalStressState" /> in horizontal direction (
		///     <see cref="ThetaX" /> = 0).
		/// </summary>
		/// <param name="principalStressState">The <see cref="PrincipalStressState" /> to horizontal <see cref="StressState" />.</param>
		public static StressState FromPrincipal(PrincipalStressState principalStressState)
		{
			if (principalStressState.Theta1.ApproxZero())
				return FromVector(principalStressState.AsVector());

			// Get the strain vector transformed
			var sVec = StressesFromPrincipal(principalStressState.Sigma1, principalStressState.Sigma2, principalStressState.Theta1);

			// Return with corrected angle
			return FromVector(sVec, 0, principalStressState.Unit);
		}

		/// <summary>
		///     Get this <see cref="StressState" /> transformed to horizontal direction (<see cref="ThetaX" /> = 0).
		/// </summary>
		public StressState ToHorizontal() => ToHorizontal(this);
		
		IState<Pressure> IState<Pressure>.ToHorizontal() => ToHorizontal();

		/// <summary>
		///     Get this <see cref="StressState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
		public StressState Transform(double theta) => Transform(this, theta);
		
		IState<Pressure> IState<Pressure>.Transform(double theta) => Transform(theta);

		/// <summary>
		///     Get the <see cref="PrincipalStressState" /> related to this <see cref="StressState" />.
		/// </summary>
		public PrincipalStressState ToPrincipal() => PrincipalStressState.FromStress(this);
		
		IPrincipalState<Pressure> IState<Pressure>.ToPrincipal() => ToPrincipal();

		/// <summary>
		///     Change the <see cref="PressureUnit" /> of this <see cref="StressState" />.
		/// </summary>
		/// <param name="unit">The <see cref="PressureUnit" /> to convert.</param>
		public void ChangeUnit(PressureUnit unit)
		{
			if (Unit == unit)
				return;

			SigmaX = SigmaX.ToUnit(unit);
			SigmaY = SigmaY.ToUnit(unit);
			TauXY  = TauXY.ToUnit(unit);
		}

		/// <summary>
		///     Convert this <see cref="StressState" /> to another <see cref="PressureUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public StressState Convert(PressureUnit unit) => unit == Unit
			? this
			: new StressState(SigmaX.ToUnit(unit), SigmaY.ToUnit(unit), TauXY.ToUnit(unit), ThetaX);

		IUnitConvertible<PressureUnit> IUnitConvertible<PressureUnit>.Convert(PressureUnit unit) => Convert(unit);

		/// <summary>
		///     Get the stresses as an <see cref="Array" />.
		/// </summary>
		/// <remarks>
		///     { SigmaX, SigmaY, TauXY }
		/// </remarks>
		public Pressure[] AsArray() => new[] { SigmaX, SigmaY, TauXY };

		/// <summary>
		///     Get the stresses as <see cref="Vector" />, in a desired <see cref="PressureUnit" />.
		/// </summary>
		/// <remarks>
		///     { SigmaX, SigmaY, TauXY }
		/// </remarks>
		/// <param name="unit">The <see cref="PressureUnit" />.</param>
		public Vector<double> AsVector(PressureUnit unit = PressureUnit.Megapascal) => AsArray().Select(s => s.ToUnit(unit).Value).ToVector();

		/// <inheritdoc />
		public StressState Clone() => new(SigmaX, SigmaY, TauXY, ThetaX);

		/// <inheritdoc />
		public bool Approaches(IState<Pressure>? other, Pressure tolerance) =>
			other is not null &&
			ThetaX.Approx(other.ThetaX, 1E-3) && SigmaX.Approx(other.X, tolerance) &&
			SigmaY.Approx(other.Y, tolerance) && TauXY.Approx(other.XY, tolerance);

		/// <summary>
		///     Compare two <see cref="StressState" /> objects.
		/// </summary>
		/// <param name="other">The <see cref="StressState" /> to compare.</param>
		public bool Equals(IState<Pressure>? other) => Approaches(other, Tolerance);

		/// <inheritdoc />
		public override bool Equals(object? obj) =>
			obj switch
			{
				StressState other                    => Equals(other),
				PrincipalStressState principalStress => Equals(principalStress),
				_                                    => false
			};

		/// <inheritdoc />
		public override string ToString()
		{
			char
				sigma = (char) Characters.Sigma,
				tau   = (char) Characters.Tau,
				theta = (char) Characters.Theta;

			return
				$"{sigma}x = {SigmaX}\n" +
				$"{sigma}y = {SigmaY}\n" +
				$"{tau}xy = {TauXY}\n" +
				$"{theta}x = {ThetaX:0.00} rad";
		}

		/// <inheritdoc />
		public override int GetHashCode() => (int) (SigmaX.Value * SigmaY.Value * TauXY.Value);

		#endregion

	}
}