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
    /// Principal stress struct.
    /// </summary>
	public partial struct PrincipalStressState : IPlaneComponent<PrincipalStressState, PressureUnit>, IEquatable<PrincipalStressState>
    {
	    // Auxiliary fields
	    private Pressure _sigma1, _sigma2;
	    private Matrix<double> _transMatrix;

	    /// <summary>
	    /// Get a <see cref="PrincipalStressState"/> with zero elements.
	    /// </summary>
	    public static readonly  PrincipalStressState Zero = new PrincipalStressState(0, 0);

        /// <summary>
        /// Get/set the stress unit (<see cref="PressureUnit"/>).
        /// </summary>
        public PressureUnit Unit
        {
	        get => _sigma1.Unit;
	        set => ChangeUnit(value);
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
        /// Get transformation <see cref="Matrix"/> from horizontal plane to principal plane.
        /// <para>See: <seealso cref="StressRelations.TransformationMatrix"/></para>
        /// </summary>
        public Matrix<double> TransformationMatrix => _transMatrix ?? CalculateTransformationMatrix();

        /// <summary>
        /// Get the <see cref="PrincipalCase"/> of <seealso cref="PrincipalStressState"/>.
        /// </summary>
        public PrincipalCase Case
        {
	        get
	        {
		        if (IsZero)
			        return PrincipalCase.Zero;

		        if (Sigma1 > 0 && Sigma2 >= 0)
			        return PrincipalCase.PureTension;

		        if (Sigma1 <= 0 && Sigma2 < 0)
			        return PrincipalCase.PureCompression;

		        return PrincipalCase.TensionCompression;
	        }
        }

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> is zero.
        /// </summary>
        public bool IsSigma1Zero => Sigma1.ApproxZero();

        /// <summary>
        /// Returns true if <see cref="Sigma2"/> is zero.
        /// </summary>
        public bool IsSigma2Zero => Sigma2.ApproxZero();

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> and <see cref="Sigma2"/> are zero.
        /// </summary>
        public bool IsZero => IsSigma1Zero && IsSigma2Zero;

        /// <summary>
        /// Returns true if <see cref="Sigma1"/> direction coincides to horizontal axis.
        /// </summary>
        public bool IsHorizontal => Theta1.ApproxZero();

        /// <summary>
        /// Principal Stress object.
        /// </summary>
        /// <param name="sigma1">The maximum principal stress (positive for tensile).</param>
        /// <param name="sigma2">The minimum principal stress (positive for tensile).</param>
        /// <param name="theta1">The angle of <paramref name="sigma1"/>, related to horizontal axis (positive to counterclockwise).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stresses (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public PrincipalStressState(double sigma1, double sigma2, double theta1 = Constants.PiOver4, PressureUnit unit = PressureUnit.Megapascal)
			: this(Pressure.From(sigma1.ToZero(), unit), Pressure.From(sigma2.ToZero(), unit), theta1)
        {
        }

        /// <summary>
        /// Principal Stress object.
        /// </summary>
        /// <param name="sigma1">The maximum principal <see cref="Pressure"/> (positive for tensile).</param>
        /// <param name="sigma2">The minimum principal <see cref="Pressure"/> (positive for tensile).</param>
        /// <inheritdoc cref="PrincipalStressState(double, double, double, PressureUnit)"/>
        public PrincipalStressState(Pressure sigma1, Pressure sigma2, double theta1 = Constants.PiOver4)
        {
	        _sigma1      = sigma1;
			_sigma2      = sigma1.Unit == sigma2.Unit ? sigma2 : sigma2.ToUnit(sigma1.Unit);
			Theta1       = theta1.ToZero();
			_transMatrix = null;
        }

        /// <summary>
        /// Change the <see cref="PressureUnit"/> of this <see cref="PrincipalStressState"/>.
        /// </summary>
        /// <param name="unit">The <see cref="PressureUnit"/> to convert.</param>
        public void ChangeUnit(PressureUnit unit)
        {
			if (Unit == unit)
				return;

	        _sigma1 = _sigma1.ToUnit(unit);
	        _sigma2 = _sigma2.ToUnit(unit);
        }

        /// <summary>
        /// Convert this <see cref="PrincipalStressState"/> to another <see cref="PressureUnit"/>.
        /// </summary>
        /// <inheritdoc cref="ChangeUnit"/>
        public PrincipalStressState Convert(PressureUnit unit) => unit == Unit
	        ? this
	        : new PrincipalStressState(_sigma1.ToUnit(unit), _sigma2.ToUnit(unit), Theta1);

        /// <summary>
        /// Get principal stresses as an <see cref="Array"/>, in <see cref="Unit"/> considered.
        /// <para>[ Sigma1, Sigma2, 0 ]</para>
        /// </summary>
        public double[] AsArray() => new[] { Sigma1, Sigma2, 0 };

        /// <summary>
        /// Get principal stresses as a <see cref="Vector"/>, in <see cref="Unit"/> considered.
        /// <para>{ Sigma1, Sigma2, 0 }</para>
        /// </summary>
        public Vector<double> AsVector() => AsArray().ToVector();

        /// <summary>
        /// Return a copy of this <see cref="PrincipalStressState"/>.
        /// </summary>
        public PrincipalStressState Copy() => new PrincipalStressState(Sigma1, Sigma2, Theta1, Unit);

		/// <summary>
        /// Calculate <see cref="TransformationMatrix"/>.
        /// </summary>
        private Matrix<double> CalculateTransformationMatrix()
        {
	        _transMatrix = StressRelations.TransformationMatrix(Theta1);
	        return _transMatrix;
        }

        /// <summary>
        /// Get a <see cref="PrincipalStressState"/> from a <see cref="StressState"/>.
        /// </summary>
        /// <param name="stressState">The <see cref="StressState"/> to transform.</param>
        public static PrincipalStressState FromStress(StressState stressState)
        {
	        var (s1, s2) = CalculatePrincipal(stressState.AsVector());
	        var theta1   = CalculatePrincipalAngles(stressState.AsVector(), s2).theta1;

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
	        switch (obj)
	        {
		        case PrincipalStressState other:
			        return Equals(other);

		        case StressState stress:
			        return Equals(stress);

		        default:
			        return false;
	        }
        }

        public override string ToString()
        {
	        char
		        sigma = (char) Characters.Sigma,
		        theta = (char) Characters.Theta;

	        return
		        $"{sigma}1 = {_sigma1}\n" +
		        $"{sigma}2 = {_sigma2}\n" +
		        $"{theta}1 = {Theta1:0.00} rad";
        }

        public override int GetHashCode() => (int)(Sigma1 * Sigma2);
    }
}
