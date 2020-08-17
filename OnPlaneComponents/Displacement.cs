using System;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// Displacement struct.
    /// </summary>
    public struct Displacement : IEquatable<Displacement>
    {
        // Auxiliar fields
        public Length DisplacementX, DisplacementY;
		
        /// <summary>
        /// Get/set the displacement unit.
        /// </summary>
        public LengthUnit Unit
        {
	        get => DisplacementX.Unit;
	        set
	        {
		        DisplacementX.ToUnit(value);
		        DisplacementY.ToUnit(value);
	        }
        }

        /// <summary>
        /// Get the displacement component in X direction, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double ComponentX => DisplacementX.Value;

        /// <summary>
        /// Get the displacement component in Y direction, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double ComponentY => DisplacementY.Value;

        /// <summary>
        /// Get the resultant displacement value, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double Resultant => CalculateResultant(ComponentX, ComponentY);

        /// <summary>
        /// Get the resultant displacement, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public Length ResultantDisplacement => CalculateResultant(DisplacementX, DisplacementY, Unit);

        /// <summary>
        /// Get the resultant displacement angle, in radians.
        /// </summary>
        public double ResultantAngle => CalculateResultantAngle(ComponentX, ComponentY);

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
            DisplacementX = Length.From(componentX, unit);
            DisplacementY = Length.From(componentY, unit);
        }

        /// <summary>
        /// Displacement object.
        /// </summary>
        /// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length"/>).</param>
        /// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length"/>).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public Displacement(Length displacementX, Length displacementY, LengthUnit unit = LengthUnit.Millimeter)
        {
            DisplacementX = displacementX.ToUnit(unit);
            DisplacementY = displacementY.ToUnit(unit);
        }

        /// <summary>
        /// Change the displacement unit.
        /// </summary>
        /// <param name="toUnit">The unit to convert.</param>
        public void ChangeUnit(LengthUnit toUnit)
        {
	        Unit = toUnit;
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
        /// <param name="displacement">Displacement component in X direction (positive to right).</param>
        public static Displacement InX(Length displacement) => new Displacement(displacement, Length.Zero, displacement.Unit);

        /// <summary>
        /// Get a Displacement in X direction.
        /// </summary>
        /// <param name="value">Value of displacement component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public static Displacement InY(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(0, value, unit);

        /// <summary>
        /// Get a Displacement in Y direction.
        /// </summary>
        /// <param name="displacement">Displacement component in Y direction (positive to right).</param>
        public static Displacement InY(Length displacement) => new Displacement(Length.Zero, displacement, displacement.Unit);

        /// <summary>
        /// Get a Displacement in from a resultant.
        /// </summary>
        /// <param name="resultant">Absolute value of displacement resultant.</param>
        /// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public static Displacement FromResultant(double resultant, double angle, LengthUnit unit = LengthUnit.Millimeter)
        {
            var (x, y) = CalculateComponents(resultant, angle);

            return
	            new Displacement(x, y, unit);
        }

        /// <summary>
        /// Get a Displacement in from a resultant.
        /// </summary>
        /// <param name="resultantDisplacement">Absolute value of displacement resultant.</param>
        /// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
        public static Displacement FromResultant(Length resultantDisplacement, double angle)
        {
            var (x, y) = CalculateComponents(resultantDisplacement, angle);

            return new Displacement(x, y, resultantDisplacement.Unit);
        }

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

        /// <summary>
        /// Compare two displacement objects.
        /// </summary>
        /// <param name="other">The displacement to compare.</param>
        /// <returns></returns>
        public bool Equals(Displacement other)
        {
            return
	            DisplacementX == other.DisplacementX && DisplacementY == other.DisplacementY;
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
                "ux = " + DisplacementX + "\n" +
                "uy = " + DisplacementY;
        }

        public override int GetHashCode()
        {
            return (int)(ComponentX * ComponentY);
        }


        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (Displacement left, Displacement right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (Displacement left, Displacement right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a displacement object with summed components, in left argument's unit.
        /// </summary>
        public static Displacement operator + (Displacement left, Displacement right)
        {
	        return
		        new Displacement(left.DisplacementX + right.DisplacementX, left.DisplacementY + right.DisplacementY, left.Unit);
        }

        /// <summary>
        /// Returns a displacement object with subtracted components, in left argument's unit.
        /// </summary>
        public static Displacement operator - (Displacement left, Displacement right)
        {
	        return
		        new Displacement(left.DisplacementX - right.DisplacementX, left.DisplacementY - right.DisplacementY, left.Unit);
        }
    }
}
