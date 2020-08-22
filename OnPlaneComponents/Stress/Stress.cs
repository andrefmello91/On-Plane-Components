using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// Stress object for XY components.
    /// </summary>
    public partial struct StressState : IEquatable<StressState>
    {
		// Auxiliary fields
		private Pressure _sigmaX, _sigmaY, _tauXY;

		/// <summary>
        /// Get/set the stress unit (<see cref="PressureUnit"/>).
        /// </summary>
		public PressureUnit Unit
		{
			get => _sigmaX.Unit;
			set
			{
				if (value != Unit)
				{
					_sigmaX.ToUnit(value);
					_sigmaY.ToUnit(value);
					_tauXY.ToUnit(value);
				}
			}
		}

        /// <summary>
        /// Get normal stress in X direction, in unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double SigmaX => _sigmaX.Value;

        /// <summary>
        /// Get normal stress in Y direction, in unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double SigmaY => _sigmaY.Value;

        /// <summary>
        /// Get shear stress, in unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double TauXY => _tauXY.Value;

        /// <summary>
        /// Get the angle of <see cref="SigmaX"/> direction, related to horizontal axis.
        /// </summary>
        public double ThetaX { get; }

        /// <summary>
        /// Get the angle of <see cref="SigmaY"/> direction, related to horizontal axis.
        /// </summary>
        public double ThetaY => ThetaX + Constants.PiOver2;

        /// <summary>
        /// Get the stress as <see cref="DenseVector"/>, in unit constructed (<see cref="Unit"/>).
        /// <para>{SigmaX, SigmaY, TauXY}</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new []{SigmaX, SigmaY, TauXY});

        /// <summary>
        /// Get transformation matrix from XY plane to horizontal plane.
        /// <para>See: <see cref="StressRelations.TransformationMatrix"/></para>
        /// </summary>
        public Matrix<double> TransformationMatrix => StressRelations.TransformationMatrix(ThetaX);

		/// <summary>
        /// Returns true if <see cref="SigmaX"/> is zero.
        /// </summary>
		public bool IsSigmaXZero => SigmaX == 0;

		/// <summary>
        /// Returns true if <see cref="SigmaY"/> is zero.
        /// </summary>
		public bool IsSigmaYZero => SigmaY == 0;

		/// <summary>
        /// Returns true if <see cref="TauXY"/> is zero.
        /// </summary>
		public bool IsTauXYZero => TauXY == 0;

		/// <summary>
        /// Returns true if all components are zero.
        /// </summary>
		public bool IsZero => IsSigmaXZero && IsSigmaYZero && IsTauXYZero;

		/// <summary>
        /// Returns true if pure shear state of stresses.
        /// </summary>
		public bool IsPureShear => IsSigmaXZero && IsSigmaYZero && !IsTauXYZero;

		/// <summary>
        /// Returns true if principal state of stresses.
        /// </summary>
		public bool IsPrincipal => !IsSigmaXZero && !IsSigmaYZero && IsTauXYZero;

		/// <summary>
		/// Returns true if <see cref="SigmaX"/> direction coincides to horizontal axis.
		/// </summary>
		public bool IsHorizontal => ThetaX == 0;

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="thetaX">The angle of <paramref name="sigmaX"/>, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public StressState(double sigmaX, double sigmaY, double tauXY, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = Pressure.From(!double.IsNaN(sigmaX) ? sigmaX : 0, unit);
			_sigmaY = Pressure.From(!double.IsNaN(sigmaY) ? sigmaY : 0, unit);
			_tauXY  = Pressure.From(!double.IsNaN(tauXY)  ? tauXY  : 0,  unit);
			ThetaX  = !double.IsNaN(thetaX) ? thetaX : 0;
		}

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="thetaX">The angle of <paramref name="sigmaX"/>, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public StressState(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = sigmaX.ToUnit(unit);
			_sigmaY = sigmaY.ToUnit(unit);
			_tauXY  = tauXY.ToUnit(unit);
			ThetaX  = !double.IsNaN(thetaX) ? thetaX : 0;
		}

        /// <summary>
        /// Change the unit of stresses.
        /// </summary>
        /// <param name="toUnit">The <see cref="PressureUnit"/> to convert.</param>
        public void ChangeUnit(PressureUnit toUnit)
        {
	        Unit = toUnit;
        }

		/// <summary>
        /// Get a <see cref="StressState"/> with zero elements.
        /// </summary>
		public static StressState Zero => new StressState(0, 0, 0);

        /// <summary>
        /// Get a <see cref="StressState"/> from a <see cref="DenseVector"/>.
		/// </summary>
        /// <param name="stressVector">The vector of stresses, in considered <paramref name="unit"/>.
        ///	<para>{SigmaX, SigmaY, TauXY}</para></param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public static StressState FromVector(Vector<double> stressVector, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal) => 
			new StressState(stressVector[0], stressVector[1], stressVector[2], thetaX, unit);

        /// <summary>
        /// Get a <see cref="StressState"/> from a <see cref="StrainState"/>.
        /// </summary>
        /// <param name="strainState">The <see cref="StrainState"/> to transform.</param>
        /// <param name="stiffnessMatrix">The stiffness <see cref="Matrix"/> (3 x 3), related to <paramref name="strainState"/> direction.</param>
        public static StressState FromStrains(StrainState strainState, Matrix<double> stiffnessMatrix)
        {
	        if (strainState.IsZero)
		        return Zero;

			return FromVector(stiffnessMatrix * strainState.Vector, strainState.ThetaX);
        }

        /// <summary>
        /// Get <see cref="StressState"/> transformed to horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        /// <param name="stressState">The <see cref="StressState"/> to transform.</param>
        public static StressState ToHorizontal(StressState stressState)
		{
			if (stressState.IsHorizontal)
				return stressState;

			// Get the strain vector transformed
			var sVec = StressRelations.Transform(stressState.Vector, -stressState.ThetaX);

			// Return with corrected angle
			return FromVector(sVec);
		}

        /// <summary>
        /// Get <see cref="StressState"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="stressState">The <see cref="StressState"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static StressState Transform(StressState stressState, double theta)
        {
	        if (theta == 0)
		        return stressState;

	        // Get the strain vector transformed
	        var sVec = StressRelations.Transform(stressState.Vector, theta);

	        // Return with corrected angle
	        return FromVector(sVec, stressState.ThetaX + theta, stressState.Unit);
        }

        /// <summary>
        /// Get <see cref="PrincipalStressState"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="principalStressState">The <see cref="PrincipalStressState"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static StressState Transform(PrincipalStressState principalStressState, double theta)
        {
	        if (theta == 0)
		        return FromVector(principalStressState.Vector, principalStressState.Theta1, principalStressState.Unit);

	        // Get the strain vector transformed
	        var sVec = StressRelations.Transform(principalStressState.Vector, theta);

	        // Return with corrected angle
	        return FromVector(sVec, principalStressState.Theta1 + theta, principalStressState.Unit);
        }


        /// <summary>
        /// Get <see cref="StressState"/> from a <see cref="PrincipalStressState"/> in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        /// <param name="principalStressState">The <see cref="PrincipalStressState"/> to horizontal <see cref="StressState"/>.</param>
        public static StressState FromPrincipal(PrincipalStressState principalStressState)
        {
	        if (principalStressState.Theta1 == 0)
		        return FromVector(principalStressState.Vector);

	        // Get the strain vector transformed
	        var sVec = StressRelations.StressesFromPrincipal(principalStressState.Sigma1, principalStressState.Sigma2, principalStressState.Theta1);

	        // Return with corrected angle
	        return FromVector(sVec, 0, principalStressState.Unit);
        }

        /// <summary>
        /// Compare two <see cref="StressState"/> objects.
        /// </summary>
        /// <param name="other">The <see cref="StressState"/> to compare.</param>
        public bool Equals(StressState other) => ThetaX == other.ThetaX && _sigmaX == other._sigmaX && _sigmaY == other._sigmaY && _tauXY == other._tauXY;

        /// <summary>
        /// Compare a <see cref="StressState"/> to a <see cref="PrincipalStressState"/> object.
        /// </summary>
        /// <param name="other">The <see cref="PrincipalStressState"/> to compare.</param>
        public bool Equals(PrincipalStressState other) => Equals(FromPrincipal(other));

        public override bool Equals(object obj)
        {
	        if (obj is StressState other)
		        return Equals(other);

	        if (obj is PrincipalStressState principalStress)
		        return Equals(principalStress);

	        return false;
        }

        public override string ToString()
        {
	        char
		        sigma = (char) Characters.Sigma,
		        tau   = (char) Characters.Tau,
		        theta = (char)Characters.Theta;

            return
                sigma + "x = "  + _sigmaX + "\n" +
		        sigma + "y = "  + _sigmaY + "\n" +
		        tau   + "xy = " + _tauXY  + "\n" +
                theta + "x = "  + $"{ThetaX:0.00}" + " rad";
        }

        public override int GetHashCode() => (int)(SigmaX * SigmaY * TauXY);
	}
}
