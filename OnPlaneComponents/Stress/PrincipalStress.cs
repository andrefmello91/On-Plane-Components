using System;
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
	public partial struct PrincipalStressState : IEquatable<PrincipalStressState>
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
        /// Get transformation <see cref="Matrix"/> from principal plane to horizontal plane.
        /// <para>See: <seealso cref="StressRelations.TransformationMatrix"/></para>
        /// </summary>
        public Matrix<double> TransformationMatrix => StressRelations.TransformationMatrix(Theta1);

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> is zero.
        /// </summary>
        public bool IsSigma1Zero => Sigma1 == 0;

        /// <summary>
        /// Returns true if <see cref="Sigma2"/> is zero.
        /// </summary>
        public bool IsSigma2Zero => Sigma2 == 0;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> and <see cref="Sigma2"/> are compressive stresses.
        /// </summary>
        public bool PureCompression => Sigma1 <= 0 && Sigma2 < 0;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> and <see cref="Sigma2"/> are tensile stresses.
        /// </summary>
        public bool PureTension => Sigma1 > 0 && Sigma2 >= 0;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> is a tensile stress and <see cref="Sigma2"/> is a compressive stress.
        /// </summary>
        public bool TensionCompression => Sigma1 > 0 && Sigma2 < 0;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> and <see cref="Sigma2"/> are zero.
        /// </summary>
        public bool IsZero => IsSigma1Zero && IsSigma2Zero;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> direction coincides to horizontal axis.
        /// </summary>
        public bool IsHorizontal => Theta1 == 0;

        /// <summary>
        /// Principal Stress object.
        /// </summary>
        /// <param name="sigma1">The maximum principal stress (positive for tensile).</param>
        /// <param name="sigma2">The minimum principal stress (positive for tensile).</param>
        /// <param name="theta1">The angle of <paramref name="sigma1"/>, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public PrincipalStressState(double sigma1, double sigma2, double theta1 = Constants.PiOver4, PressureUnit unit = PressureUnit.Megapascal)
        {
			_sigma1 = Pressure.From(!double.IsNaN(sigma1) ? sigma1 : 0, unit);
			_sigma2 = Pressure.From(!double.IsNaN(sigma2) ? sigma2 : 0, unit);
			Theta1  = !double.IsNaN(theta1) ? theta1 : 0;
        }

        /// <summary>
        /// Principal Stress object.
        /// </summary>
        /// <param name="sigma1">The maximum principal <see cref="Pressure"/> (positive for tensile).</param>
        /// <param name="sigma2">The minimum principal <see cref="Pressure"/> (positive for tensile).</param>
        /// <param name="theta1">The angle of <paramref name="sigma1"/>, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public PrincipalStressState(Pressure sigma1, Pressure sigma2, double theta1 = Constants.PiOver4, PressureUnit unit = PressureUnit.Megapascal)
        {
	        _sigma1 = sigma1.ToUnit(unit);
			_sigma2 = sigma2.ToUnit(unit);
			Theta1  = !double.IsNaN(theta1) ? theta1 : 0;
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
        /// Get principal stresses as an <see cref="Array"/>, in <see cref="Unit"/> considered.
        /// <para>[ Sigma1, Sigma2, 0 ]</para>
        /// </summary>
        public double[] AsArray() => new[] { Sigma1, Sigma2, 0 };

        /// <summary>
        /// Get principal stresses as a <see cref="Vector"/>, in <see cref="Unit"/> considered.
        /// <para>{ Sigma1, Sigma2, 0 }</para>
        /// </summary>
        public Vector<double> AsVector() => Vector.Build.DenseOfArray(AsArray());

        /// <summary>
        /// Get a <see cref="PrincipalStressState"/> with zero elements.
        /// </summary>
        public static PrincipalStressState Zero => new PrincipalStressState(0, 0);

        /// <summary>
        /// Get a <see cref="PrincipalStressState"/> from a <see cref="StressState"/>.
        /// </summary>
        /// <param name="stressState">The <see cref="StressState"/> to transform.</param>
        public static PrincipalStressState FromStress(StressState stressState)
        {
	        var (s1, s2) = StressRelations.CalculatePrincipal(stressState.AsVector());
	        var theta1   = StressRelations.CalculatePrincipalAngles(stressState.AsVector(), s2).theta1;

			return new PrincipalStressState(s1, s2, stressState.ThetaX + theta1);
        }

        /// <summary>
        /// Compare two <see cref="PrincipalStressState"/> objects.
        /// </summary>
        /// <param name="other">The other <see cref="PrincipalStressState"/> to compare.</param>
        public bool Equals(PrincipalStressState other) => Theta1 == other.Theta1 && Sigma1 == other.Sigma1 && Sigma2 == other.Sigma2;

        /// <summary>
        /// Compare a <see cref="PrincipalStressState"/> to a <see cref="StressState"/> object.
        /// </summary>
        /// <param name="other">The <see cref="StressState"/> to compare.</param>
        public bool Equals(StressState other) => Equals(FromStress(other));

        public override bool Equals(object obj)
        {
	        if (obj is PrincipalStressState other)
		        return Equals(other);

	        if (obj is StressState stress)
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
    }
}
