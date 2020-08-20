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
		// Auxiliary fields
		private double _epsilonX, _epsilonY, _gammaXY;

		/// <summary>
		/// Get normal strain in X direction.
		/// </summary>
		public double EpsilonX => _epsilonX;

		/// <summary>
		/// Get normal strain in Y direction.
		/// </summary>
		public double EpsilonY => _epsilonY;

		/// <summary>
		/// Get shear strain.
		/// </summary>
		public double GammaXY => _gammaXY;

		/// <summary>
        /// Get the angle of X direction, related to horizontal axis.
        /// </summary>
		public double ThetaX { get; }

		/// <summary>
        /// Get the angle of Y direction, related to horizontal axis.
        /// </summary>
		public double ThetaY => ThetaX + Constants.PiOver2;

        /// <summary>
        /// Get the strain vector, in unit constructed.
        /// <para>{ EpsilonX, EpsilonY, GammaXY }</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new []{EpsilonX, EpsilonY, GammaXY});

		/// <summary>
        /// Get transformation matrix from XY plane to horizontal plane.
        /// </summary>
        public Matrix<double> TransformationMatrix => StrainRelations.TransformationMatrix(ThetaX);

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
		/// Returns true if X direction coincides to horizontal axis.
		/// </summary>
		public bool IsHorizontal => ThetaX == 0;

        /// <summary>
        /// Strain object for XY components.
        /// </summary>
        /// <param name="epsilonX">The normal strain in X direction (positive for tensile).</param>
        /// <param name="epsilonY">The normal strain in Y direction (positive for tensile).</param>
        /// <param name="gammaXY">The shear strain (positive if right face of element displaces upwards).</param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
        public Strain(double epsilonX, double epsilonY, double gammaXY, double thetaX = 0)
        {
	        _epsilonX = epsilonX;
	        _epsilonY = epsilonY;
	        _gammaXY  = gammaXY;
	        ThetaX    = thetaX;
        }

        /// <summary>
        /// Strain object for XY components.
        /// </summary>
        /// <param name="strainVector">The vector of strains.
        ///	<para>{EpsilonX, EpsilonY, GammaXY}</para></param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
        public Strain(Vector<double> strainVector, double thetaX = 0)
        {
	        _epsilonX = strainVector[0];
	        _epsilonY = strainVector[1];
            _gammaXY  = strainVector[2];
            ThetaX    = thetaX;
        }

        /// <summary>
        /// Add values to current strains.
        /// </summary>
        /// <param name="incrementX">The increment for X strain.</param>
        /// <param name="incrementY">The increment for Y strain.</param>
        /// <param name="incrementXY">The increment for shear strain.</param>
        public void Add(double incrementX, double incrementY, double incrementXY)
        {
	        _epsilonX += incrementX;
	        _epsilonY += incrementY;
	        _gammaXY  += incrementXY;
        }

        /// <summary>
        /// Subtract values from current strains.
        /// </summary>
        /// <param name="decrementX">The decrement for X strain (positive value).</param>
        /// <param name="decrementY">The decrement for Y strain (positive value).</param>
        /// <param name="decrementXY">The decrement for shear strain (positive value).</param>
        public void Subtract(double decrementX, double decrementY, double decrementXY) 
        {
	        _epsilonX -= decrementX;
	        _epsilonY -= decrementY;
	        _gammaXY  -= decrementXY;
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
            _epsilonX *= multiplierX;
            _epsilonY *= multiplierY;
            _gammaXY  *= multiplierXY;
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
            _epsilonX /= dividerX;
            _epsilonY /= dividerY;
            _gammaXY  /= dividerXY;
        }

        /// <summary>
        /// Get a strain with zero elements.
        /// </summary>
        public static Strain Zero => new Strain(0, 0, 0);

		/// <summary>
        /// Get <see cref="Strain"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="strain">The <see cref="Strain"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static Strain Transform(Strain strain, double theta)
        {
	        if (theta == 0)
		        return strain;

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(strain.Vector, theta);

			// Return with corrected angle
			return new Strain(sVec, strain.ThetaX + theta);
        }

        /// <summary>
        /// Compare two strain objects.
        /// </summary>
        /// <param name="other">The strain to compare.</param>
        public bool Equals(Strain other) => ThetaX == other.ThetaX && EpsilonX == other.EpsilonX && EpsilonY == other.EpsilonY && GammaXY == other.GammaXY;

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
        /// Returns a strain object with summed components, in left argument's direction <see cref="ThetaX"/>.
        /// </summary>
        public static Strain operator + (Strain left, Strain right)
        {
	        // Transform right argument
	        var rTrans = Transform(right, left.ThetaX - right.ThetaX);

            return new Strain(left.Vector + rTrans.Vector, left.ThetaX);
        }

        /// <summary>
        /// Returns a strain object with subtracted components, in left argument's direction <see cref="ThetaX"/>.
        /// </summary>
        public static Strain operator - (Strain left, Strain right)
        {
	        // Transform right argument
	        var rTrans = Transform(right, left.ThetaX - right.ThetaX);

            return new Strain(left.Vector - rTrans.Vector, left.ThetaX);
        }

        /// <summary>
        /// Returns a strain object with multiplied components by a double.
        /// </summary>
        public static Strain operator * (Strain strain, double multiplier) => new Strain(multiplier * strain.Vector, strain.ThetaX);

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
        public static Strain operator / (Strain strain, double divider) => new Strain(strain.Vector / divider, strain.ThetaX);

        /// <summary>
        /// Returns a strain object with components divided by an integer.
        /// </summary>
        public static Strain operator / (Strain strain, int divider) => strain / (double)divider;
	}
}
