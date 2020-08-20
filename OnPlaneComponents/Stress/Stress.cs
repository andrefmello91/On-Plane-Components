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
    public struct Stress : IEquatable<Stress>
    {
		// Auxiliary fields
		private Pressure
			_sigmaX,
			_sigmaY,
			_tauXY;

		/// <summary>
        /// Get/set the stress unit.
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
        /// Get the angle of X direction, related to horizontal axis.
        /// </summary>
        public double ThetaX { get; }

        /// <summary>
        /// Get the angle of Y direction, related to horizontal axis.
        /// </summary>
        public double ThetaY => ThetaX + Constants.PiOver2;

        /// <summary>
        /// Get the stress vector, in unit constructed (<see cref="Unit"/>).
        /// <para>{SigmaX, SigmaY, TauXY}</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new []{SigmaX, SigmaY, TauXY});

        /// <summary>
        /// Get transformation matrix from XY plane to horizontal plane.
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
		/// Returns true if X direction coincides to horizontal axis.
		/// </summary>
		public bool IsHorizontal => ThetaX == 0;

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public Stress(double sigmaX, double sigmaY, double tauXY, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = Pressure.From(sigmaX, unit);
			_sigmaY = Pressure.From(sigmaY, unit);
			_tauXY  = Pressure.From(tauXY,  unit);
			ThetaX  = thetaX;
		}

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public Stress(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = sigmaX.ToUnit(unit);
			_sigmaY = sigmaY.ToUnit(unit);
			_tauXY  = tauXY.ToUnit(unit);
			ThetaX  = thetaX;
		}

        /// <summary>
        /// Stress object for XY components.
        /// </summary>
        /// <param name="stressVector">The vector of stresses, in considered <paramref name="unit"/>.
        ///	<para>{SigmaX, SigmaY, TauXY}</para></param>
        /// <param name="thetaX">The angle of X direction, related to horizontal axis.</param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public Stress(Vector<double> stressVector, double thetaX = 0, PressureUnit unit = PressureUnit.Megapascal)
        {
	        _sigmaX = Pressure.From(stressVector[0], unit);
	        _sigmaY = Pressure.From(stressVector[1], unit);
	        _tauXY  = Pressure.From(stressVector[2], unit);
	        ThetaX  = thetaX;
        }

        /// <summary>
        /// Change the unit of stresses.
        /// </summary>
        /// <param name="toUnit">The unit to convert (<see cref="PressureUnit"/>).</param>
        public void ChangeUnit(PressureUnit toUnit)
        {
	        Unit = toUnit;
        }

        /// <summary>
        /// Add values to current stresses.
        /// </summary>
        /// <param name="incrementX">The increment for X stress, in current unit (<see cref="Unit"/>).</param>
        /// <param name="incrementY">The increment for Y stress, in current unit (<see cref="Unit"/>).</param>
        /// <param name="incrementXY">The increment for shear stress, in current unit (<see cref="Unit"/>).</param>
        public void Add(double incrementX, double incrementY, double incrementXY) =>
            Add(Pressure.From(incrementX, Unit), Pressure.From(incrementY, Unit), Pressure.From(incrementXY, Unit));

        /// <summary>
        /// Add values to current stresses.
        /// </summary>
        /// <param name="incrementX">The force increment for X stress.</param>
        /// <param name="incrementY">The force increment for Y stress.</param>
        /// <param name="incrementXY">The increment for shear stress.</param>
        public void Add(Pressure incrementX, Pressure incrementY, Pressure incrementXY)
        {
            _sigmaX += incrementX;
            _sigmaY += incrementY;
            _tauXY  += incrementXY;
        }

        /// <summary>
        /// Subtract values from current stresses.
        /// </summary>
        /// <param name="decrementX">The decrement for X stress (positive value), in current unit (<see cref="Unit"/>).</param>
        /// <param name="decrementY">The decrement for Y stress (positive value), in current unit (<see cref="Unit"/>).</param>
        /// <param name="decrementXY">The decrement for shear stress (positive value), in current unit (<see cref="Unit"/>).</param>
        public void Subtract(double decrementX, double decrementY, double decrementXY) =>
            Subtract(Pressure.From(decrementX, Unit), Pressure.From(decrementY, Unit), Pressure.From(decrementXY, Unit));

        /// <summary>
        /// Subtract values from current stresses.
        /// </summary>
        /// <param name="decrementX">The force decrement for X stress (positive value).</param>
        /// <param name="decrementY">The force decrement for Y stress (positive value).</param>
        /// <param name="decrementXY">The decrement for shear stress (positive value).</param>
        public void Subtract(Pressure decrementX, Pressure decrementY, Pressure decrementXY)
        {
            _sigmaX -= decrementX;
            _sigmaY -= decrementY;
            _tauXY  -= decrementXY;
        }

        /// <summary>
        /// Multiply current stresses by a value.
        /// </summary>
        /// <param name="multiplier">The multiplier for all stress components.</param>
        public void Multiply(double multiplier) => Multiply(multiplier, multiplier, multiplier);

        /// <summary>
        /// Multiply current stresses by values.
        /// </summary>
        /// <param name="multiplierX">The multiplier for X stress.</param>
        /// <param name="multiplierY">The multiplier for Y stress.</param>
        /// <param name="multiplierXY">The multiplier for shear stress.</param>
        public void Multiply(double multiplierX, double multiplierY, double multiplierXY)
        {
            _sigmaX *= multiplierX;
            _sigmaY *= multiplierY;
            _tauXY  *= multiplierXY;
        }

        /// <summary>
        /// Divide current stresses by a value.
        /// </summary>
        /// <param name="divider">The divider for all stress components.</param>
        public void Divide(double divider) => Divide(divider, divider, divider);

        /// <summary>
        /// Divide current stresses by values.
        /// </summary>
        /// <param name="dividerX">The divider for X stress.</param>
        /// <param name="dividerY">The divider for Y stress.</param>
        /// <param name="dividerXY">The divider for shear stress.</param>
        public void Divide(double dividerX, double dividerY, double dividerXY)
        {
            _sigmaX /= dividerX;
            _sigmaY /= dividerY;
            _tauXY  /= dividerXY;
        }

		/// <summary>
        /// Get a stress with zero elements.
        /// </summary>
		public static Stress Zero => new Stress(0, 0, 0);

        /// <summary>
        /// Get <see cref="Stress"/> transformed by a rotation angle.
        /// </summary>
        /// <param name="stress">The <see cref="Stress"/> to transform.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static Stress Transform(Stress stress, double theta)
        {
	        if (theta == 0)
		        return stress;

	        // Get the strain vector transformed
	        var sVec = StressRelations.Transform(stress.Vector, theta);

	        // Return with corrected angle
	        return new Stress(sVec, stress.ThetaX + theta, stress.Unit);
        }

        /// <summary>
        /// Compare two stress objects.
        /// </summary>
        /// <param name="other">The stress to compare.</param>
        public bool Equals(Stress other) => _sigmaX == other._sigmaX && _sigmaY == other._sigmaY && _tauXY == other._tauXY;

        public override bool Equals(object obj)
        {
	        if (obj is Stress other)
		        return Equals(other);

	        return false;
        }

        public override string ToString()
        {
	        char
		        sigma = (char) Characters.Sigma,
		        tau   = (char) Characters.Tau;

	        return
		        sigma + "x = "  + _sigmaX + "\n" +
		        sigma + "y = "  + _sigmaY + "\n" +
		        tau   + "xy = " + _tauXY;
        }

        public override int GetHashCode() => (int)(SigmaX * SigmaY * TauXY);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (Stress left, Stress right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (Stress left, Stress right) => !left.Equals(right);

        /// <summary>
        /// Returns a stress object with summed components, in left argument's <see cref="Unit"/> and angle <see cref="ThetaX"/>.
        /// </summary>
        public static Stress operator + (Stress left, Stress right)
        {
			// Transform right argument
			var rTrans = Transform(right, left.ThetaX - right.ThetaX);

			return new Stress(left._sigmaX + rTrans._sigmaX, left._sigmaY + rTrans._sigmaY, left._tauXY + rTrans._tauXY, left.ThetaX, left.Unit);
        }

        /// <summary>
        /// Returns a stress object with subtracted components, in left argument's <see cref="Unit"/> and angle <see cref="ThetaX"/>.
        /// </summary>
        public static Stress operator - (Stress left, Stress right)
        {
	        // Transform right argument
	        var rTrans = Transform(right, left.ThetaX - right.ThetaX);

	        return new Stress(left._sigmaX - rTrans._sigmaX, left._sigmaY - rTrans._sigmaY, left._tauXY - rTrans._tauXY, left.ThetaX, left.Unit);
        }

        /// <summary>
        /// Returns a stress object with multiplied components by a double.
        /// </summary>
        public static Stress operator * (Stress stress, double multiplier) => new Stress(multiplier * stress._sigmaX, multiplier * stress._sigmaY, multiplier * stress._tauXY, stress.ThetaX, stress.Unit);

        /// <summary>
        /// Returns a stress object with multiplied components by a double.
        /// </summary>
        public static Stress operator * (double multiplier, Stress stress) => stress * multiplier;

        /// <summary>
        /// Returns a stress object with multiplied components by an integer.
        /// </summary>
        public static Stress operator * (Stress stress, int multiplier) => stress * (double)multiplier;

        /// <summary>
        /// Returns a stress object with multiplied components by an integer.
        /// </summary>
        public static Stress operator * (int multiplier, Stress stress) => stress * (double)multiplier;

        /// <summary>
        /// Returns a stress object with components divided by a double.
        /// </summary>
        public static Stress operator / (Stress stress, double divider) => new Stress(stress._sigmaX / divider, stress._sigmaY / divider, stress._tauXY / divider, stress.ThetaX, stress.Unit);

        /// <summary>
        /// Returns a stress object with components divided by an integer.
        /// </summary>
        public static Stress operator / (Stress stress, int divider) => stress / (double)divider;
	}
}
