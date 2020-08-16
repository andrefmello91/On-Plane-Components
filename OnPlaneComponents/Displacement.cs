using System;
using MathNet.Numerics;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// Displacement struct.
    /// </summary>
    public struct Displacement : IEquatable<Displacement>
    {
        // Auxiliar fields
        private readonly UnitsNet.Length
            _componentX,
            _componentY,
            _resultant;

        /// <summary>
        /// Get the displacement component in X direction, in the unit constructed.
        /// </summary>
        public double ComponentX => _componentX.Value;

        /// <summary>
        /// Get the displacement component in Y direction, in the unit constructed.
        /// </summary>
        public double ComponentY => _componentY.Value;

        /// <summary>
        /// Get the resultant displacement, in the unit constructed.
        /// </summary>
        public double Resultant => _resultant.Value;

        /// <summary>
        /// Get the resultant displacement angle, in radians.
        /// </summary>
        public double ResultantAngle { get; }

        /// <summary>
        /// Verify if X component is zero.
        /// </summary>
        public bool IsComponentXZero => ComponentX == 0;

        /// <summary>
        /// Verify if Y component is zero.
        /// </summary>
        public bool IsComponentYZero => ComponentY == 0;

        /// <summary>
        /// Verify if displacement components are zero.
        /// </summary>
        public bool AreComponentsZero => IsComponentXZero && IsComponentYZero;

        /// <summary>
        /// Verify if displacement resultant is zero.
        /// </summary>
        public bool IsResultantZero => Resultant == 0;

        /// <summary>
        /// Displacement object.
        /// </summary>
        /// <param name="componentX">Value of displacement component in X direction (positive to right).</param>
        /// <param name="componentY">Value of displacement component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public Displacement(double componentX, double componentY, LengthUnit unit = LengthUnit.Millimeter)
        {
            _componentX    = UnitsNet.Length.From(componentX, unit);
            _componentY    = UnitsNet.Length.From(componentY, unit);
            _resultant     = UnitsNet.Length.From(CalculateResultant(componentX, componentY), unit);
            ResultantAngle = CalculateResultantAngle(componentX, componentY);
        }

        /// <summary>
        /// Get a Displacement with zero value.
        /// </summary>
        public static Displacement Zero => new Displacement(0, 0);

        /// <summary>
        /// Get a Displacement in X direction.
        /// </summary>
        /// <param name="value">Value of displacement component in X direction (positive to right).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public static Displacement InX(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(value, 0, unit);

        /// <summary>
        /// Get a Displacement in X direction.
        /// </summary>
        /// <param name="value">Value of displacement component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public static Displacement InY(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(0, value, unit);

        /// <summary>
        /// Get a Displacement in from a resultant.
        /// </summary>
        /// <param name="resultant">Value of displacement resultant.</param>
        /// <param name="angle">Angle of displacement resultant, in radians (0 to Pi/2).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public static Displacement FromResultant(double resultant, double angle, LengthUnit unit = LengthUnit.Millimeter)
        {
            var (x, y) = CalculateComponents(resultant, angle);

            return new Displacement(x, y, unit);
        }

        /// <summary>
        /// Calculate the resultant displacement.
        /// </summary>
        /// <param name="componentX">Value of displacement component in X direction.</param>
        /// <param name="componentY">Value of displacement component in Y direction.</param>
        public static double CalculateResultant(double componentX, double componentY)
        {
            if (componentX == 0 && componentY == 0)
                return 0;

            return
                Math.Sqrt(componentX * componentX + componentY * componentY);
        }

        /// <summary>
        /// Calculate the angle of the resultant displacement.
        /// </summary>
        /// <param name="componentX">Value of displacement component in X direction.</param>
        /// <param name="componentY">Value of displacement component in Y direction.</param>
        public static double CalculateResultantAngle(double componentX, double componentY)
        {
            return
                Math.Atan(componentY / componentX);
        }

        /// <summary>
        /// Calculate components of a resultant displacement.
        /// </summary>
        /// <param name="resultant">Value of displacement resultant.</param>
        /// <param name="angle">Angle of displacement resultant, in radians (0 to Pi/2).</param>
        public static (double X, double Y) CalculateComponents(double resultant, double angle)
        {
            if (angle == 0)
                return (resultant, 0);

            if (angle == Constants.PiOver2)
                return (0, resultant);

            return
                (resultant * Math.Acos(angle), resultant * Math.Asin(angle));
        }

        /// <summary>
        /// Compare two displacement objects.
        /// </summary>
        /// <param name="other">The displacement to compare.</param>
        /// <returns></returns>
        public bool Equals(Displacement other)
        {
            return ComponentX == other.ComponentX && ComponentY == other.ComponentY;
        }

        public override bool Equals(object obj)
        {
            if (obj is Displacement other)
                return Equals(other);

            return false;
        }

        public override string ToString()
        {
            return
                "ux = " + _componentX + "\n" +
                "uy = " + _componentY;
        }

        public override int GetHashCode()
        {
            return (int)(ComponentX * ComponentY);
        }

        public static bool operator == (Displacement lhs, Displacement rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator != (Displacement lhs, Displacement rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
