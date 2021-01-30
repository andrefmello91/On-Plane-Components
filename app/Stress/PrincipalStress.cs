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
using static OnPlaneComponents.StressState;

namespace OnPlaneComponents
{
	/// <summary>
	///     Principal stress struct.
	/// </summary>
	public partial struct PrincipalStressState : IPrincipalState<Pressure>, IApproachable<StressState, Pressure>, IApproachable<PrincipalStressState, Pressure>, IUnitConvertible<PrincipalStressState, PressureUnit>, IEquatable<PrincipalStressState>, IEquatable<StressState>, ICloneable<PrincipalStressState>
	{
		#region Fields

		/// <summary>
		///     Get a <see cref="PrincipalStressState" /> with zero elements.
		/// </summary>
		public static readonly  PrincipalStressState Zero = new PrincipalStressState(0, 0);

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the stress unit (<see cref="PressureUnit" />).
		/// </summary>
		public PressureUnit Unit
		{
			get => Sigma1.Unit;
			set => ChangeUnit(value);
		}

		public PrincipalCase Case
		{
			get
			{
				if (IsZero)
					return PrincipalCase.Zero;

				if (Sigma1 > Pressure.Zero && Sigma2 >= Pressure.Zero)
					return PrincipalCase.PureTension;

				if (Sigma1 <= Pressure.Zero && Sigma2 < Pressure.Zero)
					return PrincipalCase.PureCompression;

				return PrincipalCase.TensionCompression;
			}
		}

		Pressure IPrincipalState<Pressure>.T1 => Sigma1;

		Pressure IPrincipalState<Pressure>.T2 => Sigma2;

		bool IState<Pressure>.IsPrincipal => true;

		bool IState<Pressure>.IsPureShear => false;

		double IState<Pressure>.ThetaX => Theta1;

		double IState<Pressure>.ThetaY => Theta2;

		Pressure IState<Pressure>.X => Sigma1;

		Pressure IState<Pressure>.Y => Sigma1;

		Pressure IState<Pressure>.XY => Pressure.Zero;

		bool IState<Pressure>.IsXZero => Is1Zero;

		bool IState<Pressure>.IsYZero => Is2Zero;

		bool IState<Pressure>.IsXYZero => true;

		public bool IsHorizontal => Theta1.ApproxZero() || Theta1.Approx(Constants.Pi);

		public bool IsVertical => Theta1.Approx(Constants.PiOver2) || Theta1.Approx(Constants.Pi3Over2);

		public bool IsAt45Degrees => Theta1.Approx(Constants.PiOver4) || Theta2.Approx(Constants.PiOver4) || Theta1.Approx(-Constants.PiOver4) || Theta2.Approx(-Constants.PiOver4);

		/// <summary>
		///     Returns true if <see cref="Sigma1" /> is nearly zero.
		/// </summary>
		public bool Is1Zero => Sigma1.ApproxZero(Tolerance);

		/// <summary>
		///     Returns true if <see cref="Sigma2" /> is nearly zero.
		/// </summary>
		public bool Is2Zero => Sigma2.ApproxZero(Tolerance);

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

		public Matrix<double> TransformationMatrix { get; }

		/// <summary>
		///     Get maximum principal stress.
		/// </summary>
		public Pressure Sigma1 { get; private set; }

		/// <summary>
		///     Get minimum principal stress=.
		/// </summary>
		public Pressure Sigma2 { get; private set; }

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
			: this(Pressure.From(sigma1.ToZero(), unit), Pressure.From(sigma2.ToZero(), unit), theta1)
		{
		}

		/// <inheritdoc cref="PrincipalStressState(double, double, double, PressureUnit)" />
		public PrincipalStressState(Pressure sigma1, Pressure sigma2, double theta1 = Constants.PiOver4)
		{
			Sigma1               = sigma1;
			Sigma2               = sigma2.ToUnit(sigma1.Unit);
			Sigma2               = sigma2.ToUnit(sigma1.Unit);
			Theta1               = theta1.ToZero();
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
			var theta1   = CalculatePrincipalAngles(stressState.AsVector(), s2).theta1;

			return new PrincipalStressState(s1, s2, stressState.ThetaX + theta1);
		}

