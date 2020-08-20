using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// Principal stress struct.
    /// </summary>
	public struct PrincipalStress
    {
	    // Auxiliary fields
	    private Pressure _sigma1, _sigma2;

	    /// <summary>
	    /// Get/set the stress unit (<see cref="PressureUnit"/>).
	    /// </summary>
	    public PressureUnit Unit
	    {
		    get => _sigma1.Unit;
		    set
		    {
			    if (value != Unit)
			    {
				    _sigma1.ToUnit(value);
				    _sigma2.ToUnit(value);
			    }
		    }
	    }

	    /// <summary>
	    /// Get maximum principal stress, in <see cref="Unit"/> considered.
	    /// </summary>
	    public double Sigma1 => _sigma1.Value;

        /// <summary>
        /// Get minimum principal stress, in <see cref="Unit"/> considered..
	    /// </summary>
        public double Sigma2 => _sigma2.Value;

        /// <summary>
        /// Get the angle of maximum principal stress <see cref="Sigma1"/>, related to horizontal axis.
        /// </summary>
        public double Theta1 { get; }

        /// <summary>
        /// Get the angle of minimum principal stress <see cref="Sigma2"/>, related to horizontal axis.
        /// </summary>
        public double Theta2 => Theta1 + Constants.PiOver2;

        /// <summary>
        /// Get principal stresses as vector, in <see cref="Unit"/> considered..
        /// <para>{ Sigma1, Sigma2, 0 }</para>
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new[] { Sigma1, Sigma2, 0 });

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> is zero.
        /// </summary>
        public bool IsSigma1Zero => Sigma1 == 0;

        /// <summary>
        /// Returns true if <see cref="Sigma2"/> is zero.
        /// </summary>
        public bool IsSigma2Zero => Sigma2 == 0;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> and <see cref="Sigma2"/> are zero.
        /// </summary>
        public bool IsZero => IsSigma1Zero && IsSigma2Zero;

        /// <summary>
        /// Returns true if direction maximum principal stress <see cref="Theta1"/> coincides to horizontal axis.
        /// </summary>
        public bool IsHorizontal => Theta1 == 0;

        /// <summary>
        /// Principal Stress object.
        /// </summary>
        /// <param name="sigma1">The maximum principal stress (positive for tensile).</param>
        /// <param name="sigma2">The minimum principal stress (positive for tensile).</param>
        /// <param name="theta1">The angle of maximum principal stress, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of <paramref name="sigma1"/> and <paramref name="sigma2"/>.</param>
        public PrincipalStress(double sigma1, double sigma2, double theta1 = Constants.PiOver4, PressureUnit unit = PressureUnit.Megapascal)
        {
			_sigma1 = Pressure.From(sigma1, unit);
			_sigma2 = Pressure.From(sigma2, unit);
			Theta1  = theta1;
        }

        /// <summary>
        /// Principal Stress object.
        /// </summary>
        /// <param name="sigma1">The maximum principal <see cref="Pressure"/> (positive for tensile).</param>
        /// <param name="sigma2">The minimum principal <see cref="Pressure"/> (positive for tensile).</param>
        /// <param name="theta1">The angle of maximum principal stress, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of <paramref name="sigma1"/> and <paramref name="sigma2"/>.</param>
        public PrincipalStress(Pressure sigma1, Pressure sigma2, double theta1 = Constants.PiOver4, PressureUnit unit = PressureUnit.Megapascal)
        {
	        _sigma1 = sigma1.ToUnit(unit);
			_sigma2 = sigma2.ToUnit(unit);
			Theta1  = theta1;
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
        /// Get a <see cref="PrincipalStress"/> with zero elements.
        /// </summary>
        public static PrincipalStress Zero => new PrincipalStress(0, 0);

        /// <summary>
        /// Get a <see cref="PrincipalStress"/> from a <see cref="Stress"/>.
        /// </summary>
        /// <param name="stress">The <see cref="Stress"/> to transform.</param>
        public static PrincipalStress FromStress(Stress stress)
        {
	        var (s1, s2) = StressRelations.CalculatePrincipal(stress.Vector);
	        var theta1   = StressRelations.CalculatePrincipalAngles(stress.Vector, s2).theta1;

			return new PrincipalStress(s1, s2, stress.ThetaX + theta1);
        }

        /// <summary>
        /// Compare two <see cref="PrincipalStress"/> objects.
        /// </summary>
        /// <param name="other">The other <see cref="PrincipalStress"/> to compare.</param>
        public bool Equals(PrincipalStress other) => Theta1 == other.Theta1 && Sigma1 == other.Sigma1 && Sigma2 == other.Sigma2;

        /// <summary>
        /// Compare a <see cref="PrincipalStress"/> to a <see cref="Stress"/> object.
        /// </summary>
        /// <param name="other">The <see cref="Stress"/> to compare.</param>
        public bool Equals(Stress other) => Equals(FromStress(other));

        public override bool Equals(object obj)
        {
	        if (obj is PrincipalStress other)
		        return Equals(other);

	        if (obj is Stress stress)
		        return Equals(stress);

	        return false;
        }

        public override string ToString()
        {
	        char
		        sigma = (char) Characters.Sigma,
		        theta = (char) Characters.Theta;

	        return
		        sigma + "1 = " + _sigma1 + "\n" +
		        sigma + "2 = " + _sigma2 + "\n" +
		        theta + "1 = "   + $"{Theta1:0.00}" + " rad";
        }

        public override int GetHashCode() => (int)(Sigma1 * Sigma2);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStress left, PrincipalStress right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStress left, PrincipalStress right) => !left.Equals(right);

        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (PrincipalStress left, Stress right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (PrincipalStress left, Stress right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="Stress"/> object with summed components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static Stress operator + (PrincipalStress left, PrincipalStress right) => Stress.FromPrincipal(left) + Stress.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="Stress"/> object with subtracted components, in horizontal direction (<see cref="ThetaX"/> = 0).
        /// </summary>
        public static Stress operator - (PrincipalStress left, PrincipalStress right) => Stress.FromPrincipal(left) - Stress.FromPrincipal(right);

        /// <summary>
        /// Returns a <see cref="PrincipalStress"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStress operator * (PrincipalStress principalStrain, double multiplier) => new PrincipalStress(multiplier * principalStrain.Sigma1, multiplier * principalStrain.Sigma2, principalStrain.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStress"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStress operator *(double multiplier, PrincipalStress stress) => stress * multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStress"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStress operator *(PrincipalStress principalStrain, int multiplier) => principalStrain * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStress"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStress operator *(int multiplier, PrincipalStress stress) => stress * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="PrincipalStress"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static PrincipalStress operator /(PrincipalStress principalStrain, double divider) => new PrincipalStress(principalStrain.Sigma1 / divider, principalStrain.Sigma2 / divider, principalStrain.Theta1);

        /// <summary>
        /// Returns a <see cref="PrincipalStress"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static PrincipalStress operator /(PrincipalStress principalStrain, int divider) => principalStrain / (double)divider;

    }
}
