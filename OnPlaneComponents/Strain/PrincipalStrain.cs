using System;
using Extensions.LinearAlgebra;
using Extensions.Number;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using static OnPlaneComponents.StrainRelations;

namespace OnPlaneComponents
{
	/// <summary>
    /// Principal strain struct.
    /// </summary>
	public partial struct PrincipalStrainState : IEquatable<PrincipalStrainState>
    {
	    // Auxiliary fields
	    private Matrix<double> _transMatrix;

        /// <summary>
        /// Get maximum principal strain.
        /// </summary>
        public double Epsilon1 { get; }

        /// <summary>
        /// Get minimum principal strain.
	    /// </summary>
        public double Epsilon2 { get; }

        /// <summary>
        /// Get the angle of maximum principal strain (<see cref="Epsilon1"/>), related to horizontal axis.
        /// </summary>
        public double Theta1 { get; }

        /// <summary>
        /// Get the angle of minimum principal strain (<see cref="Epsilon2"/>), related to horizontal axis.
        /// </summary>
        public double Theta2 => Theta1 + Constants.PiOver2;

        /// <summary>
        /// Get transformation <see cref="Matrix"/> from horizontal plane to principal plane.
        /// <para>See: <seealso cref="StrainRelations.TransformationMatrix"/></para>
        /// </summary>
        public Matrix<double> TransformationMatrix => _transMatrix ?? CalculateTransformationMatrix();

		/// <summary>
        /// Get the <see cref="PrincipalCase"/> of <seealso cref="PrincipalStrainState"/>.
        /// </summary>
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

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> is zero.
        /// </summary>
        public bool IsEpsilon1Zero => Epsilon1 == 0;

        /// <summary>
        /// Returns true if <see cref="Epsilon2"/> is zero.
        /// </summary>
        public bool IsEpsilon2Zero => Epsilon2 == 0;

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> and <see cref="Epsilon2"/> are zero.
        /// </summary>
        public bool IsZero => IsEpsilon1Zero && IsEpsilon2Zero;

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> direction coincides to horizontal axis.
        /// </summary>
        public bool IsHorizontal => Theta1 == 0;

        /// <summary>
        /// Principal Strain object.
        /// </summary>
        /// <param name="epsilon1">The maximum principal strain (positive for tensile).</param>
        /// <param name="epsilon2">The minimum principal strain (positive for tensile).</param>
        /// <param name="theta1">The angle of maximum principal strain (<paramref name="epsilon1"/>), related to horizontal axis (positive to counterclockwise).</param>
        public PrincipalStrainState(double epsilon1, double epsilon2, double theta1 = Constants.PiOver4)
        {
	        Epsilon1     = epsilon1.ToZero();
	        Epsilon2     = epsilon2.ToZero();
	        Theta1       = theta1.ToZero();
	        _transMatrix = null;
        }

        /// <summary>
        /// Return a copy of this <see cref="PrincipalStrainState"/>.
        /// </summary>
        public PrincipalStrainState Copy() => new PrincipalStrainState(Epsilon1, Epsilon2, Theta1);
		
        /// <summary>
        /// Get principal strains as an <see cref="Array"/>.
        /// <para>{ Epsilon1, Epsilon2, 0 }</para>
        /// </summary>
        public double[] AsArray() => new[] { Epsilon1, Epsilon2, 0 };

        /// <summary>
        /// Get principal strains as <see cref="Vector"/>.
        /// <para>{ Epsilon1, Epsilon2, 0 }</para>
        /// </summary>
        public Vector<double> AsVector() => AsArray().ToVector();

        /// <summary>
        /// Calculate <see cref="TransformationMatrix"/>.
        /// </summary>
        private Matrix<double> CalculateTransformationMatrix()
        {
	        _transMatrix = StrainRelations.TransformationMatrix(Theta1);
	        return _transMatrix;
        }

        /// <summary>
        /// Get a <see cref="PrincipalStrainState"/> with zero elements.
        /// </summary>
        public static PrincipalStrainState Zero => new PrincipalStrainState(0, 0);

        /// <summary>
        /// Get a <see cref="PrincipalStrainState"/> from a <see cref="StrainState"/>.
        /// </summary>
        /// <param name="strainState">The <see cref="StrainState"/> to transform.</param>
        public static PrincipalStrainState FromStrain(StrainState strainState)
        {
	        var (e1, e2) = CalculatePrincipal(strainState.AsVector());
	        var theta1   = CalculatePrincipalAngles(strainState.AsVector(), e2).theta1;

			return new PrincipalStrainState(e1, e2, strainState.ThetaX + theta1);
        }

        /// <summary>
        /// Return zero if <paramref name="number"/> is <see cref="double.NaN"/> or <see cref="double.PositiveInfinity"/> or <see cref="double.NegativeInfinity"/>.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static double DoubleToZero(double number) => !double.IsNaN(number) && !double.IsInfinity(number) ? number : 0;

        /// <summary>
        /// Compare two <see cref="PrincipalStrainState"/> objects.
        /// </summary>
        /// <param name="other">The other <see cref="PrincipalStrainState"/> to compare.</param>
        public bool Equals(PrincipalStrainState other) => Theta1 == other.Theta1 && Epsilon1 == other.Epsilon1 && Epsilon2 == other.Epsilon2;

        /// <summary>
        /// Compare a <see cref="PrincipalStrainState"/> to a <see cref="StrainState"/> object.
        /// </summary>
        /// <param name="other">The <see cref="StrainState"/> to compare.</param>
        public bool Equals(StrainState other) => Equals(FromStrain(other));

        public override bool Equals(object obj)
        {
	        if (obj is PrincipalStrainState other)
		        return Equals(other);

	        if (obj is StrainState strain)
		        return Equals(strain);

	        return false;
        }

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

        public override int GetHashCode() => (int)(Epsilon1 * Epsilon2);
    }
}
