using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Relations
{
    public static class Stress
    {
        /// <summary>
        /// Stress transformation.
        /// </summary>
        /// <param name="stresses">Vector of stresses.</param>
        /// <param name="theta">Rotation angle, in radians.</param>
        /// <returns></returns>
        public static Vector<double> Transform(Vector<double> stresses, double theta)
		{
            // Get the stresses
            var f = stresses;

            var (cos2Theta, sin2Theta) = DirectionCosines(2 * theta);

			// Calculate radius and center of Mohr's Circle
			double
				a = 0.5 * (f[0] + f[1]),
				b = 0.5 * (f[0] - f[1]) * cos2Theta,
				c = f[2] * sin2Theta,
				d = 0.5 * (f[0] - f[1]) * sin2Theta,
				e = f[2] * cos2Theta;

            return
                Vector<double>.Build.DenseOfArray(new []
				{
					a + b + c,
					a - b - c,
					e - d
				});
		}

        /// <summary>
        /// Calculate principal stress angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="stresses">Vector of stresses.</param>
        /// <param name="principalStresses">Calculated principal stresses.</param>
        public static (double theta1, double theta2) PrincipalAngles(Vector<double> stresses, (double f1, double f2) principalStresses)
        {
	        double theta1 = Constants.PiOver4;

	        // Get the strains
	        var f   = stresses;
	        var f2 = principalStresses.f2;

	        // Verify the strains
	        if (f.Exists(NotZero))
	        {
		        // Calculate the strain slope
		        if (f[2] == 0)
			        theta1 = 0;

		        else if (Math.Abs(f[0] - f[1]) <= 1E-9 && f[2] < 0)
			        theta1 = -Constants.PiOver4;

		        else
			        //theta1 = 0.5 * Trig.Atan(e[2] / (e[0] - e[1]));
			        theta1 = Constants.PiOver2 - Trig.Atan((f[0] - f2) / f[2]);
	        }

	        // Calculate theta2
	        double theta2 = Constants.PiOver2 - theta1;

	        //if (theta2 > Constants.PiOver2)
	        //	theta2 -= Constants.Pi;

	        return
		        (theta1, theta2);
        }

        /// <summary>
        /// Calculate principal stress angles, in radians.
        /// <para>theta1 is the maximum stress angle and theta2 is the minimum stress angle.</para>
        /// </summary>
        /// <param name="stresses">Vector of stresses.</param>
        public static (double e1, double e2) PrincipalStresses(Vector<double> stresses)
        {
            // Get the stresses
            var f = stresses;

	        // Calculate radius and center of Mohr's Circle
	        double
		        cen = 0.5 * (f[0] + f[1]),
		        rad = Math.Sqrt(0.25 * (f[1] - f[0]) * (f[1] - f[0]) + f[2] * f[2]);

	        // Calculate principal strains in concrete
	        double
		        f1 = cen + rad,
		        f2 = cen - rad;

	        return
		        (f1, f2);
        }

        /// <summary>
        /// Calculate the vector of stresses, in X and Y, from principal stresses.
        /// </summary>
        /// <param name="principalStresses">Principal stresses.</param>
        /// <param name="theta2">Angle of the minimum principal stress, in radians.</param>
        /// <returns></returns>
		public static Vector<double> StressesFromPrincipal((double f1, double f2) principalStresses, double theta2)
		{
			// Get principal stresses
			var (f1, f2) = principalStresses;

			// Calculate theta2 (fc2 angle)
			var (cos, sin) = DirectionCosines(2 * theta2);

			// Calculate stresses by Mohr's Circle
			double
				cen  = 0.5 * (f1 + f2),
				rad  = 0.5 * (f1 - f2),
				fx   = cen - rad * cos,
				fy   = cen + rad * cos,
				fxy  = rad * sin;

			return
				CreateVector.DenseOfArray(new[] { fx, fy, fxy });
        }

        /// <summary>
        /// Calculate stresses/strains transformation matrix.
        /// <para>This matrix transforms from x-y to 1-2 coordinates.</para>
        /// </summary>
        /// <param name="theta1">Angle of the maximum principal strain, in radians.</param>
        /// <returns></returns>
		public static Matrix<double> TransformationMatrix(double theta1)
		{
			var (cos, sin) = DirectionCosines(theta1);
			double
				cos2   = cos * cos,
				sin2   = sin * sin,
				cosSin = cos * sin;

			return
				Matrix<double>.Build.DenseOfArray(new[,]
				{
					{        cos2,       sin2,      cosSin },
					{        sin2,       cos2,     -cosSin },
					{ -2 * cosSin, 2 * cosSin, cos2 - sin2 }
				});
		}

        /// <summary>
        /// Verify if a number is zero (true if is not zero).
        /// </summary>
        /// <param name="number">The number.</param>
		private static bool NotZero(double number) => number != 0;

        /// <summary>
        /// Calculate the direction cosines (cos, sin) of an angle.
        /// </summary>
        /// <param name="angle">Angle, in radians.</param>
        /// <returns></returns>
		private static (double cos, double sin) DirectionCosines(double angle)
		{
			double
				cos = Trig.Cos(angle).CoerceZero(1E-6),
				sin = Trig.Sin(angle).CoerceZero(1E-6);

			return (cos, sin);
		}
    }
}
