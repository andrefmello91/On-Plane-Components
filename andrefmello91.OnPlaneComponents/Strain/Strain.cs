using System;
using andrefmello91.Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using static andrefmello91.OnPlaneComponents.StrainRelations;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Strain struct for XY components.
	/// </summary>
	public readonly partial struct StrainState : IState<double>, IApproachable<StrainState, double>, IApproachable<PrincipalStrainState, double>, IEquatable<StrainState>, IEquatable<PrincipalStrainState>, ICloneable<StrainState>
	{

		#region Fields

		/// <summary>
		///     Get a <see cref="StrainState" /> with zero elements.
		/// </summary>
		public static readonly StrainState Zero = new(0, 0, 0);

		/// <summary>
		///     The default tolerance for strains.
		/// </summary>
		public static readonly double Tolerance = 1E-12;

		#endregion

		#region Properties

		double IState<double>.X => EpsilonX;

		double IState<double>.Y => EpsilonY;

		double IState<double>.XY => GammaXY;

		/// <summary>
		///     Returns true if <see cref="EpsilonX" /> is zero.
		/// </summary>
		public bool IsXZero => EpsilonX.ApproxZero();

		/// <summary>
		///     Returns true if <see cref="EpsilonY" /> is zero.
		/// </summary>
		public bool IsYZero => EpsilonY.ApproxZero();

		/// <summary>
		///     Returns true if <see cref="GammaXY" /> is zero.
		/// </summary>
		public bool IsXYZero => GammaXY.ApproxZero();

		/// <inheritdoc />
		public bool IsHorizontal => ThetaX.ApproxZero() || ThetaX.Approx(Constants.Pi);

		/// <inheritdoc />
		public bool IsVertical => ThetaX.Approx(Constants.PiOver2) || ThetaX.Approx(Constants.Pi3Over2);

		/// <inheritdoc />
		public bool IsPrincipal => !IsXZero && !IsYZero && IsXYZero;

		/// <inheritdoc />
		public bool IsPureShear => IsXZero && IsYZero && !IsXYZero;

		/// <inheritdoc />
		public bool IsZero => IsXZero && IsYZero && IsXYZero;

		/// <inheritdoc />
		public double ThetaX { get; }

		/// <inheritdoc />
		public double ThetaY => ThetaX + Constants.PiOver2;

		/// <inheritdoc />
		public Matrix<double> TransformationMatrix { get; }

		/// <summary>
		///     Get the normal strain in X direction.
		/// </summary>
		public double EpsilonX { get; }

		/// <summary>
		///     Get the normal strain in Y direction.
		/// </summary>
		public double EpsilonY { get; }

		/// <summary>
		///     Get the shear strain.
		/// </summary>
		public double GammaXY { get; }

		#endregion

		#region Constructors

		/// <summary>
		///     Strain object for XY components.
		/// </summary>
		/// <param name="epsilonX">The normal strain in X direction (positive for tensile).</param>
		/// <param name="epsilonY">The normal strain in Y direction (positive for tensile).</param>
		/// <param name="gammaXY">The shear strain (positive if right face of element displaces upwards).</param>
		/// <param name="thetaX">
		///     The angle of <paramref name="epsilonX" /> direction, related to horizontal axis (positive
		///     counterclockwise).
		/// </param>
		public StrainState(double epsilonX, double epsilonY, double gammaXY, double thetaX = 0)
		{
			EpsilonX             = epsilonX.AsFinite();
			EpsilonY             = epsilonY.AsFinite();
			GammaXY              = gammaXY.AsFinite();
			ThetaX               = thetaX.AsFinite();
			TransformationMatrix = TransformationMatrix(thetaX);
		}

		#endregion

		#region

		/// <summary>
		///     Get a <see cref="StrainState" /> from a <see cref="Vector" /> of strains.
		/// </summary>
		/// <param name="strainVector">
		///     The <see cref="Vector" /> of strains.
		///     <para>{EpsilonX, EpsilonY, GammaXY}</para>
		/// </param>
		/// <param name="thetaX">The angle of X direction, related to horizontal axis (positive counterclockwise).</param>
		public static StrainState FromVector(Vector<double> strainVector, double thetaX = 0) =>
			new(strainVector[0], strainVector[1], strainVector[2], thetaX);

		/// <summary>
		///     Get a <see cref="StrainState" /> from a <see cref="StressState" />.
		/// </summary>
		/// <param name="stressState">The <see cref="StressState" /> to transform.</param>
		/// <param name="stiffnessMatrix">
		///     The stiffness <see cref="Matrix" /> (3 x 3), related to <paramref name="stressState" /> direction, with elements in
		///     MPa.
		/// </param>
		public static StrainState FromStresses(StressState stressState, Matrix<double> stiffnessMatrix) => stressState.IsZero ? Zero : FromVector(stiffnessMatrix.Solve(stressState.AsVector()), stressState.ThetaX);

		/// <summary>
		///     Get <see cref="StrainState" /> transformed to horizontal direction (<see cref="ThetaX" /> = 0).
		/// </summary>
		/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
		public static StrainState ToHorizontal(StrainState strainState)
		{
			if (strainState.IsHorizontal)
				return strainState;

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(strainState.AsVector(), -strainState.ThetaX);

			// Return with corrected angle
			return FromVector(sVec);
		}

		/// <summary>
		///     Get <see cref="StrainState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
		/// <inheritdoc cref="Transform(double)" />
		public static StrainState Transform(StrainState strainState, double theta)
		{
			if (theta.ApproxZero())
				return strainState;

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(strainState.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, strainState.ThetaX + theta);
		}

		/// <summary>
		///     Get <see cref="StrainState" /> transformed by a rotation angle.
		/// </summary>
		/// <param name="principalStrainState">The <see cref="PrincipalStrainState" /> to transform.</param>
		/// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
		public static StrainState Transform(PrincipalStrainState principalStrainState, double theta)
		{
			if (theta.ApproxZero())
				return FromVector(principalStrainState.AsVector(), principalStrainState.Theta1);

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(principalStrainState.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, principalStrainState.Theta1 + theta);
		}

		/// <summary>
		///     Get <see cref="StrainState" /> from a <see cref="PrincipalStrainState" /> in horizontal direction (
		///     <see cref="ThetaX" /> = 0).
		/// </summary>
		/// <param name="principalStrainState">The <see cref="PrincipalStrainState" /> to horizontal <see cref="StrainState" />.</param>
		public static StrainState FromPrincipal(PrincipalStrainState principalStrainState)
		{
			if (principalStrainState.Theta1.ApproxZero())
				return FromVector(principalStrainState.AsVector());

			// Get the strain vector transformed
			var sVec = StrainsFromPrincipal(principalStrainState.Epsilon1, principalStrainState.Epsilon2, principalStrainState.Theta1);

			// Return with corrected angle
			return FromVector(sVec);
		}

		/// <summary>
		///     Get this <see cref="StrainState" /> transformed to horizontal direction (<see cref="ThetaX" /> = 0).
		/// </summary>
		public IState<double> ToHorizontal() => ToHorizontal(this);

		/// <summary>
		///     Get this <see cref="StrainState" /> transformed by a rotation angle.
		/// </summary>
		/// <inheritdoc cref="IState{T}.Transform" />
		public IState<double> Transform(double rotationAngle) => Transform(this, rotationAngle);

		/// <summary>
		///     Get strains as an <see cref="Array" />.
		/// </summary>
		/// <remarks>
		///     { EpsilonX, EpsilonY, GammaXY }
		/// </remarks>
		public double[] AsArray() => new[] { EpsilonX, EpsilonY, GammaXY };

		/// <summary>
		///     Get strains as a <see cref="Vector" />.
		/// </summary>
		/// <remarks>
		///     { EpsilonX, EpsilonY, GammaXY }
		/// </remarks>
		public Vector<double> AsVector() => AsArray().ToVector();

		/// <summary>
		///     Get the <see cref="PrincipalStrainState" /> related to this <see cref="StrainState" />.
		/// </summary>
		public IPrincipalState<double> ToPrincipal() => PrincipalStrainState.FromStrain(this);

		/// <inheritdoc />
		public bool Approaches(StrainState other, double tolerance) =>
			ThetaX.Approx(other.ThetaX, tolerance) && EpsilonX.Approx(other.EpsilonX, tolerance) &&
			EpsilonY.Approx(other.EpsilonY, tolerance) && GammaXY.Approx(other.GammaXY, tolerance);


		/// <inheritdoc />
		public bool Approaches(PrincipalStrainState other, double tolerance) => Approaches(FromPrincipal(other), tolerance);

		/// <inheritdoc />
		public StrainState Clone() => new(EpsilonX, EpsilonY, GammaXY, ThetaX);

		/// <summary>
		///     Compare a <see cref="StrainState" /> to a <see cref="PrincipalStrainState" /> object.
		/// </summary>
		/// <param name="other">The <see cref="PrincipalStrainState" /> to compare.</param>
		public bool Equals(PrincipalStrainState other) => Approaches(other, Tolerance);

		/// <summary>
		///     Compare two <see cref="StrainState" /> objects.
		/// </summary>
		/// <param name="other">The strain to compare.</param>
		public bool Equals(StrainState other) => Approaches(other, Tolerance);

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			switch (obj)
			{
				case StrainState other:
					return Equals(other);

				case PrincipalStrainState principalStrain:
					return Equals(principalStrain);

				default:
					return false;
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			char
				epsilon = (char) Characters.Epsilon,
				gamma   = (char) Characters.Gamma,
				theta   = (char) Characters.Theta;


			return
				$"{epsilon}x = {EpsilonX:0.##E+00}\n" +
				$"{epsilon}y = {EpsilonY:0.##E+00}\n" +
				$"{gamma}xy = {GammaXY:0.##E+00}\n" +
				$"{theta}x = {ThetaX:0.00} rad";
		}

		/// <inheritdoc />
		public override int GetHashCode() => (int) (EpsilonX * EpsilonY * GammaXY);

		#endregion

	}
}