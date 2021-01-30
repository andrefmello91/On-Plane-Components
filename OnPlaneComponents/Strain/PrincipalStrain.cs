using System;
using Extensions.LinearAlgebra;
using Extensions.Number;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet.Units;
using static OnPlaneComponents.StrainRelations;
using static OnPlaneComponents.StrainState;

namespace OnPlaneComponents
{
	/// <summary>
	///     Principal strain struct.
	/// </summary>
	public partial struct PrincipalStrainState : IPrincipalState<double>, IApproachable<StrainState, double>, IApproachable<PrincipalStrainState, double>, IEquatable<PrincipalStrainState>, IEquatable<StrainState>, ICloneable<PrincipalStrainState>
	{
		#region Fields

		/// <summary>
		///     Get a <see cref="PrincipalStrainState" /> with zero elements.
		/// </summary>
		public static readonly PrincipalStrainState Zero = new PrincipalStrainState(0, 0);

		#endregion

		#region Properties

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

		double IPrincipalState<double>.T1 => Epsilon1;

		double IPrincipalState<double>.T2 => Epsilon2;

		/// <summary>
		///     Get maximum principal strain.
		/// </summary>
		public double Epsilon1 { get; }

		/// <summary>
		///     Get minimum principal strain.
		/// </summary>
		public double Epsilon2 { get; }

		/// <summary>
		///     Returns true if <see cref="Epsilon1" /> is nearly zero.
		/// </summary>
		public bool Is1Zero => Epsilon1.ApproxZero();

		/// <summary>
		///     Returns true if <see cref="Epsilon2" /> is nearly zero.
		/// </summary>
		public bool Is2Zero => Epsilon2.ApproxZero();

		public bool IsHorizontal => Theta1.ApproxZero() || Theta1.Approx(Constants.Pi);

		public bool IsVertical => Theta1.Approx(Constants.PiOver2) || Theta1.Approx(Constants.Pi3Over2);

		public bool IsAt45Degrees => Theta1.Approx(Constants.PiOver4) || Theta2.Approx(Constants.PiOver4) || Theta1.Approx(-Constants.PiOver4) || Theta2.Approx(-Constants.PiOver4);

		public bool IsZero => Is1Zero && Is2Zero;

		public double Theta1 { get; }

		public double Theta2 => Theta1 + Constants.PiOver2;

		public Matrix<double> TransformationMatrix { get; }

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
			TransformationMatrix = StrainRelations.TransformationMatrix(theta1);
		}

		#endregion

		#region  Methods

		/// <summary>
		///     Get a <see cref="PrincipalStrainState" /> from a <see cref="StrainState" />.
		/// </summary>
		/// <param name="strainState">The <see cref="StrainState" /> to transform.</param>
		public static PrincipalStrainState FromStrain(StrainState strainState)
		{
			var (e1, e2) = CalculatePrincipal(strainState.AsVector());
			var theta1   = CalculatePrincipalAngles(strainState.AsVector(), e2).theta1;

			return new PrincipalStrainState(e1, e2, strainState.ThetaX + theta1);
		}


		public PrincipalStrainState Clone() => new PrincipalStrainState(Epsilon1, Epsilon2, Theta1);

		/// <summary>
		///     Get principal strains as an <see cref="Array" />.
		///     <para>{ Epsilon1, Epsilon2, 0 }</para>
		/// </summary>
		public double[] AsArray() => new[] { Epsilon1, Epsilon2, 0 };

		/// <summary>
		///     Get principal strains as <see cref="Vector" />.
		///     <para>{ Epsilon1, Epsilon2, 0 }</para>
		/// </summary>
		public Vector<double> AsVector() => AsArray().ToVector();

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

		public override bool Equals(object obj)
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

		public bool Approaches(StrainState other, double tolerance) => Approaches(other.ToPrincipal(), tolerance);

		public bool Approaches(PrincipalStrainState other, double tolerance) =>
			  Theta1.Approx(other.Theta1,   1E-3)      &&
			Epsilon1.Approx(other.Epsilon1, tolerance) && Epsilon2.Approx(other.Epsilon2, tolerance);

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

		public override int GetHashCode() => (int) (Epsilon1 * Epsilon2);

		#endregion
	}
}