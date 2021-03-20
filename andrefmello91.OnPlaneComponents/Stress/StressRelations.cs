using System;
using Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents.Stress
{
    public static class StressRelations
    {
        /// <summary>
        /// Get a <see cref="Vector"/> of stresses transformed by a rotation angle.
        /// </summary>
        /// <param name="stresses"><see cref="Vector"/> of stresses.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static Vector<double> Transform(Vector<double> stresses, double theta)
		{
            // Get the stresses
            var f = stresses;

            var (cos2Theta, sin2Theta) = (2 * theta).DirectionCosines();

			// Calculate radius and center of Mohr's Circle
			double
				a = 0.5 * (f[0] + f[1]),
				b = 0.5 * (f[0] - f[1]) * cos2Theta,
				c = f[2] * sin2Theta,
				d = 0.5 * (f[0] - f[1]) * sin2Theta,
				e = f[2] * cos2Theta;

            return
               new []
				{
					a + b + c,
					a - b - c,
					e - d
				}.ToVector();
		}

        /// <summary>
        /// Calculate principal stresses.
        /// <para>sigma1 is the maximum stress and sigma2 is the minimum stress.</para>
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        public static (double sigma1, double sigma2) CalculatePrincipal(double sigmaX, double sigmaY, double tauXY)
        {
            if (sigmaX.ApproxZero() && sigmaY.ApproxZero() && tauXY.ApproxZero())
                return (0, 0);

            if (tauXY.ApproxZero())
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
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="unit">The <see cref="PressureUnit"/> of stress to return (default: <see cref="PressureUnit.Megapascal"/>).</param>
        public static (Pressure sigma1, Pressure sigma2) CalculatePrincipal(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, PressureUnit unit = PressureUnit.Megapascal)
        {
            var (s1, s2) = CalculatePrincipal(sigmaX.Value, sigmaY.ToUnit(sigmaX.Unit).Value, tauXY.ToUnit(sigmaX.Unit).Value);

            return
                (Pressure.From(s1, unit), Pressure.From(s2, unit));
        }

        /// <summary>
        /// Calculate principal stresses.
        /// <para>sigma1 is the maximum stress and sigma2 is the minimum stress.</para>
        /// </summary>
        /// <param name="stresses"><see cref="Vector"/> of stresses.</param>
        public static (double sigma1, double sigma2) CalculatePrincipal(Vector<double> stresses) =>
            CalculatePrincipal(stresses[0], stresses[1], stresses[2]);

        /// <summary>
        /// Calculate principal stresses angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="sigma2">Minimum principal stress, if known.</param>
        public static (double theta1, double theta2) CalculatePrincipalAngles(double sigmaX, double sigmaY, double tauXY, double? sigma2 = null)
        {
            double theta1 = Constants.PiOver4;

            if (!sigmaX.ApproxZero() || !sigmaY.ApproxZero() || !tauXY.ApproxZero())
            {
                // Calculate the strain slope
                if (tauXY.ApproxZero())
                {
                    if (sigmaX >= sigmaY)
                        theta1 = 0;
                    else
                        theta1 = Constants.PiOver2;
                }

                else if (sigmaX.Approx(sigmaY) && tauXY < 0)
	                theta1 = -Constants.PiOver4;

                else if (sigma2.HasValue)
                {
                    var f2 = sigma2.Value;
	                theta1 = Constants.PiOver2 - ((sigmaX - f2) / tauXY).Atan();
                }

                else
	                theta1 = Constants.PiOver2 - 0.5 * (2 * tauXY / (sigmaY - sigmaX)).Atan();

                if (double.IsNaN(theta1))
                    theta1 = Constants.PiOver4;
            }

            // Calculate theta2
            double theta2 = Constants.PiOver2 + theta1;

            return
                (theta1, theta2);
        }

        /// <summary>
        /// Calculate principal stresses angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="sigmaX">The normal stress in X direction (positive for tensile).</param>
        /// <param name="sigmaY">The normal stress in Y direction (positive for tensile).</param>
        /// <param name="tauXY">The shear stress (positive if upwards in right face of element).</param>
        /// <param name="sigma2">Minimum principal stress, if known.</param>
        public static (double theta1, double theta2) CalculatePrincipalAngles(Pressure sigmaX, Pressure sigmaY, Pressure tauXY, Pressure? sigma2 = null)
        {
            double? s2 = null;
            if (sigma2.HasValue)
                s2 = sigma2.Value.ToUnit(sigmaX.Unit).Value;

            return
                CalculatePrincipalAngles(sigmaX.Value, sigmaY.ToUnit(sigmaX.Unit).Value, tauXY.ToUnit(sigmaX.Unit).Value, s2);
        }

        /// <summary>
        /// Calculate principal stresses angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="stresses">Vector of stresses.</param>
        /// <param name="sigma2">Minimum principal stress, if known.</param>
        public static (double theta1, double theta2) CalculatePrincipalAngles(Vector<double> stresses, double? sigma2 = null) =>
            CalculatePrincipalAngles(stresses[0], stresses[1], stresses[2], sigma2);

        /// <summary>
        /// Calculate the <see cref="Vector"/> of stresses, in X and Y, from principal stresses.
        /// </summary>
        /// <param name="sigma1">Maximum principal stress.</param>
        /// <param name="sigma2">Minimum principal stress.</param>
        /// <param name="theta1">Angle of <paramref name="sigma1"/>, in radians.</param>
		public static Vector<double> StressesFromPrincipal(double sigma1, double sigma2, double theta1)
		{
			// Calculate theta2
			var theta2     = Constants.PiOver2 - theta1;
			var (cos, sin) = (2 * theta2).DirectionCosines();

			// Calculate stresses by Mohr's Circle
			double
				cen  = 0.5 * (sigma1 + sigma2),
				rad  = 0.5 * (sigma1 - sigma2),
				fx   = cen - rad * cos,
				fy   = cen + rad * cos,
				fxy  = rad * sin;

			return
				new[] { fx, fy, fxy }.ToVector();
        }

        /// <summary>
        /// Calculate the <see cref="Vector"/> of stresses, in X and Y, from principal stresses.
        /// </summary>
        /// <param name="sigma1">Maximum principal stress.</param>
        /// <param name="sigma2">Minimum principal stress.</param>
        /// <param name="theta1">Angle of the maximum principal stress, in radians.</param>
		/// <param name="unit">The unit of stresses (default: MPa).</param>
        public static Vector<double> StressesFromPrincipal(Pressure sigma1, Pressure sigma2, double theta1, PressureUnit unit = PressureUnit.Megapascal) =>
	        StressesFromPrincipal(sigma1.ToUnit(unit).Value, sigma2.ToUnit(unit).Value, theta1);
		
        /// <summary>
        /// Calculate stress transformation matrix.
        /// <para>This matrix transforms from x'-y' to x-y coordinates.</para>
        /// </summary>
        /// <param name="theta">Angle of rotation, in radians (positive if counterclockwise).</param>
		public static Matrix<double> TransformationMatrix(double theta)
		{
			var (cos, sin) = theta.DirectionCosines();
			double
				cos2   = cos * cos,
				sin2   = sin * sin,
				cosSin = cos * sin;

			return
				new[,]
				{
					{        cos2,       sin2,      cosSin },
					{        sin2,       cos2,     -cosSin },
					{ -2 * cosSin, 2 * cosSin, cos2 - sin2 }
				}.ToMatrix();
		}
    }
}
