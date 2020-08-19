using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
	/// <summary>
    /// Strain struct for XY components.
    /// </summary>
    public struct Strain : IEquatable<Strain>
    {
        /// <summary>
        /// Get/set normal strain in X direction.
        /// </summary>
        public double EpsilonX { get; set; }

        /// <summary>
        /// Get/set normal strain in Y direction.
        /// </summary>
        public double EpsilonY { get; set; }

        /// <summary>
        /// Get/set shear strain.
        /// </summary>
        public double GammaXY { get; set; }

        /// <summary>
        /// Get principal strains.
        /// <para>epsilon1 is the maximum strain and epsilon2 is the minimum strain.</para>
        /// </summary>
        public (double epsilon1, double epsilon2) PrincipalStrains => StrainRelations.CalculatePrincipal(EpsilonX, EpsilonY, GammaXY);

        /// <summary>
        /// Get principal strain angles, in radians.
        /// <para>theta1 is the maximum strain angle and theta2 is the minimum strain angle.</para>
        /// </summary>
        public (double theta1, double theta2) PrincipalAngles => StrainRelations.CalculatePrincipalAngles(EpsilonX, EpsilonY, GammaXY);

        /// <summary>
        /// Get the strain vector, in unit constructed (<see cref="Unit"/>).
        /// <para>{ EpsilonX, EpsilonY, GammaXY }</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new []{EpsilonX, EpsilonY, GammaXY});

		/// <summary>
        /// Get transformation matrix from principal strains to xy strains.
        /// </summary>
        public Matrix<double> TransformationMatrix => StrainRelations.TransformationMatrix(PrincipalAngles.theta1);

		/// <summary>
		/// Returns true if <see cref="EpsilonX"/> is zero.
		/// </summary>
		public bool IsEpsilonXZero => EpsilonX == 0;

		/// <summary>
		/// Returns true if <see cref="EpsilonY"/> is zero.
		/// </summary>
		public bool IsEpsilonYZero => EpsilonY == 0;

		/// <summary>
		/// Returns true if <see cref="GammaXY"/> is zero.
		/// </summary>
		public bool IsGammaXYZero => GammaXY == 0;

        /// <summary>
        /// Returns true if all components are zero.
		/// </summary>
        public bool IsZero => IsEpsilonXZero && IsEpsilonYZero && IsGammaXYZero;

		/// <summary>
		/// Returns true if pure shear state of strains.
		/// </summary>
		public bool IsPureShear => IsEpsilonXZero && IsEpsilonYZero && !IsGammaXYZero;

		/// <summary>
		/// Returns true if principal state of strains.
		/// </summary>
		public bool IsPrincipal => !IsEpsilonXZero && !IsEpsilonYZero && IsGammaXYZero;

        /// <summary>
        /// Strain object for XY components.
        /// </summary>
        /// <param name="epsilonX">The normal strain in X direction (positive for tensile).</param>
        /// <param name="epsilonY">The normal strain in Y direction (positive for tensile).</param>
        /// <param name="gammaXY">The shear strain (positive if right face of element displaces upwards).</param>
        public Strain(double epsilonX, double epsilonY, double gammaXY)
        {
	        EpsilonX = epsilonX;
	        EpsilonY = epsilonY;
	        GammaXY  = gammaXY;
        }

        /// <summary>
        /// Strain object for XY components.
        /// </summary>
        /// <param name="strainVector">The vector of strains.
        ///	<para>{EpsilonX, EpsilonY, GammaXY}</para></param>
        public Strain(Vector<double> strainVector)
        {
	        EpsilonX = strainVector[0];
	        EpsilonY = strainVector[1];
            GammaXY  = strainVector[2];
        }

        /// <summary>
        /// Add values to current strains.
        /// </summary>
        /// <param name="incrementX">The increment for X strain.</param>
        /// <param name="incrementY">The increment for Y strain.</param>
        /// <param name="incrementXY">The increment for shear strain.</param>
        public void Add(double incrementX, double incrementY, double incrementXY)
        {
	        EpsilonX += incrementX;
	        EpsilonY += incrementY;
	        GammaXY  += incrementXY;
        }

        /// <summary>
        /// Subtract values from current strains.
        /// </summary>
        /// <param name="decrementX">The decrement for X strain (positive value).</param>
        /// <param name="decrementY">The decrement for Y strain (positive value).</param>
        /// <param name="decrementXY">The decrement for shear strain (positive value).</param>
        public void Subtract(double decrementX, double decrementY, double decrementXY) 
        {
	        EpsilonX -= decrementX;
	        EpsilonY -= decrementY;
	        GammaXY  -= decrementXY;
        }

        /// <summary>
        /// Multiply current strains by a value.
        /// </summary>
        /// <param name="multiplier">The multiplier for all strain components.</param>
        public void Multiply(double multiplier) => Multiply(multiplier, multiplier, multiplier);

        /// <summary>
        /// Multiply current strains by values.
        /// </summary>
        /// <param name="multiplierX">The multiplier for X strain.</param>
        /// <param name="multiplierY">The multiplier for Y strain.</param>
        /// <param name="multiplierXY">The multiplier for shear strain.</param>
        public void Multiply(double multiplierX, double multiplierY, double multiplierXY)
        {
            EpsilonX *= multiplierX;
            EpsilonY *= multiplierY;
            GammaXY  *= multiplierXY;
        }

        /// <summary>
        /// Divide current strains by a value.
        /// </summary>
        /// <param name="divider">The divider for all strain components.</param>
        public void Divide(double divider) => Divide(divider, divider, divider);

        /// <summary>
        /// Divide current strains by values.
        /// </summary>
        /// <param name="dividerX">The divider for X strain.</param>
        /// <param name="dividerY">The divider for Y strain.</param>
        /// <param name="dividerXY">The divider for shear strain.</param>
        public void Divide(double dividerX, double dividerY, double dividerXY)
        {
            EpsilonX /= dividerX;
            EpsilonY /= dividerY;
            GammaXY  /= dividerXY;
        }

        /// <summary>
        /// Get a strain with zero elements.
        /// </summary>
        public static Strain Zero => new Strain(0, 0, 0);

        /// <summary>
        /// Get a strain from known principal strains values.
        /// </summary>
        /// <param name="epsilon1">Maximum principal strain.</param>
        /// <param name="epsilon2">Minimum principal strain.</param>
        /// <param name="theta1">Angle of the maximum principal strain, in radians.</param>
        public static Strain FromPrincipal(double epsilon1, double epsilon2, double theta1)
        {
	        var sVec = StrainRelations.StrainsFromPrincipal(epsilon1, epsilon2, theta1);

			return
				new Strain(sVec);
        }

        /// <summary>
        /// Compare two strain objects.
        /// </summary>
        /// <param name="other">The strain to compare.</param>
        public bool Equals(Strain other) => EpsilonX == other.EpsilonX && EpsilonY == other.EpsilonY && GammaXY == other.GammaXY;

        public override bool Equals(object obj)
        {
	        if (obj is Strain other)
		        return Equals(other);

	        return false;
        }

        public override string ToString()
        {
	        char
		        epsilon = (char) Characters.Epsilon,
		        gamma   = (char) Characters.Gamma;

	        return
		        epsilon + "x = "  + $"{EpsilonX:0.##E+00}" + "\n" +
		        epsilon + "y = "  + $"{EpsilonY:0.##E+00}" + "\n" +
		        gamma   + "xy = " + $"{GammaXY:0.##E+00}";
        }

        public override int GetHashCode() => (int)(EpsilonX * EpsilonY * GammaXY);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (Strain left, Strain right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (Strain left, Strain right) => !left.Equals(right);

        /// <summary>
        /// Returns a strain object with summed components.
        /// </summary>
        public static Strain operator + (Strain left, Strain right) => new Strain(left.Vector + right.Vector);

        /// <summary>
        /// Returns a strain object with subtracted components.
        /// </summary>
        public static Strain operator - (Strain left, Strain right) => new Strain(left.Vector - right.Vector);

        /// <summary>
        /// Returns a strain object with multiplied components by a double.
        /// </summary>
        public static Strain operator * (Strain strain, double multiplier) => new Strain(multiplier * strain.Vector);

        /// <summary>
        /// Returns a strain object with multiplied components by a double.
        /// </summary>
        public static Strain operator * (double multiplier, Strain strain) => strain * multiplier;

        /// <summary>
        /// Returns a strain object with multiplied components by an integer.
        /// </summary>
        public static Strain operator * (Strain strain, int multiplier) => strain * (double)multiplier;

        /// <summary>
        /// Returns a strain object with multiplied components by an integer.
        /// </summary>
        public static Strain operator * (int multiplier, Strain strain) => strain * (double)multiplier;

        /// <summary>
        /// Returns a strain object with components divided by a double.
        /// </summary>
        public static Strain operator / (Strain strain, double divider) => new Strain(strain.Vector / divider);

        /// <summary>
        /// Returns a strain object with components divided by an integer.
        /// </summary>
        public static Strain operator / (Strain strain, int divider) => strain / (double)divider;
	}
}
