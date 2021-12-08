using System;
using andrefmello91.Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using static andrefmello91.OnPlaneComponents.StrainRelations;

namespace andrefmello91.OnPlaneComponents;

/// <summary>
///     Strain struct for XY components.
/// </summary>
public readonly partial struct StrainState : IState<double>, IApproachable<IState<double>, double>, IEquatable<IState<double>>, ICloneable<StrainState>
{

	#region Properties

	/// <summary>
	///     The default tolerance for strains.
	/// </summary>
	public static double Tolerance => 1E-12;

	/// <summary>
	///     Get a <see cref="StrainState" /> with zero elements.
	/// </summary>
	public static StrainState Zero { get; } = new(0, 0, 0);

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

	/// <inheritdoc />
	public bool IsHorizontal => ThetaX.ApproxZero() || ThetaX.Approx(Constants.Pi);

	/// <inheritdoc />
	public bool IsPrincipal => !IsXZero && !IsYZero && IsXYZero;

	/// <inheritdoc />
	public bool IsPureShear => IsXZero && IsYZero && !IsXYZero;

	/// <inheritdoc />
	public bool IsVertical => ThetaX.Approx(Constants.PiOver2) || ThetaX.Approx(Constants.Pi3Over2);

	/// <summary>
	///     Returns true if <see cref="GammaXY" /> is zero.
	/// </summary>
	public bool IsXYZero => GammaXY.ApproxZero();

	/// <summary>
	///     Returns true if <see cref="EpsilonX" /> is zero.
	/// </summary>
	public bool IsXZero => EpsilonX.ApproxZero();

	/// <summary>
	///     Returns true if <see cref="EpsilonY" /> is zero.
	/// </summary>
	public bool IsYZero => EpsilonY.ApproxZero();

	/// <inheritdoc />
	public bool IsZero => IsXZero && IsYZero && IsXYZero;

	/// <inheritdoc />
	public double ThetaX { get; }

	/// <inheritdoc />
	public double ThetaY => ThetaX + Constants.PiOver2;

	/// <inheritdoc />
	public Matrix<double> TransformationMatrix { get; }

	double IState<double>.X => EpsilonX;

	double IState<double>.XY => GammaXY;

	double IState<double>.Y => EpsilonY;

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

	private StrainState(IState<double> state)
		: this(state.X, state.Y, state.XY, state.ThetaX)
	{
	}

	#endregion

	#region Methods

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
	///     Get <see cref="StrainState" /> transformed to horizontal direction (<see cref="ThetaX" /> = 0).
	/// </summary>
	/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
	public static StrainState ToHorizontal(IState<double> strainState) =>
		Transform(strainState, -strainState.ThetaX);

	/// <summary>
	///     Get <see cref="StrainState" /> transformed by a rotation angle.
	/// </summary>
	/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
	/// <inheritdoc cref="Transform(double)" />
	public static StrainState Transform(IState<double> strainState, double theta)
	{
		var state = strainState is StrainState strains
			? strains
			: new StrainState(strainState);

		if (theta.ApproxZero(1E-6))
			return state;

		// Get the strain vector transformed
		var sVec = StrainRelations.Transform(state.AsVector(), theta);

		// Return with corrected angle
		return FromVector(sVec, state.ThetaX + theta);
	}

	/// <inheritdoc />
	public override bool Equals(object? obj) => obj is IState<double> state && Equals(state);

	/// <inheritdoc />
	public override int GetHashCode() => (int) (EpsilonX * EpsilonY * GammaXY);

	/// <summary>
	///     Get this <see cref="StrainState" /> transformed to horizontal direction (<see cref="ThetaX" /> = 0).
	/// </summary>
	public StrainState ToHorizontal() => ToHorizontal(this);

	/// <summary>
	///     Get the <see cref="PrincipalStrainState" /> related to this <see cref="StrainState" />.
	/// </summary>
	public PrincipalStrainState ToPrincipal() => PrincipalStrainState.FromStrain(this);

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

	/// <summary>
	///     Get this <see cref="StrainState" /> transformed by a rotation angle.
	/// </summary>
	/// <inheritdoc cref="IState{T}.Transform" />
	public StrainState Transform(double rotationAngle) => Transform(this, rotationAngle);

	/// <inheritdoc />
	public bool Approaches(IState<double>? other, double tolerance) =>
		other is not null &&
		ThetaX.Approx(other.ThetaX, tolerance) && EpsilonX.Approx(other.X, tolerance) &&
		EpsilonY.Approx(other.Y, tolerance) && GammaXY.Approx(other.XY, tolerance);

	/// <inheritdoc />
	public StrainState Clone() => new(EpsilonX, EpsilonY, GammaXY, ThetaX);

	/// <summary>
	///     Compare two <see cref="StrainState" /> objects.
	/// </summary>
	/// <param name="other">The strain to compare.</param>
	public bool Equals(IState<double>? other) => Approaches(other, Tolerance);

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

	/// <inheritdoc />
	IState<double> IState<double>.ToHorizontal() => ToHorizontal();

	/// <inheritdoc />
	IPrincipalState<double> IState<double>.ToPrincipal() => ToPrincipal();

	/// <inheritdoc />
	IState<double> IState<double>.Transform(double rotationAngle) => Transform(rotationAngle);

	#endregion

}