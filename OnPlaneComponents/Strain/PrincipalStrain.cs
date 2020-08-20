using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace OnPlaneComponents
{
    /// <summary>
    /// Principal strain struct.
    /// </summary>
	public struct PrincipalStrain
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
        /// Get the angle of maximum principal strain, related to horizontal axis.
        /// </summary>
        public double Theta1 { get; }

        /// <summary>
        /// Get the angle of minimum principal strain, related to horizontal axis.
        /// </summary>
        public double Theta2 => Theta1 + Constants.PiOver2;

        /// <summary>
        /// Get principal strains as vector.
        /// <para>{ Epsilon1, Epsilon2, 0 }</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new[] { Epsilon1, Epsilon2, 0 });

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
        /// Returns true if maximum principal strain coincides to horizontal axis.
        /// </summary>
        public bool IsHorizontal => Theta1 == 0;

        /// <summary>
        /// Strain object for XY components.
        /// </summary>
        /// <param name="epsilon1">The maximum principal strain (positive for tensile).</param>
        /// <param name="epsilon2">The minimum principal strain (positive for tensile).</param>
        /// <param name="theta1">The angle of maximum principal strain, related to horizontal axis (positive to counterclockwise).</param>
        public PrincipalStrain(double epsilon1, double epsilon2, double theta1 = Constants.PiOver4)
        {
	        Epsilon1 = epsilon1;
	        Epsilon2 = epsilon2;
	        Theta1   = theta1;
        }

        /// <summary>
        /// Get a <see cref="PrincipalStrain"/> with zero elements.
        /// </summary>
        public static PrincipalStrain Zero => new PrincipalStrain(0, 0);

        /// <summary>
        /// Get a <see cref="PrincipalStrain"/> from a <see cref="Strain"/>.
        /// </summary>
        /// <param name="strain">The <see cref="Strain"/> to transform.</param>
        public static PrincipalStrain FromStrain(Strain strain)
        {
	        var (e1, e2) = StrainRelations.CalculatePrincipal(strain.Vector);
	        var theta1   = StrainRelations.CalculatePrincipalAngles(strain.Vector, e2).theta1;

			return new PrincipalStrain(e1, e2, strain.ThetaX + theta1);
        }

        /// <summary>
        /// Compare two <see cref="PrincipalStrain"/> objects.
        /// </summary>
        /// <param name="other">The other <see cref="PrincipalStrain"/> to compare.</param>
        public bool Equals(PrincipalStrain other) => Theta1 == other.Theta1 && Epsilon1 == other.Epsilon1 && Epsilon2 == other.Epsilon2;

        /// <summary>
        /// Compare a <see cref="PrincipalStrain"/> to a <see cref="Strain"/> object.
        /// </summary>
        /// <param name="other">The <see cref="Strain"/> to compare.</param>
        public bool Equals(Strain other) => Equals(FromStrain(other));

        public override bool Equals(object obj)
        {
	        if (obj is PrincipalStrain other)
		        return Equals(other);

	        if (obj is Strain strain)
		        return Equals(strain);

	        return false;
        }

        public override string ToString()
        {
	        char
		        epsilon = (char) Characters.Epsilon,
		        gamma   = (char) Characters.Gamma,
		        theta   = (char) Characters.Theta;

	        return
		        epsilon + "1 = " + $"{Epsilon1:0.##E+00}" + "\n" +
		        epsilon + "2 = " + $"{Epsilon2:0.##E+00}" + "\n" +
		        theta + "1 = "   + $"{Theta1:0.00}" + " rad";
        }

        public override int GetHashCode() => (int)(Epsilon1 * Epsilon2);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStrain left, PrincipalStrain right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStrain left, PrincipalStrain right) => !left.Equals(right);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStrain left, Strain right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStrain left, Strain right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="Strain"/> object with summed components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static Strain operator + (PrincipalStrain left, PrincipalStrain right) => Strain.FromPrincipal(left) + Strain.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="Strain"/> object with subtracted components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static Strain operator - (PrincipalStrain left, PrincipalStrain right) => Strain.FromPrincipal(left) - Strain.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="PrincipalStrain"/> object with multiplied components by a double.
        /// </summary>
        public static PrincipalStrain operator * (PrincipalStrain principalStrain, double multiplier) => new PrincipalStrain(multiplier * principalStrain.Epsilon1, multiplier * principalStrain.Epsilon2, principalStrain.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStrain"/> object with multiplied components by a double.
        /// </summary>
        public static PrincipalStrain operator *(double multiplier, PrincipalStrain strain) => strain * multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStrain"/> object with multiplied components by an integer.
        /// </summary>
        public static PrincipalStrain operator *(PrincipalStrain principalStrain, int multiplier) => principalStrain * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStrain"/> object with multiplied components by an integer.
        /// </summary>
        public static PrincipalStrain operator *(int multiplier, PrincipalStrain strain) => strain * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStrain"/> object with components divided by a double.
        /// </summary>
        public static PrincipalStrain operator /(PrincipalStrain principalStrain, double divider) => new PrincipalStrain(principalStrain.Epsilon1 / divider, principalStrain.Epsilon2 / divider, principalStrain.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStrain"/> object with components divided by an integer.
        /// </summary>
        public static PrincipalStrain operator /(PrincipalStrain principalStrain, int divider) => principalStrain / (double)divider;

    }
}