		public IPrincipalState<Pressure> ToPrincipal() => this;

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

		/// <summary>
		///     Convert this <see cref="PrincipalStressState" /> to another <see cref="PressureUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public PrincipalStressState Convert(PressureUnit unit) => unit == Unit
			? this
			: new PrincipalStressState(Sigma1.ToUnit(unit), Sigma2.ToUnit(unit), Theta1);

		/// <summary>
		///     Get principal stresses as an <see cref="Array" />.
		/// </summary>
		/// <remarks>
		///		{ Sigma1, Sigma2, 0 }
		/// </remarks>
		public Pressure[] AsArray() => new[] { Sigma1, Sigma2, Pressure.Zero };

		/// <summary>
		///     Get principal stresses as a <see cref="Vector" />, in <see cref="Unit" /> considered.
		/// </summary>
		/// <remarks>
		///		{ Sigma1, Sigma2, 0 }
		/// </remarks>
		public Vector<double> AsVector() => AsArray().Select(s => s.Value).ToVector();

		/// <summary>
		///     Get this <see cref="PrincipalStressState" /> as a <see cref="StressState" />.
		/// </summary>
		/// <remarks>
		///		{ Sigma1, Sigma2, 0 }
		/// </remarks>
		public StressState AsStressState() => new StressState(Sigma1, Sigma2, Pressure.Zero, Theta1);

		/// <summary>
		///     Get this <see cref="PrincipalStressState" /> transformed to horizontal direction (<see cref="StressState.ThetaX" /> = 0).
		/// </summary>
		public StressState ToHorizontal() => AsStressState().ToHorizontal();

		/// <summary>
		///     Get this <see cref="PrincipalStressState" /> transformed by a rotation angle.
		/// </summary>
		/// <inheritdoc cref="IState{T}.Transform" />
		public StressState Transform(double rotationAngle) => AsStressState().Transform(rotationAngle);

		IState<Pressure> IPrincipalState<Pressure>.AsState() => AsStressState();

		IState<Pressure> IState<Pressure>.ToHorizontal() => ToHorizontal();

		IState<Pressure> IState<Pressure>.Transform(double rotationAngle) => Transform(rotationAngle);

		/// <summary>
		///     Return a copy of this <see cref="PrincipalStressState" />.
		/// </summary>
		public PrincipalStressState Clone() => new PrincipalStressState(Sigma1, Sigma2, Theta1);

		public bool Approaches(StressState other, Pressure tolerance) => Approaches(other.ToPrincipal(), tolerance);

		public bool Approaches(PrincipalStressState other, Pressure tolerance) =>
			Theta1.Approx(other.Theta1, 1E-3) &&
			Sigma1.Approx(other.Sigma1, tolerance) && Sigma2.Approx(other.Sigma2, tolerance);

		/// <summary>
		///     Compare two <see cref="PrincipalStressState" /> objects.
		/// </summary>
		/// <param name="other">The other <see cref="PrincipalStressState" /> to compare.</param>
		public bool Equals(PrincipalStressState other) => Approaches(other, Tolerance);

		/// <summary>
		///     Compare a <see cref="PrincipalStressState" /> to a <see cref="StressState" /> object.
		/// </summary>
		/// <param name="other">The <see cref="StressState" /> to compare.</param>
		public bool Equals(StressState other) => Approaches(other, Tolerance);

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case PrincipalStressState other:
					return Equals(other);

				case StressState stress:
					return Equals(stress);

				default:
					return false;
			}
		}

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

		public override int GetHashCode() => (int) (Sigma1.Value * Sigma2.Value);

		#endregion
	}
}