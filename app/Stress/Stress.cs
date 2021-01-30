using System;
using System.Linq;
using Extensions;
using Extensions.LinearAlgebra;
using Extensions.Number;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;
using static OnPlaneComponents.StressRelations;

namespace OnPlaneComponents
{
	/// <summary>
	///     Stress object for XY components.
	/// </summary>
	public partial struct StressState : IState<Pressure>, IApproachable<StressState, Pressure>, IApproachable<PrincipalStressState, Pressure>, IUnitConvertible<StressState, PressureUnit>, IEquatable<StressState>, IEquatable<PrincipalStressState>, ICloneable<StressState>
	{
		#region Fields

		/// <summary>
		///     Get a <see cref="StressState" /> with zero elements.
		/// </summary>
		public static readonly StressState Zero = new StressState(0, 0, 0);

		/// <summary>
		///     The default tolerance for stresses.
		/// </summary>
		public static readonly Pressure Tolerance = Pressure.FromPascals(1E-3);

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

		public bool IsHorizontal => ThetaX.ApproxZero() || ThetaX.Approx(Constants.Pi);

		public bool IsVertical => ThetaX.Approx(Constants.PiOver2) || ThetaX.Approx(Constants.Pi3Over2);

		public bool IsPrincipal => !IsXZero && !IsYZero && IsXYZero;

		public bool IsPureShear => IsXZero && IsYZero && !IsXYZero;

		public bool IsXZero => SigmaX.ApproxZero(Tolerance);

		public bool IsYZero => SigmaY.ApproxZero(Tolerance);

		public bool IsXYZero => TauXY.ApproxZero(Tolerance);

		Pressure IState<Pressure>.X => SigmaX;

		Pressure IState<Pressure>.Y => SigmaY;

		Pressure IState<Pressure>.XY => TauXY;

		public bool IsZero => IsXZero && IsYZero && IsXYZero;

		public double ThetaX { get; }

		public double ThetaY => ThetaX + Constants.PiOver2;

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
			: this (Pressure.From(sigmaX.ToZero(), unit), Pressure.From(sigmaY.ToZero(), unit), Pressure.From(tauXY.ToZero(), unit), thetaX)
		{
		}

		/// <inheritdoc cref="StressState(double, double, double, double, PressureUnit)" />
		public StressState(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, double thetaX = 0)
		{
			SigmaX               = sigmaX;
			SigmaY               = sigmaY.ToUnit(sigmaX.Unit);
			TauXY                = tauXY.ToUnit(sigmaX.Unit);
			ThetaX               = thetaX.ToZero();
			TransformationMatrix = TransformationMatrix(thetaX);
		}

		#endregion

		#region

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
			new StressState(stressVector[0], stressVector[1], stressVector[2], thetaX, unit);

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
		public static StressState ToHorizontal(StressState stressState)
		{
			if (stressState.IsHorizontal)
				return stressState;

			// Get the strain vector transformed
			var sVec = StressRelations.Transform(stressState.AsVector(), -stressState.ThetaX);

			// Return with corrected angle
			return FromVector(sVec, 0, stressState.Unit);
		}

		/// <summary>
		///     Get <see cref="StressState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="stressState">The <see cref="StressState" /> to transform.</param>
		/// <inheritdoc cref="Transform(double)" />
		public static StressState Transform(StressState stressState, double theta)
		{
			if (theta.ApproxZero())
				return stressState;

			// Get the strain vector transformed
			var sVec = StressRelations.Transform(stressState.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, stressState.ThetaX + theta, stressState.Unit);
		}

		/// <summary>
		///     Get <see cref="PrincipalStressState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="principalStressState">The <see cref="PrincipalStressState" /> to transform.</param>
		/// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
		public static StressState Transform(PrincipalStressState principalStressState, double theta)
		{
			if (theta.ApproxZero())
				return FromVector(principalStressState.AsVector(), principalStressState.Theta1, principalStressState.Unit);

			// Get the strain vector transformed
			var sVec = StressRelations.Transform(principalStressState.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, principalStressState.Theta1 + theta, principalStressState.Unit);
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

		/// <summary>
		///     Get this <see cref="StressState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
		public StressState Transform(double theta) => Transform(this, theta);

		/// <summary>
		///     Get the <see cref="PrincipalStressState" /> related to this <see cref="StressState" />.
		/// </summary>
		public PrincipalStressState ToPrincipal() => PrincipalStressState.FromStress(this);

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

		/// <summary>
		///     Get the stresses as an <see cref="Array" />.
		/// </summary>
		/// <remarks>
		///		{ SigmaX, SigmaY, TauXY }
		/// </remarks>
		public Pressure[] AsArray() => new[] { SigmaX, SigmaY, TauXY };

		/// <summary>
		///     Get the stresses as <see cref="Vector" />, in current (<see cref="Unit" />).
		/// </summary>
		/// <remarks>
		///		{ SigmaX, SigmaY, TauXY }
		/// </remarks>
		public Vector<double> AsVector() => AsArray().Select(s => s.Value).ToVector();

		public StressState Clone() => new StressState(SigmaX, SigmaY, TauXY, ThetaX);

		public bool Approaches(StressState other, Pressure tolerance) =>
			ThetaX.Approx(other.ThetaX, 1E-3)      && SigmaX.Approx(other.SigmaX, tolerance) &&
			SigmaY.Approx(other.SigmaY, tolerance) &&  TauXY.Approx(other.TauXY,  tolerance);

		public bool Approaches(PrincipalStressState other, Pressure tolerance) => Approaches(FromPrincipal(other), tolerance);

		IPrincipalState<Pressure> IState<Pressure>.ToPrincipal() => ToPrincipal();

		/// <summary>
		///     Compare a <see cref="StressState" /> to a <see cref="PrincipalStressState" /> object.
		/// </summary>
		/// <param name="other">The <see cref="PrincipalStressState" /> to compare.</param>
		public bool Equals(PrincipalStressState other) => Approaches(other, Tolerance);

		/// <summary>
		///     Compare two <see cref="StressState" /> objects.
		/// </summary>
		/// <param name="other">The <see cref="StressState" /> to compare.</param>
		public bool Equals(StressState other) => Approaches(other, Tolerance);

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case StressState other:
					return Equals(other);

				case PrincipalStressState principalStress:
					return Equals(principalStress);

				default:
					return false;
			}
		}

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

		public override int GetHashCode() => (int) (SigmaX.Value * SigmaY.Value * TauXY.Value);

		#endregion
	}
}