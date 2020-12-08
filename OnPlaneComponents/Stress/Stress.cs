using System;
using Extensions.LinearAlgebra;
using Extensions.Number;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;
using static OnPlaneComponents.StressRelations;

namespace OnPlaneComponents
{
    /// <summary>
    /// Stress object for XY components.
    /// </summary>
    public partial struct StressState : IEquatable<StressState>
    {
		// Auxiliary fields
		private Pressure _sigmaX, _sigmaY, _tauXY;
		private Matrix<double> _transMatrix;

		/// <summary>
		/// Get a <see cref="StressState"/> with zero elements.
		/// </summary>
		public static readonly StressState Zero = new StressState(0, 0, 0);

        /// <summary>
        /// Get/set the stress unit (<see cref="PressureUnit"/>).
        /// </summary>
        public PressureUnit Unit => _sigmaX.Unit;

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
        /// Get transformation matrix from horizontal plane to XY plane.
        /// <para>See: <see cref="StressRelations.TransformationMatrix"/></para>
        /// </summary>
        public Matrix<double> TransformationMatrix => _transMatrix ?? CalculateTransformationMatrix();

		/// <summary>
        /// Returns true if <see cref="SigmaX"/> is zero.
        /// </summary>
		public bool IsSigmaXZero => SigmaX.ApproxZero();

		/// <summary>
        /// Returns true if <see cref="SigmaY"/> is zero.
        /// </summary>
		public bool IsSigmaYZero => SigmaY.ApproxZero();

		/// <summary>
        /// Returns true if <see cref="TauXY"/> is zero.
        /// </summary>
		public bool IsTauXYZero => TauXY.ApproxZero();

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
		public bool IsHorizontal => ThetaX.ApproxZero();

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="thetaX">The angle of <paramref name="sigmaX"/>, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public StressState(double sigmaX, double sigmaY, double tauXY, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
			: this (Pressure.From(sigmaX.ToZero(), unit), Pressure.From(sigmaY.ToZero(), unit), Pressure.From(tauXY.ToZero(), unit), thetaX)
		{
		}

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="thetaX">The angle of <paramref name="sigmaX"/>, related to horizontal axis (positive to counterclockwise).</param>
        public StressState(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, double thetaX = 0)
		{
			_sigmaX      = sigmaX;
			_sigmaY      = sigmaX.Unit == sigmaY.Unit ? sigmaY : sigmaY.ToUnit(sigmaX.Unit);
			_tauXY       = sigmaX.Unit == tauXY.Unit  ? tauXY  : tauXY.ToUnit(sigmaX.Unit);
            ThetaX       = thetaX.ToZero();
			_transMatrix = null;
		}

        /// <summary>
        /// Change the unit of stresses.
        /// </summary>
        /// <param name="toUnit">The <see cref="PressureUnit"/> to convert.</param>
        public void ChangeUnit(PressureUnit toUnit)
        {
			if (Unit == toUnit)
				return;

	        _sigmaX = _sigmaX.ToUnit(toUnit);
	        _sigmaY = _sigmaY.ToUnit(toUnit);
	        _tauXY  = _tauXY.ToUnit(toUnit);
        }

        /// <summary>
        /// Get the stresses as an <see cref="Array"/>, in unit constructed (<see cref="Unit"/>).
        /// <para>[ SigmaX, SigmaY, TauXY ]</para>
        /// </summary>
        public double[] AsArray() => new[] { SigmaX, SigmaY, TauXY };

        /// <summary>
        /// Get the stresses as <see cref="Vector"/>, in unit constructed (<see cref="Unit"/>).
        /// <para>{ SigmaX, SigmaY, TauXY }</para>
        /// </summary>
        public Vector<double> AsVector() => AsArray().ToVector();

        /// <summary>
        /// Return a copy of this <see cref="StressState"/>.
        /// </summary>
        public StressState Copy() => new StressState(SigmaX, SigmaY, TauXY, ThetaX, Unit);

        /// <summary>
        /// Calculate <see cref="TransformationMatrix"/>.
        /// </summary>
        private Matrix<double> CalculateTransformationMatrix()
        {
	        _transMatrix = StressRelations.TransformationMatrix(ThetaX);
	        return _transMatrix;
        }

        /// <summary>
        /// Get a <see cref="StressState"/> from a <see cref="Vector"/>.
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

			return FromVector(stiffnessMatrix * strainState.AsVector(), strainState.ThetaX);
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
			var sVec = StressRelations.Transform(stressState.AsVector(), -stressState.ThetaX);

			// Return with corrected angle
			return FromVector(sVec, 0, stressState.Unit);
		}

        /// <summary>
        /// Get <see cref="StressState"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="stressState">The <see cref="StressState"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static StressState Transform(StressState stressState, double theta)
        {
	        if (theta.ApproxZero())
		        return stressState;

	        // Get the strain vector transformed
	        var sVec = StressRelations.Transform(stressState.AsVector(), theta);

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
	        if (theta.ApproxZero())
		        return FromVector(principalStressState.AsVector(), principalStressState.Theta1, principalStressState.Unit);

	        // Get the strain vector transformed
	        var sVec = StressRelations.Transform(principalStressState.AsVector(), theta);

	        // Return with corrected angle
	        return FromVector(sVec, principalStressState.Theta1 + theta, principalStressState.Unit);
        }


        /// <summary>
        /// Get <see cref="StressState"/> from a <see cref="PrincipalStressState"/> in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        /// <param name="principalStressState">The <see cref="PrincipalStressState"/> to horizontal <see cref="StressState"/>.</param>
        public static StressState FromPrincipal(PrincipalStressState principalStressState)
        {
	        if (principalStressState.Theta1.ApproxZero())
		        return FromVector(principalStressState.AsVector());

	        // Get the strain vector transformed
	        var sVec = StressesFromPrincipal(principalStressState.Sigma1, principalStressState.Sigma2, principalStressState.Theta1);

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
	            $"{sigma}x = {_sigmaX}\n" +
	            $"{sigma}y = {_sigmaY}\n" +
	            $"{tau}xy = {_tauXY}\n" +
	            $"{theta}x = {ThetaX:0.00} rad";
        }

        public override int GetHashCode() => (int)(SigmaX * SigmaY * TauXY);
	}
}
