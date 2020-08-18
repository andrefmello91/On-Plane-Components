using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
	/// <summary>
    /// Stress struct.
    /// </summary>
    public struct Stress : IEquatable<Stress>
    {
		// Auxiliar fields
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
        /// Get principal stresses, in unit constructed (<see cref="Unit"/>).
        /// <para>sigma1 is the maximum stress and sigma2 is the minimum stress.</para>
        /// </summary>
        public (double sigma1, double sigma2) PrincipalStresses => StressRelations.CalculatePrincipal(SigmaX, SigmaY, TauXY);

        /// <summary>
        /// Get principal stress angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        public (double theta1, double theta2) PrincipalAngles => StressRelations.CalculatePrincipalAngles(SigmaX, SigmaY, TauXY);

        /// <summary>
        /// Get the stress vector, in unit constructed (<see cref="Unit"/>).
        /// <para>{SigmaX, SigmaY, TauXY}</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new []{SigmaX, SigmaY, TauXY});

		/// <summary>
        /// Get transformation matrix from principal stresses to xy stresses.
        /// </summary>
        public Matrix<double> TransformationMatrix => StressRelations.TransformationMatrix(PrincipalAngles.theta1);

        /// <summary>
        /// Stress object
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public Stress(double sigmaX, double sigmaY, double tauXY, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = Pressure.From(sigmaX, unit);
			_sigmaY = Pressure.From(sigmaY, unit);
			_tauXY  = Pressure.From(tauXY,  unit);
		}

        /// <summary>
        /// Stress object
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public Stress(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = sigmaX.ToUnit(unit);
			_sigmaY = sigmaY.ToUnit(unit);
			_tauXY  = tauXY.ToUnit(unit);
		}

        /// <summary>
        /// Stress object
        /// </summary>
        /// <param name="stressVector">The vector of stresses, in considered <paramref name="unit"/>.
        ///	<para>{SigmaX, SigmaY, TauXY}</para></param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public Stress(Vector<double> stressVector, PressureUnit unit = PressureUnit.Megapascal)
        {
	        _sigmaX = Pressure.From(stressVector[0], unit);
	        _sigmaY = Pressure.From(stressVector[1], unit);
	        _tauXY  = Pressure.From(stressVector[2], unit);
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
        /// Transform this stresses by a rotation angle.
        /// </summary>
        /// <param name="theta">Rotation angle, in radians.</param>
        public void Transform(double theta)
        {
	        var sVec = StressRelations.Transform(Vector, theta);

	        _sigmaX = Pressure.From(sVec[0], Unit);
	        _sigmaY = Pressure.From(sVec[1], Unit);
	        _tauXY  = Pressure.From(sVec[2], Unit);
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
        /// Get a stress from known principal stresses values.
        /// </summary>
        /// <param name="sigma1">Maximum principal stress.</param>
        /// <param name="sigma2">Minimum principal stress.</param>
        /// <param name="theta1">Angle of the maximum principal stress, in radians.</param>
        /// <param name="unit">The unit of stress to return (default: MPa).</param>
        public static Stress FromPrincipal(Pressure sigma1, Pressure sigma2, double theta1, PressureUnit unit = PressureUnit.Megapascal)
        {
	        var sVec = StressRelations.StressesFromPrincipal(sigma1, sigma2, theta1);

			return
				new Stress(sVec, unit);
        }

        /// <summary>
        /// Get a stress from known principal stresses values.
        /// </summary>
        /// <param name="sigma1">Maximum principal stress.</param>
        /// <param name="sigma2">Minimum principal stress.</param>
        /// <param name="theta1">Angle of the maximum principal stress, in radians.</param>
        /// <param name="unit">The unit of stresses (default: MPa).</param>
        public static Stress FromPrincipal(double sigma1, double sigma2, double theta1, PressureUnit unit = PressureUnit.Megapascal)
        {
	        var sVec = StressRelations.StressesFromPrincipal(sigma1, sigma2, theta1);

			return
				new Stress(sVec, unit);
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
        /// Returns a stress object with summed components, in left argument's unit.
        /// </summary>
        public static Stress operator + (Stress left, Stress right) => new Stress(left._sigmaX + right._sigmaX, left._sigmaY + right._sigmaY, left._tauXY + right._tauXY, left.Unit);

        /// <summary>
        /// Returns a stress object with subtracted components, in left argument's unit.
        /// </summary>
        public static Stress operator - (Stress left, Stress right) => new Stress(left._sigmaX - right._sigmaX, left._sigmaY - right._sigmaY, left._tauXY - right._tauXY, left.Unit);

        /// <summary>
        /// Returns a stress object with multiplied components by a double.
        /// </summary>
        public static Stress operator * (Stress stress, double multiplier) => new Stress(multiplier * stress._sigmaX, multiplier * stress._sigmaY, multiplier * stress._tauXY, stress.Unit);

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
        public static Stress operator / (Stress stress, double divider) => new Stress(stress._sigmaX / divider, stress._sigmaY / divider, stress._tauXY / divider, stress.Unit);

        /// <summary>
        /// Returns a stress object with components divided by an integer.
        /// </summary>
        public static Stress operator / (Stress stress, int divider) => stress / (double)divider;
	}
}
