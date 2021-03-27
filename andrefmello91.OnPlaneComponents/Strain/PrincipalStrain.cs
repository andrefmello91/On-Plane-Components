using System;
using andrefmello91.Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using static andrefmello91.OnPlaneComponents.StrainRelations;
using static andrefmello91.OnPlaneComponents.StrainState;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Principal strain struct.
	/// </summary>
	public readonly partial struct PrincipalStrainState : IPrincipalState<double>, IApproachable<StrainState, double>, IApproachable<PrincipalStrainState, double>, IEquatable<StrainState>, IEquatable<PrincipalStrainState>, ICloneable<PrincipalStrainState>
	{

		#region Fields

		/// <summary>
		///     Get a <see cref="PrincipalStrainState" /> with zero elements.
		/// </summary>
		public static readonly PrincipalStrainState Zero = new(0, 0);

		#endregion

		#region Properties

		/// <inheritdoc />
		public PrincipalCase Case
		{
			get
			{
				if (IsZero)
					return PrincipalCase.Zero;

				if (Epsilon1 > 0 && Epsilon2 >= 0)
					return PrincipalCase.PureTension;

				if (Epsilon1 <= 0 && Epsilon2 < 0)
					return PrincipalCase.PureCompression;

				return PrincipalCase.TensionCompression;
			}
		}

		double IPrincipalState<double>.S1 => Epsilon1;

		double IPrincipalState<double>.S2 => Epsilon2;

		/// <summary>
		///     Returns true if <see cref="Epsilon1" /> is nearly zero.
		/// </summary>
		public bool Is1Zero => Epsilon1.ApproxZero();

		/// <summary>
		///     Returns true if <see cref="Epsilon2" /> is nearly zero.
		/// </summary>
		public bool Is2Zero => Epsilon2.ApproxZero();

		/// <inheritdoc />
		public bool IsHorizontal => Theta1.ApproxZero() || Theta1.Approx(Constants.Pi);

		/// <inheritdoc />
		public bool IsVertical => Theta1.Approx(Constants.PiOver2) || Theta1.Approx(Constants.Pi3Over2);

		bool IState<double>.IsPrincipal => true;

		bool IState<double>.IsPureShear => false;

		double IState<double>.ThetaX => Theta1;

		double IState<double>.ThetaY => Theta2;

		/// <inheritdoc />
		public bool IsAt45Degrees => Theta1.Approx(Constants.PiOver4) || Theta2.Approx(Constants.PiOver4) || Theta1.Approx(-Constants.PiOver4) || Theta2.Approx(-Constants.PiOver4);

		double IState<double>.X => Epsilon1;

		double IState<double>.Y => Epsilon2;

		double IState<double>.XY => 0;

		bool IState<double>.IsXZero => Is1Zero;

		bool IState<double>.IsYZero => Is2Zero;

		bool IState<double>.IsXYZero => true;

		/// <inheritdoc />
		public bool IsZero => Is1Zero && Is2Zero;

		/// <inheritdoc />
		public double Theta1 { get; }

		/// <inheritdoc />
		public double Theta2 => Theta1 + Constants.PiOver2;

		/// <inheritdoc />
		public Matrix<double> TransformationMatrix { get; }

		/// <summary>
		///     Get maximum principal strain.
		/// </summary>
		public double Epsilon1 { get; }

		/// <summary>
		///     Get minimum principal strain.
		/// </summary>
		public double Epsilon2 { get; }

		#endregion

		#region Constructors

		/// <summary>
		///     Principal Strain object.
		/// </summary>
		/// <param name="epsilon1">The maximum principal strain (positive for tensile).</param>
		/// <param name="epsilon2">The minimum principal strain (positive for tensile).</param>
		/// <param name="theta1">
		///     The angle of maximum principal strain (<paramref name="epsilon1" />), related to horizontal axis
		///     (positive to counterclockwise).
		/// </param>
		public PrincipalStrainState(double epsilon1, double epsilon2, double theta1 = Constants.PiOver4)
		{
			Epsilon1             = epsilon1.ToZero();
			Epsilon2             = epsilon2.ToZero();
			Theta1               = theta1.ToZero();
			TransformationMatrix = TransformationMatrix(theta1);
		}

		#endregion

		#region

		/// <summary>
		///     Get a <see cref="PrincipalStrainState" /> from a <see cref="StrainState" />.
		/// </summary>
		/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
		public static PrincipalStrainState FromStrain(StrainState strainState)
		{
			var (e1, e2) = CalculatePrincipal(strainState.AsVector());
			var theta1 = CalculatePrincipalAngles(strainState.AsVector(), e2).theta1;

			return new PrincipalStrainState(e1, e2, strainState.ThetaX + theta1);
		}

		/// <inheritdoc />
		public IPrincipalState<double> ToPrincipal() => this;

		/// <inheritdoc />
		public PrincipalStrainState Clone() => new(Epsilon1, Epsilon2, Theta1);

		/// <summary>
		///     Get principal strains as an <see cref="Array" />.
		/// </summary>
		/// <remarks>
		///     { Epsilon1, Epsilon2, 0 }
		/// </remarks>
		public double[] AsArray() => new[] { Epsilon1, Epsilon2, 0 };

		/// <summary>
		///     Get principal strains as <see cref="Vector" />.
		/// </summary>
		/// <remarks>
		///     { Epsilon1, Epsilon2, 0 }
		/// </remarks>
		public Vector<double> AsVector() => AsArray().ToVector();

		/// <inheritdoc />
		public bool Approaches(StrainState other, double tolerance) => Approaches((PrincipalStrainState) other.ToPrincipal(), tolerance);

		/// <inheritdoc />
		public bool Approaches(PrincipalStrainState other, double tolerance) =>
			Theta1.Approx(other.Theta1, 1E-3) &&
			Epsilon1.Approx(other.Epsilon1, tolerance) && Epsilon2.Approx(other.Epsilon2, tolerance);

		/// <summary>
		///     Get this <see cref="PrincipalStrainState" /> as a <see cref="StrainState" />.
		/// </summary>
		/// <remarks>
		///     { Epsilon1, Epsilon2, 0 }
		/// </remarks>
		public StrainState AsStrainState() => new(Epsilon1, Epsilon2, 0, Theta1);

		/// <summary>
		///     Get this <see cref="PrincipalStrainState" /> transformed to horizontal direction (<see cref="StrainState.ThetaX" />
		///     = 0).
		/// </summary>
		public IState<double> ToHorizontal() => AsStrainState().ToHorizontal();

		/// <summary>
		///     Get this <see cref="PrincipalStrainState" /> transformed by a rotation angle.
		/// </summary>
		/// <inheritdoc />
		public IState<double> Transform(double rotationAngle) => AsStrainState().Transform(rotationAngle);

		IState<double> IPrincipalState<double>.AsState() => AsStrainState();

		/// <summary>
		///     Compare two <see cref="PrincipalStrainState" /> objects.
		/// </summary>
		/// <param name="other">The other <see cref="PrincipalStrainState" /> to compare.</param>
		public bool Equals(PrincipalStrainState other) => Approaches(other, Tolerance);

		/// <summary>
		///     Compare a <see cref="PrincipalStrainState" /> to a <see cref="StrainState" /> object.
		/// </summary>
		/// <param name="other">The <see cref="StrainState" /> to compare.</param>
		public bool Equals(StrainState other) => Approaches(other, Tolerance);

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			switch (obj)
			{
				case PrincipalStrainState other:
					return Equals(other);

				case StrainState strain:
					return Equals(strain);

				default:
					return false;
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			char
				epsilon = (char) Characters.Epsilon,
				theta   = (char) Characters.Theta;

			return
				$"{epsilon}1 = {Epsilon1:0.##E+00}\n" +
				$"{epsilon}2 = {Epsilon2:0.##E+00}\n" +
				$"{theta}1 = {Theta1:0.00} rad";
		}

		/// <inheritdoc />
		public override int GetHashCode() => (int) (Epsilon1 * Epsilon2);

		#endregion

	}
}