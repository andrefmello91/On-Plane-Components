using System;
using System.Linq;
using andrefmello91.Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;
using static andrefmello91.OnPlaneComponents.StressRelations;
using static andrefmello91.OnPlaneComponents.StressState;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Principal stress struct.
	/// </summary>
	public partial struct PrincipalStressState : IPrincipalState<Pressure>, IUnitConvertible<PressureUnit>, IApproachable<IState<Pressure>, Pressure>, IEquatable<IState<Pressure>>, ICloneable<PrincipalStressState>
	{

		#region Properties

		/// <summary>
		///     Get a <see cref="PrincipalStressState" /> with zero elements.
		/// </summary>
		public static PrincipalStressState Zero { get; } = new(0, 0);

		/// <summary>
		///     Get maximum principal stress.
		/// </summary>
		public Pressure Sigma1 { get; private set; }

		/// <summary>
		///     Get minimum principal stress.
		/// </summary>
		public Pressure Sigma2 { get; private set; }

		/// <inheritdoc />
		public PrincipalCase Case =>
			Is1Zero switch
			{
				true when Is2Zero                 => PrincipalCase.Zero,
				true when !Is2Zero                => PrincipalCase.UniaxialCompression,
				false when Is2Zero                => PrincipalCase.UniaxialTension,
				false when Sigma2 > Pressure.Zero => PrincipalCase.PureTension,
				false when Sigma1 < Pressure.Zero => PrincipalCase.PureCompression,
				_                                 => PrincipalCase.TensionCompression
			};

		/// <summary>
		///     Returns true if <see cref="Sigma1" /> is nearly zero.
		/// </summary>
		public bool Is1Zero => Sigma1.ApproxZero(Tolerance);

		/// <summary>
		///     Returns true if <see cref="Sigma2" /> is nearly zero.
		/// </summary>
		public bool Is2Zero => Sigma2.ApproxZero(Tolerance);

		/// <inheritdoc />
		public bool IsAt45Degrees => Theta1.Approx(Constants.PiOver4) || Theta2.Approx(Constants.PiOver4) || Theta1.Approx(-Constants.PiOver4) || Theta2.Approx(-Constants.PiOver4);

		/// <inheritdoc />
		public bool IsHorizontal => Theta1.ApproxZero() || Theta1.Approx(Constants.Pi);

		/// <inheritdoc />
		public bool IsVertical => Theta1.Approx(Constants.PiOver2) || Theta1.Approx(Constants.Pi3Over2);

		/// <summary>
		///     Returns true if <see cref="Sigma1" /> and <see cref="Sigma2" /> are zero.
		/// </summary>
		public bool IsZero => Is1Zero && Is2Zero;

		/// <summary>
		///     Get the angle of maximum principal stress <see cref="Sigma1" />, related to horizontal axis.
		/// </summary>
		public double Theta1 { get; }

		/// <summary>
		///     Get the angle of minimum principal stress <see cref="Sigma2" />, related to horizontal axis.
		/// </summary>
		public double Theta2 => Theta1 + Constants.PiOver2;

		/// <inheritdoc />
		public Matrix<double> TransformationMatrix { get; }

		bool IState<Pressure>.IsPrincipal => true;

		bool IState<Pressure>.IsPureShear => false;

		bool IState<Pressure>.IsXYZero => true;

		bool IState<Pressure>.IsXZero => Is1Zero;

		bool IState<Pressure>.IsYZero => Is2Zero;

		Pressure IPrincipalState<Pressure>.S1 => Sigma1;

		Pressure IPrincipalState<Pressure>.S2 => Sigma2;

		double IState<Pressure>.ThetaX => Theta1;

		double IState<Pressure>.ThetaY => Theta2;

		Pressure IState<Pressure>.X => Sigma1;

		Pressure IState<Pressure>.XY => Pressure.Zero;

		Pressure IState<Pressure>.Y => Sigma2;

		/// <summary>
		///     Get/set the stress unit (<see cref="PressureUnit" />).
		/// </summary>
		public PressureUnit Unit
		{
			get => Sigma1.Unit;
			set => ChangeUnit(value);
		}

		#endregion

		#region Constructors

		/// <summary>
		///     Principal Stress object.
		/// </summary>
		/// <param name="sigma1">The maximum principal stress (positive for tensile).</param>
		/// <param name="sigma2">The minimum principal stress (positive for tensile).</param>
		/// <param name="theta1">
		///     The angle of <paramref name="sigma1" />, related to horizontal axis (positive to
		///     counterclockwise).
		/// </param>
		/// <param name="unit">The <see cref="PressureUnit" /> of stresses (default: <see cref="PressureUnit.Megapascal" />).</param>
		public PrincipalStressState(double sigma1, double sigma2, double theta1 = Constants.PiOver4, PressureUnit unit = PressureUnit.Megapascal)
			: this((Pressure) sigma1.As(unit), (Pressure) sigma2.As(unit), theta1)
		{
		}

		/// <inheritdoc cref="PrincipalStressState(double, double, double, PressureUnit)" />
		public PrincipalStressState(Pressure sigma1, Pressure sigma2, double theta1 = Constants.PiOver4)
		{
			Sigma1               = sigma1;
			Sigma2               = sigma2.ToUnit(sigma1.Unit);
			Theta1               = theta1.AsFinite();
			TransformationMatrix = StrainRelations.TransformationMatrix(theta1);
		}

		#endregion

		#region Methods

		/// <summary>
		///     Get a <see cref="PrincipalStressState" /> from a <see cref="StressState" />.
		/// </summary>
		/// <param name="stressState">The <see cref="StressState" /> to transform.</param>
		public static PrincipalStressState FromStress(StressState stressState)
		{
			var (s1, s2) = CalculatePrincipal(stressState.AsVector());
			var theta1 = CalculatePrincipalAngles(stressState.AsVector(), s2).theta1;

			return new PrincipalStressState(s1, s2, stressState.ThetaX + theta1);
		}

		/// <summary>
		///     Get this <see cref="PrincipalStressState" /> as a <see cref="StressState" />.
		/// </summary>
		/// <remarks>
		///     { Sigma1, Sigma2, 0 }
		/// </remarks>
		public StressState AsStressState() => new(Sigma1, Sigma2, Pressure.Zero, Theta1);

		/// <summary>
		///     Get principal stresses as a <see cref="Vector" />, in a desired <see cref="PressureUnit" />.
		/// </summary>
		/// <remarks>
		///     { Sigma1, Sigma2, 0 }
		/// </remarks>
		/// <param name="unit">The <see cref="PressureUnit" />.</param>
		public Vector<double> AsVector(PressureUnit unit = PressureUnit.Megapascal) => AsArray().Select(s => s.ToUnit(unit).Value).ToVector();

		/// <summary>
		///     Convert this <see cref="PrincipalStressState" /> to another <see cref="PressureUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public PrincipalStressState Convert(PressureUnit unit) => unit == Unit
			? this
			: new PrincipalStressState(Sigma1.ToUnit(unit), Sigma2.ToUnit(unit), Theta1);

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return obj switch
			{
				PrincipalStressState other => Equals(other),
				StressState stress         => Equals(stress),
				_                          => false
			};
		}

		/// <inheritdoc />
		public override int GetHashCode() => (int) (Sigma1.Value * Sigma2.Value);

		/// <summary>
		///     Get this <see cref="PrincipalStressState" /> transformed to horizontal direction (<see cref="StressState.ThetaX" />
		///     = 0).
		/// </summary>
		public StressState ToHorizontal() => StressState.ToHorizontal(this);

		/// <inheritdoc />
		public override string ToString()
		{
			char
				sigma = (char) Characters.Sigma,
				theta = (char) Characters.Theta;

			return
				$"{sigma}1 = {Sigma1}\n" +
				$"{sigma}2 = {Sigma2}\n" +
				$"{theta}1 = {Theta1:0.00} rad";
		}

		/// <summary>
		///     Get this <see cref="PrincipalStressState" /> transformed by a rotation angle.
		/// </summary>
		/// <inheritdoc cref="IState{T}.Transform" />
		public StressState Transform(double rotationAngle) => StressState.Transform(this, rotationAngle);

		/// <inheritdoc />
		public bool Approaches(IState<Pressure>? other, Pressure tolerance)
		{
			if (other is null)
				return false;

			var principal = other.ToPrincipal();

			return
				Theta1.Approx(principal.Theta1, 1E-3) &&
				Sigma1.Approx(principal.S1, tolerance) && Sigma2.Approx(principal.S2, tolerance);
		}

		/// <summary>
		///     Return a copy of this <see cref="PrincipalStressState" />.
		/// </summary>
		public PrincipalStressState Clone() => new(Sigma1, Sigma2, Theta1);

		/// <summary>
		///     Compare a <see cref="PrincipalStressState" /> to a <see cref="StressState" /> object.
		/// </summary>
		/// <param name="other">The <see cref="StressState" /> to compare.</param>
		public bool Equals(IState<Pressure>? other) => Approaches(other, Tolerance);

		/// <summary>
		///     Get principal stresses as an <see cref="Array" />.
		/// </summary>
		/// <remarks>
		///     { Sigma1, Sigma2, 0 }
		/// </remarks>
		public Pressure[] AsArray() => new[] { Sigma1, Sigma2, Pressure.Zero };

		/// <inheritdoc />
		public Vector<double> AsVector() => AsVector(Unit);

		IState<Pressure> IPrincipalState<Pressure>.AsState() => AsStressState();

		IState<Pressure> IState<Pressure>.ToHorizontal() => ToHorizontal();

		IPrincipalState<Pressure> IState<Pressure>.ToPrincipal() => this;

		IState<Pressure> IState<Pressure>.Transform(double rotationAngle) => Transform(rotationAngle);

		/// <summary>
		///     Change the <see cref="PressureUnit" /> of this <see cref="PrincipalStressState" />.
		/// </summary>
		/// <param name="unit">The <see cref="PressureUnit" /> to convert.</param>
		public void ChangeUnit(PressureUnit unit)
		{
			if (Unit == unit)
				return;

			Sigma1 = Sigma1.ToUnit(unit);
			Sigma2 = Sigma2.ToUnit(unit);
		}

		IUnitConvertible<PressureUnit> IUnitConvertible<PressureUnit>.Convert(PressureUnit unit) => Convert(unit);

		#endregion

	}
}