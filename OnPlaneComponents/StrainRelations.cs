using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Relations
{
    public static class Strain
    {
        /// <summary>
        /// Strain transformation.
        /// </summary>
        /// <param name="strains">Vector of strains.</param>
        /// <param name="theta">Rotation angle, in radians.</param>
        /// <returns></returns>
        public static Vector<double> Transform(Vector<double> strains, double theta)
	    {
		    // Get the stresses
		    var eps = strains;

		    var (cos2Theta, sin2Theta) = DirectionCosines(2 * theta);

		    // Calculate radius and center of Mohr's Circle
		    double
			    a = 0.5 * (eps[0] + eps[1]),
			    b = 0.5 * (eps[0] - eps[1]) * cos2Theta,
			    c = 0.5 * eps[2] * sin2Theta,
			    d = (eps[0] - eps[1]) * sin2Theta,
			    e =  eps[2] * cos2Theta;

		    return
			    Vector<double>.Build.DenseOfArray(new[]
			    {
				    a + b + c,
				    a - b - c,
				    e - d
			    });
	    }

        /// <summary>
        /// Calculate principal strain angles, in radians.
        /// <para>theta1 is the maximum strain angle and theta2 is the minimum strain angle.</para>
        /// </summary>
        /// <param name="strains">Vector of strains.</param>
        /// <param name="principalStrains">Calculated principal strains.</param>
        public static (double theta1, double theta2) PrincipalAngles(Vector<double> strains, (double e1, double e2) principalStrains)
        {
	        double theta1 = Constants.PiOver4;

	        // Get the strains
	        var e   = strains;
	        var ec2 = principalStrains.e2;

	        // Verify the strains
	        if (e.Exists(NotZero))
	        {
		        // Calculate the strain slope
		        if (e[2] == 0)
			        theta1 = 0;

		        else if (Math.Abs(e[0] - e[1]) <= 1E-9 && e[2] < 0)
			        theta1 = -Constants.PiOver4;

		        else
			        //theta1 = 0.5 * Trig.Atan(e[2] / (e[0] - e[1]));
			        theta1 = Constants.PiOver2 - Trig.Atan(2 * (e[0] - ec2) / e[2]);
	        }

	        // Calculate theta2
	        double theta2 = Constants.PiOver2 - theta1;

	        //if (theta2 > Constants.PiOver2)
	        //	theta2 -= Constants.Pi;

	        return
		        (theta1, theta2);
        }

        /// <summary>
        /// Calculate principal strain angles, in radians.
        /// <para>theta1 is the maximum strain angle and theta2 is the minimum strain angle.</para>
        /// </summary>
        /// <param name="strains">Vector of strains.</param>
        public static (double theta1, double theta2) PrincipalAngles(Vector<double> strains)
        {
	        double theta1 = Constants.PiOver4;

	        // Get the strains
	        var e = strains;

	        // Verify the strains
	        if (e.Exists(NotZero))
	        {
		        // Calculate the strain slope
		        if (e[2] == 0)
			        theta1 = 0;

		        else if (e[0] - e[1] <= 1E-9 && e[2] < 0)
			        theta1 = -Constants.PiOver4;

		        else
			        theta1 = 0.5 * Trig.Atan(e[2] / (e[0] - e[1]));

		        if (double.IsNaN(theta1))
			        theta1 = Constants.PiOver4;
	        }

	        // Calculate theta2
	        double theta2 = Constants.PiOver2 - theta1;

	        //if (theta2 > Constants.PiOver2)
	        //	theta2 -= Constants.Pi;

	        return
		        (theta1, theta2);
        }

        /// <summary>
        /// Calculate principal strains.
        /// <para>e1 is the maximum strain angle and e2 is the minimum strain.</para>
        /// </summary>
        /// <param name="strains">Vector of strains.</param>
        public static (double e1, double e2) PrincipalStrains(Vector<double> strains)
        {
            // Get the strains
            var e = strains;

	        // Calculate radius and center of Mohr's Circle
	        double
		        cen = 0.5 * (e[0] + e[1]),
		        rad = 0.5 * Math.Sqrt((e[1] - e[0]) * (e[1] - e[0]) + e[2] * e[2]);

	        // Calculate principal strains in concrete
	        double
		        e1 = cen + rad,
		        e2 = cen - rad;

	        return
		        (e1, e2);
        }

        /// <summary>
        /// Calculate the vector of strains, in X and Y, from principal strains.
        /// </summary>
        /// <param name="principalStrains">Principal strains.</param>
        /// <param name="theta2">Angle of the minimum principal strain, in radians.</param>
        /// <returns></returns>
        public static Vector<double> StrainsFromPrincipal((double e1, double e2) principalStrains, double theta2)
        {
	        // Get principal stresses
	        var (e1, e2) = principalStrains;

	        // Calculate theta2 (fc2 angle)
	        var (cos, sin) = DirectionCosines(2 * theta2);

	        // Calculate stresses by Mohr's Circle
	        double
		        cen  = 0.5 * (e1 + e2),
		        rad  = 0.5 * (e1 - e2),
		        ex   = cen - rad * cos,
		        ey   = cen + rad * cos,
		        exy  = 2 * rad * sin;

	        return
		        CreateVector.DenseOfArray(new[] { ex, ey, exy });
        }

        /// <summary>
        /// Calculate stresses/strains transformation matrix.
        /// <para>This matrix transforms from x-y to 1-2 coordinates.</para>
        /// </summary>
        /// <param name="theta1">Angle of the maximum principal strain, in radians.</param>
        /// <returns></returns>
        public static Matrix<double> TransformationMatrix(double theta1) => Stress.TransformationMatrix(theta1);

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
