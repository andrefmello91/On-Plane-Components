using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace OnPlaneComponents
{
	/// <summary>
    /// Strain struct for XY components.
    /// </summary>
    public partial struct StrainState : IEquatable<StrainState>
    {
		/// <summary>
		/// Get normal strain in X direction.
		/// </summary>
		public double EpsilonX { get; }

        /// <summary>
        /// Get normal strain in Y direction.
        /// </summary>
        public double EpsilonY { get; }

        /// <summary>
        /// Get shear strain.
        /// </summary>
        public double GammaXY { get; }

        /// <summary>
        /// Get the angle of X direction (<see cref="EpsilonX"/>), related to horizontal axis.
        /// </summary>
        public double ThetaX { get; }

        /// <summary>
        /// Get the angle of Y direction (<see cref="EpsilonY"/>), related to horizontal axis.
        /// </summary>
        public double ThetaY => ThetaX + Constants.PiOver2;

        /// <summary>
        /// Get transformation matrix from XY plane to horizontal plane.
        /// <para>See: <seealso cref="StrainRelations.TransformationMatrix"/></para>
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
		/// Returns true if <see cref="EpsilonX"/> direction coincides to horizontal axis.
		/// </summary>
		public bool IsHorizontal => ThetaX == 0;

        /// <summary>
        /// Strain object for XY components.
        /// </summary>
        /// <param name="epsilonX">The normal strain in X direction (positive for tensile).</param>
        /// <param name="epsilonY">The normal strain in Y direction (positive for tensile).</param>
        /// <param name="gammaXY">The shear strain (positive if right face of element displaces upwards).</param>
        /// <param name="thetaX">The angle of <paramref name="epsilonX"/> direction, related to horizontal axis (positive counterclockwise).</param>
        public StrainState(double epsilonX, double epsilonY, double gammaXY, double thetaX = 0)
        {
	        EpsilonX = !double.IsNaN(epsilonX) ? epsilonX : 0;
	        EpsilonY = !double.IsNaN(epsilonY) ? epsilonY : 0;
	        GammaXY  = !double.IsNaN(gammaXY)  ? gammaXY  : 0;
	        ThetaX   = !double.IsNaN(thetaX)   ? thetaX   : 0;
        }

        /// <summary>
        /// Get a <see cref="StrainState"/> with zero elements.
        /// </summary>
        public static StrainState Zero => new StrainState(0, 0, 0);

        /// <summary>
        /// Get strains as an <see cref="Array"/>.
        /// <para>[ EpsilonX, EpsilonY, GammaXY ]</para>
        /// </summary>
        public double[] AsArray() => new[] { EpsilonX, EpsilonY, GammaXY };

        /// <summary>
        /// Get strains as a <see cref="Vector"/>.
        /// <para>{ EpsilonX, EpsilonY, GammaXY }</para>
        /// </summary>
        public Vector<double> AsVector() => Vector.Build.DenseOfArray(AsArray());

        /// <summary>
        /// Get a <see cref="StrainState"/> from a <see cref="Vector"/> of strains.
        /// </summary>
        /// <param name="strainVector">The <see cref="Vector"/> of strains.
        ///	<para>{EpsilonX, EpsilonY, GammaXY}</para></param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis (positive counterclockwise).</param>
        public static StrainState FromVector(Vector<double> strainVector, double thetaX = 0) =>
			new StrainState(strainVector[0], strainVector[1], strainVector[2], thetaX);

        /// <summary>
        /// Get a <see cref="StrainState"/> from a <see cref="StressState"/>.
        /// </summary>
        /// <param name="stressState">The <see cref="StressState"/> to transform.</param>
        /// <param name="stiffnessMatrix">The stiffness <see cref="Matrix"/> (3 x 3), related to <paramref name="stressState"/> direction.</param>
        public static StrainState FromStresses(StressState stressState, Matrix<double> stiffnessMatrix)
        {
	        if (stressState.IsZero)
		        return Zero;

	        return FromVector(stiffnessMatrix.Solve(stressState.AsVector()), stressState.ThetaX);
        }

        /// <summary>
        /// Get <see cref="StrainState"/> transformed to horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        /// <param name="strainState">The <see cref="StrainState"/> to transform.</param>
        public static StrainState ToHorizontal(StrainState strainState)
        {
	        if (strainState.IsHorizontal)
		        return strainState;

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(strainState.AsVector(), - strainState.ThetaX);

			// Return with corrected angle
			return FromVector(sVec);
        }

		/// <summary>
        /// Get <see cref="StrainState"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="strainState">The <see cref="StrainState"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static StrainState Transform(StrainState strainState, double theta)
        {
	        if (theta == 0)
		        return strainState;

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(strainState.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, strainState.ThetaX + theta);
        }

        /// <summary>
        /// Get <see cref="StrainState"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="principalStrainState">The <see cref="PrincipalStrainState"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static StrainState Transform(PrincipalStrainState principalStrainState, double theta)
        {
	        if (theta == 0)
		        return FromVector(principalStrainState.AsVector(), principalStrainState.Theta1);

			// Get the strain vector transformed
			var sVec = StrainRelations.Transform(principalStrainState.AsVector(), theta);

			// Return with corrected angle
			return FromVector(sVec, principalStrainState.Theta1 + theta);
        }

        /// <summary>
        /// Get <see cref="StrainState"/> from a <see cref="PrincipalStrainState"/> in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        /// <param name="principalStrainState">The <see cref="PrincipalStrainState"/> to horizontal <see cref="StrainState"/>.</param>
        public static StrainState FromPrincipal(PrincipalStrainState principalStrainState)
        {
	        if (principalStrainState.Theta1 == 0)
		        return FromVector(principalStrainState.AsVector());

			// Get the strain vector transformed
			var sVec = StrainRelations.StrainsFromPrincipal(principalStrainState.Epsilon1, principalStrainState.Epsilon2, principalStrainState.Theta1);

			// Return with corrected angle
			return FromVector(sVec);
        }
		
		/// <summary>
        /// Compare two <see cref="StrainState"/> objects.
        /// </summary>
        /// <param name="other">The strain to compare.</param>
        public bool Equals(StrainState other) => ThetaX == other.ThetaX && EpsilonX == other.EpsilonX && EpsilonY == other.EpsilonY && GammaXY == other.GammaXY;

        /// <summary>
        /// Compare a <see cref="StrainState"/> to a <see cref="PrincipalStrainState"/> object.
        /// </summary>
        /// <param name="other">The <see cref="PrincipalStrainState"/> to compare.</param>
        public bool Equals(PrincipalStrainState other) => Equals(FromPrincipal(other));

        public override bool Equals(object obj)
        {
	        if (obj is StrainState other)
		        return Equals(other);

	        if (obj is PrincipalStrainState principalStrain)
		        return Equals(principalStrain);

	        return false;
        }

        public override string ToString()
        {
	        char
		        epsilon = (char) Characters.Epsilon,
		        gamma   = (char) Characters.Gamma,
	            theta   = (char)Characters.Theta;


            return
                epsilon + "x = "  + $"{EpsilonX:0.##E+00}" + "\n" +
		        epsilon + "y = "  + $"{EpsilonY:0.##E+00}" + "\n" +
		        gamma   + "xy = " + $"{GammaXY:0.##E+00}" + "\n"  +
                theta + "x = "    + $"{ThetaX:0.00}" + " rad";
        }

        public override int GetHashCode() => (int)(EpsilonX * EpsilonY * GammaXY);
    }
}
