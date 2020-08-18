using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
	/// <summary>
    /// Static class for displacement relations.
    /// </summary>
    public static class DisplacementRelations
    {
        /// <summary>
        /// Calculate the resultant displacement.
        /// </summary>
        /// <param name="componentX">Value of displacement component in X direction (positive to right).</param>
        /// <param name="componentY">Value of displacement component in Y direction (positive upwards).</param>
        public static double CalculateResultant(double componentX, double componentY)
        {
            if (componentX == 0 && componentY == 0)
                return 0;

            return
                Math.Sqrt(componentX * componentX + componentY * componentY);
        }

        /// <summary>
        /// Calculate the resultant displacement.
        /// </summary>
        /// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length"/>).</param>
        /// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length"/>).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public static Length CalculateResultant(Length displacementX, Length displacementY, LengthUnit unit = LengthUnit.Millimeter)
        {
            if (displacementX == Length.Zero && displacementY == Length.Zero)
                return Length.Zero;

            return
                Length.From(CalculateResultant(displacementX.Value, displacementY.ToUnit(displacementX.Unit).Value), unit);
        }

        /// <summary>
        /// Calculate the angle of the resultant displacement.
        /// </summary>
        /// <param name="componentX">Value of displacement component in X direction (positive to right).</param>
        /// <param name="componentY">Value of displacement component in Y direction (positive upwards).</param>
        public static double CalculateResultantAngle(double componentX, double componentY)
        {
            if (componentX > 0 && componentY == 0)
                return 0;

            if (componentX < 0 && componentY == 0)
                return Constants.Pi;

            if (componentX == 0 && componentY > 0)
                return Constants.PiOver2;

            if (componentX == 0 && componentY < 0)
                return Constants.Pi3Over2;

            return
                Math.Atan(componentY / componentX);
        }

        /// <summary>
        /// Calculate the angle of the resultant displacement.
        /// </summary>
        /// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length"/>).</param>
        /// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length"/>).</param>
        public static double CalculateResultantAngle(Length displacementX, Length displacementY)
        {
            return
                CalculateResultantAngle(displacementX.Value, displacementY.ToUnit(displacementX.Unit).Value);
        }

        /// <summary>
        /// Calculate components of a resultant displacement.
        /// </summary>
        /// <param name="resultant">Absolute value of displacement resultant.</param>
        /// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
        public static (double X, double Y) CalculateComponents(double resultant, double angle)
        {
            if (angle == 0)
                return (resultant, 0);

            if (angle == Constants.PiOver2)
                return (0, resultant);

            if (angle == Constants.Pi)
                return (-resultant, 0);

            if (angle == Constants.Pi3Over2)
                return (0, -resultant);

            return
                (resultant * Math.Acos(angle), resultant * Math.Asin(angle));
        }

        /// <summary>
        /// Calculate Displacement components of a resultant displacement.
        /// </summary>
        /// <param name="resultantDisplacement">Absolute displacement resultant.</param>
        /// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
        public static (Length X, Length Y) CalculateComponents(Length resultantDisplacement, double angle)
        {
            var (x, y) = CalculateComponents(resultantDisplacement.Value, angle);

            return
                (Length.From(x, resultantDisplacement.Unit), Length.From(y, resultantDisplacement.Unit));
        }

    }
}
