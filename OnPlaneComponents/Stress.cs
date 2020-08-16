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
		private readonly Pressure
			_sigmaX,
			_sigmaY,
			_tauXY,
			_sigma1,
			_sigma2;

		/// <summary>
        /// Get normal stress in X direction, in unit constructed.
        /// </summary>
		public double SigmaX => _sigmaX.Value;

		/// <summary>
        /// Get normal stress in Y direction, in unit constructed.
        /// </summary>
		public double SigmaY => _sigmaY.Value;

		/// <summary>
        /// Get shear stress, in unit constructed.
        /// </summary>
		public double TauXY => _tauXY.Value;

		/// <summary>
        /// Get the maximum principal stress angle, in radians.
        /// </summary>
		public double Theta1 { get; }

		/// <summary>
        /// Get the minimum principal stress angle, in radians.
        /// </summary>
		public double Theta2 { get; }

        /// <summary>
        /// Get the stress vector {SigmaX, SigmaY, TauXY}.
        /// </summary>
        public Vector<double> Vector => DenseVector.OfArray(new []{SigmaX, SigmaY, TauXY});

        /// <summary>
        /// Stress object
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="unit">The unit of stress (default: MPa).</param>
        public Stress(double sigmaX, double sigmaY, double tauXY, PressureUnit unit = PressureUnit.Megapascal)
		{
			_sigmaX = Pressure.From(sigmaX, unit);
			_sigmaY = Pressure.From(sigmaY, unit);
			_tauXY  = Pressure.From(tauXY,  unit);
			
			// Get principal stresses
			var (s1, s2) = PrincipalStresses(sigmaX, sigmaY, tauXY);
			_sigma1      = Pressure.From(s1, unit);
			_sigma2      = Pressure.From(s2, unit);

			// Get principal angles
			(Theta1, Theta2) = PrincipalAngles(sigmaX, sigmaY, tauXY, s2);
		}

        /// <summary>
        /// Calculate principal stresses.
        /// <para>sigma1 is the maximum stress and sigma2 is the minimum stress.</para>
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        public static (double sigma1, double sigma2) PrincipalStresses(double sigmaX, double sigmaY, double tauXY)
        {
	        if (sigmaX == 0 && sigmaY == 0 && tauXY == 0)
		        return (0, 0);

	        if (tauXY == 0)
		        return (Math.Max(sigmaX, sigmaY), Math.Min(sigmaX, sigmaY));

	        // Calculate radius and center of Mohr's Circle
	        double
		        cen = 0.5 * (sigmaX + sigmaY),
		        rad = Math.Sqrt(0.25 * (sigmaY - sigmaX) * (sigmaY - sigmaX) + tauXY * tauXY);

	        // Calculate principal strains in concrete
	        double
		        f1 = cen + rad,
		        f2 = cen - rad;

	        return
		        (f1, f2);
        }

        /// <summary>
        /// Calculate principal stresses.
        /// <para>sigma1 is the maximum stress and sigma2 is the minimum stress.</para>
        /// </summary>
        /// <param name="stresses">Vector of stresses.</param>
        public static (double sigma1, double sigma2) PrincipalStresses(Vector<double> stresses) =>
	        PrincipalStresses(stresses[0], stresses[1], stresses[2]);

        /// <summary>
        /// Calculate principal stresses angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="sigma2">Minimum principal stress, if known.</param>
        public static (double theta1, double theta2) PrincipalAngles(double sigmaX, double sigmaY, double tauXY, double? sigma2 = null)
        {
	        double theta1 = Constants.PiOver4;

	        if (sigmaX != 0 || sigmaY != 0 || tauXY != 0)
	        {
		        // Calculate the strain slope
		        if (tauXY == 0)
		        {
			        if (sigmaX >= sigmaY)
				        theta1 = 0;
			        else
				        theta1 = Constants.PiOver2;
		        }

		        else if (sigma2.HasValue)
		        {
			        var f2 = sigma2.Value;

			        if (Math.Abs(sigmaX - sigmaY) <= 1E-9 && tauXY < 0)
				        theta1 = -Constants.PiOver4;

			        else
				        theta1 = Constants.PiOver2 - Trig.Atan((sigmaX - f2) / tauXY);
		        }

		        else
		        {
			        if (sigmaX - sigmaY <= 1E-9 && tauXY < 0)
				        theta1 = -Constants.PiOver4;

			        else
				        theta1 = 0.5 * Trig.Atan(2 * tauXY / (sigmaX - sigmaY));
		        }

		        if (double.IsNaN(theta1))
			        theta1 = Constants.PiOver4;
	        }

            // Calculate theta2
            double theta2 = Constants.PiOver2 - theta1;

	        return
		        (theta1, theta2);
        }

        /// <summary>
        /// Calculate principal stresses angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="stresses">Vector of stresses.</param>
        /// <param name="sigma2">Minimum principal stress, if known.</param>
        public static (double theta1, double theta2) PrincipalAngles(Vector<double> stresses, double? sigma2 = null) =>
	        PrincipalAngles(stresses[0], stresses[1], stresses[2], sigma2);
    }
}
