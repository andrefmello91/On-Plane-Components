using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace OnPlaneComponents
{
    /// <summary>
    /// Principal strain struct.
    /// </summary>
	public partial struct PrincipalStrainState : IEquatable<PrincipalStrainState>
    {
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
        /// Get principal strains as <see cref="DenseVector"/>.
        /// <para>{ Epsilon1, Epsilon2, 0 }</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new[] { Epsilon1, Epsilon2, 0 });

        /// <summary>
        /// Get transformation <see cref="Matrix"/> from principal plane to horizontal plane.
        /// <para>See: <seealso cref="StrainRelations.TransformationMatrix"/></para>
        /// </summary>
        public Matrix<double> TransformationMatrix => StrainRelations.TransformationMatrix(Theta1);

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> is zero.
        /// </summary>
        public bool IsEpsilon1Zero => Epsilon1 == 0;

        /// <summary>
        /// Returns true if <see cref="Epsilon2"/> is zero.
        /// </summary>
        public bool IsEpsilon2Zero => Epsilon2 == 0;

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> and <see cref="Epsilon2"/> are compressive strains.
        /// </summary>
        public bool PureCompression => Epsilon1 <= 0 && Epsilon2 < 0;

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> and <see cref="Epsilon2"/> are tensile strains.
        /// </summary>
        public bool PureTension => Epsilon1 > 0 && Epsilon2 >= 0;

        /// <summary>
        /// Returns true if <see cref="Epsilon1"/> is a tensile strain and <see cref="Epsilon2"/> is a compressive strain.
        /// </summary>
        public bool TensionCompression => Epsilon1 > 0 && Epsilon2 < 0;

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
	        Epsilon1 = !double.IsNaN(epsilon1) ? epsilon1 : 0;
	        Epsilon2 = !double.IsNaN(epsilon2) ? epsilon2 : 0;
	        Theta1   = !double.IsNaN(theta1)   ? theta1   : 0;
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
	        var (e1, e2) = StrainRelations.CalculatePrincipal(strainState.Vector);
	        var theta1   = StrainRelations.CalculatePrincipalAngles(strainState.Vector, e2).theta1;

			return new PrincipalStrainState(e1, e2, strainState.ThetaX + theta1);
        }

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
		        epsilon + "1 = " + $"{Epsilon1:0.##E+00}" + "\n" +
		        epsilon + "2 = " + $"{Epsilon2:0.##E+00}" + "\n" +
		        theta + "1 = "   + $"{Theta1:0.00}" + " rad";
        }

        public override int GetHashCode() => (int)(Epsilon1 * Epsilon2);
    }
}
