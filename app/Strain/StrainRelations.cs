using System;
using Extensions;
using Extensions.LinearAlgebra;
using Extensions.Number;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


namespace OnPlaneComponents
{
    public static class StrainRelations
    {
        /// <summary>
        /// Get a <see cref="Vector"/> of strains transformed by a rotation angle.
        /// </summary>
        /// <param name="strainVector"><see cref="Vector"/> of strains.</param>
        /// <param name="theta">The rotation angle, in radians (positive to counterclockwise).</param>
        public static Vector<double> Transform(Vector<double> strainVector, double theta)
	    {
		    // Get the strains
		    var eps = strainVector;

		    var (cos2Theta, sin2Theta) = (2 * theta).DirectionCosines();

		    // Calculate equation components
		    double
                a = 0.5 * (eps[0] + eps[1]),
			    b = 0.5 * (eps[0] - eps[1]) * cos2Theta,
			    c = 0.5 * eps[2] * sin2Theta,
			    d = (eps[0] - eps[1]) * sin2Theta,
			    e =  eps[2] * cos2Theta;

		    return
			   new[]
			   {
				   a + b + c, 
				   a - b - c, 
				   e - d
			   }.ToVector();
	    }

        /// <summary>
        /// Calculate principal strains.
        /// <para>epsilon1 is the maximum strain and epsilon2 is the minimum strain.</para>
        /// </summary>
        /// <param name="epsilonX">The normal strain in X direction (positive for tensile).</param>
        /// <param name="epsilonY">The normal strain in Y direction (positive for tensile).</param>
        /// <param name="gammaXY">The shear strain (positive if right face of element displaces upwards).</param>
        public static (double epsilon1, double epsilon2) CalculatePrincipal(double epsilonX, double epsilonY, double gammaXY)
        {
	        if (epsilonX.ApproxZero(1E-9) && epsilonY.ApproxZero(1E-9) && gammaXY.ApproxZero(1E-9))
		        return (0, 0);

	        if (gammaXY.ApproxZero(1E-9))
		        return (Math.Max(epsilonX, epsilonY), Math.Min(epsilonX, epsilonY));

            // Calculate radius and center of Mohr's Circle
            double
                cen = 0.5 * (epsilonX + epsilonY),
		        rad = 0.5 * Math.Sqrt((epsilonY - epsilonX) * (epsilonY - epsilonX) + gammaXY * gammaXY);

	        // Calculate principal strains in concrete
	        return
		        (cen + rad, cen - rad);
        }

        /// <summary>
        /// Calculate principal strains.
        /// <para>epsilon1 is the maximum strain and epsilon2 is the minimum strain.</para>
        /// </summary>
        /// <param name="strainVector">The <see cref="Vector"/> of strains.
        /// <para>{ EpsilonX, EpsilonY, GammaXY }</para></param>
        public static (double epsilon1, double epsilon2) CalculatePrincipal(Vector<double> strainVector) =>
	        CalculatePrincipal(strainVector[0], strainVector[1], strainVector[2]);

        /// <summary>
        /// Calculate principal strains angles, in radians.
        /// <para>theta1 is the maximum strain angle and theta2 is the minimum strain angle.</para>
        /// </summary>
        /// <param name="epsilonX">The normal strain in X direction (positive for tensile).</param>
        /// <param name="epsilonY">The normal strain in Y direction (positive for tensile).</param>
        /// <param name="gammaXY">The shear strain (positive if right face of element displaces upwards).</param>
        /// <param name="epsilon2">Minimum principal strain, if known.</param>
        public static (double theta1, double theta2) CalculatePrincipalAngles(double epsilonX, double epsilonY, double gammaXY, double? epsilon2 = null)
        {
            double theta1 = Constants.PiOver4;

            if (!epsilonX.ApproxZero(1E-9) || !epsilonY.ApproxZero(1E-9) || !gammaXY.ApproxZero(1E-9))
            {
                // Calculate the strain slope
                if (gammaXY.ApproxZero(1E-9))
                {
                    if (epsilonX >= epsilonY)
                        theta1 = 0;
                    else
                        theta1 = Constants.PiOver2;
                }

                else if (epsilonX.Approx(epsilonY, 1E-9) && gammaXY < 0)
	                theta1 = -Constants.PiOver4;

                else if (epsilon2.HasValue)
                {
                    var e2 = epsilon2.Value;
                    theta1 = Constants.PiOver2 - (2 * (epsilonX - e2) / gammaXY).Atan();
                }

                else
	                theta1 = Constants.PiOver2 - 0.5 * (gammaXY / (epsilonY - epsilonX)).Atan();

                if (theta1.IsNaN())
                    theta1 = Constants.PiOver4;
            }

            // Calculate theta2
            double theta2 = Constants.PiOver2 + theta1;

            return
                (theta1, theta2);
        }

        /// <summary>
        /// Calculate principal strains angles, in radians.
        /// <para>theta1 is the maximum strain angle and theta2 is the minimum strain angle.</para>
        /// </summary>
        /// <param name="strainVector">The <see cref="Vector"/> of strains.
        /// <para>{ EpsilonX, EpsilonY, GammaXY }</para></param>
        /// <param name="epsilon2">Minimum principal strain, if known.</param>
        public static (double theta1, double theta2) CalculatePrincipalAngles(Vector<double> strainVector, double? epsilon2 = null) =>
	        CalculatePrincipalAngles(strainVector[0], strainVector[1], strainVector[2], epsilon2);

        /// <summary>
        /// Calculate the vector of strains, in X and Y, from principal strains.
        /// </summary>
        /// <param name="epsilon1">Maximum principal strain.</param>
        /// <param name="epsilon2">Minimum principal strain.</param>
        /// <param name="theta1">Angle of the <paramref name="epsilon1"/>, in radians.</param>
        /// <returns></returns>
        public static Vector<double> StrainsFromPrincipal(double epsilon1, double epsilon2, double theta1)
        {
	        // Calculate theta2
	        var theta2 = Constants.PiOver2 - theta1;

	        // Calculate theta2 (fc2 angle)
	        var (cos, sin) = (2 * theta2).DirectionCosines();

	        // Calculate straines by Mohr's Circle
	        double
		        cen  = 0.5 * (epsilon1 + epsilon2),
		        rad  = 0.5 * (epsilon1 - epsilon2),
		        ex   = cen - rad * cos,
		        ey   = cen + rad * cos,
		        exy  = 2 * rad * sin;

	        return
		        new[] { ex, ey, exy }.ToVector();
        }

        /// <summary>
        /// Calculate strain transformation matrix.
        /// <para>This matrix transforms from x'-y' to x-y coordinates.</para>
        /// </summary>
        /// <param name="theta">Angle of rotation, in radians (positive if counterclockwise).</param>
        public static Matrix<double> TransformationMatrix(double theta) => StressRelations.TransformationMatrix(theta);
    }
}
