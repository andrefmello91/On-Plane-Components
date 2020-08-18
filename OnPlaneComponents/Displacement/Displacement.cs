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
        private Length _displacementX, _displacementY;
		
        /// <summary>
        /// Get/set the displacement unit.
        /// </summary>
        public LengthUnit Unit
        {
	        get => _displacementX.Unit;
	        set
	        {
		        if (value != Unit)
		        {
			        _displacementX.ToUnit(value);
			        _displacementY.ToUnit(value);
		        }
	        }
        }

        /// <summary>
        /// Get the displacement component in X direction, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double ComponentX => _displacementX.Value;

        /// <summary>
        /// Get the displacement component in Y direction, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double ComponentY => _displacementY.Value;

        /// <summary>
        /// Get the resultant displacement value, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double Resultant => DisplacementRelations.CalculateResultant(ComponentX, ComponentY);

        /// <summary>
        /// Get the resultant displacement, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public Length ResultantDisplacement => DisplacementRelations.CalculateResultant(_displacementX, _displacementY, Unit);

        /// <summary>
        /// Get the resultant displacement angle, in radians.
        /// </summary>
        public double ResultantAngle => DisplacementRelations.CalculateResultantAngle(ComponentX, ComponentY);

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
            _displacementX = Length.From(componentX, unit);
            _displacementY = Length.From(componentY, unit);
        }

        /// <summary>
        /// Displacement object.
        /// </summary>
        /// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length"/>).</param>
        /// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length"/>).</param>
        /// <param name="unit">The unit of displacement (default: mm).</param>
        public Displacement(Length displacementX, Length displacementY, LengthUnit unit = LengthUnit.Millimeter)
        {
            _displacementX = displacementX.ToUnit(unit);
            _displacementY = displacementY.ToUnit(unit);
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
        /// Add values to current displacements.
        /// </summary>
        /// <param name="incrementX">The increment for X component, in current unit (<see cref="Unit"/>).</param>
        /// <param name="incrementY">The increment for Y component, in current unit (<see cref="Unit"/>).</param>
        public void Add(double incrementX, double incrementY) =>
            Add(Length.From(incrementX, Unit), Length.From(incrementY, Unit));

        /// <summary>
        /// Add values to current displacements.
        /// </summary>
        /// <param name="incrementX">The displacement increment for X component.</param>
        /// <param name="incrementY">The displacement increment for Y component.</param>
        public void Add(Length incrementX, Length incrementY)
        {
            _displacementX += incrementX;
            _displacementY += incrementY;
        }

        /// <summary>
        /// Subtract values from current displacements.
        /// </summary>
        /// <param name="decrementX">The decrement for X component (positive value), in current unit (<see cref="Unit"/>).</param>
        /// <param name="decrementY">The decrement for Y component (positive value), in current unit (<see cref="Unit"/>).</param>
        public void Subtract(double decrementX, double decrementY) =>
            Subtract(Length.From(decrementX, Unit), Length.From(decrementY, Unit));

        /// <summary>
        /// Subtract values from current displacements.
        /// </summary>
        /// <param name="decrementX">The displacement decrement for X component (positive value).</param>
        /// <param name="decrementY">The displacement decrement for Y component (positive value).</param>
        public void Subtract(Length decrementX, Length decrementY)
        {
            _displacementX -= decrementX;
            _displacementY -= decrementY;
        }

        /// <summary>
        /// Multiply current displacements by a value.
        /// </summary>
        /// <param name="multiplier">The multiplier for X and Y components.</param>
        public void Multiply(double multiplier) => Multiply(multiplier, multiplier);

        /// <summary>
        /// Multiply current displacements by values.
        /// </summary>
        /// <param name="multiplierX">The multiplier for X component.</param>
        /// <param name="multiplierY">The multiplier for Y component.</param>
        public void Multiply(double multiplierX, double multiplierY)
        {
            _displacementX *= multiplierX;
            _displacementY *= multiplierY;
        }

        /// <summary>
        /// Divide current displacements by a value.
        /// </summary>
        /// <param name="divider">The divider for X and Y components.</param>
        public void Divide(double divider) => Divide(divider, divider);

        /// <summary>
        /// Divide current displacements by values.
        /// </summary>
        /// <param name="dividerX">The divider for X component.</param>
        /// <param name="dividerY">The divider for Y component.</param>
        public void Divide(double dividerX, double dividerY)
        {
            _displacementX /= dividerX;
            _displacementY /= dividerY;
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
            var (x, y) = DisplacementRelations.CalculateComponents(resultant, angle);

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
            var (x, y) = DisplacementRelations.CalculateComponents(resultantDisplacement, angle);

            return new Displacement(x, y, resultantDisplacement.Unit);
        }


        /// <summary>
        /// Compare two displacement objects.
        /// </summary>
        /// <param name="other">The displacement to compare.</param>
        /// <returns></returns>
        public bool Equals(Displacement other) => _displacementX == other._displacementX && _displacementY == other._displacementY;

        public override bool Equals(object obj)
        {
            if (obj is Displacement other)
                return Equals(other);

            return false;
        }

        public override string ToString()
        {
            return
                "ux = " + _displacementX + "\n" +
                "uy = " + _displacementY;
        }

        public override int GetHashCode() => (int)(ComponentX * ComponentY);


        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (Displacement left, Displacement right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (Displacement left, Displacement right) => !left.Equals(right);

        /// <summary>
        /// Returns a displacement object with summed components, in left argument's unit.
        /// </summary>
        public static Displacement operator + (Displacement left, Displacement right) => new Displacement(left._displacementX + right._displacementX, left._displacementY + right._displacementY, left.Unit);

        /// <summary>
        /// Returns a displacement object with subtracted components, in left argument's unit.
        /// </summary>
        public static Displacement operator - (Displacement left, Displacement right) => new Displacement(left._displacementX - right._displacementX, left._displacementY - right._displacementY, left.Unit);

        /// <summary>
        /// Returns a displacement object with multiplied components by a double.
        /// </summary>
        public static Displacement operator *(Displacement displacement, double multiplier) => new Displacement(multiplier * displacement._displacementX, multiplier * displacement._displacementY, displacement.Unit);

        /// <summary>
        /// Returns a displacement object with multiplied components by a double.
        /// </summary>
        public static Displacement operator *(double multiplier, Displacement displacement) => displacement * multiplier;

        /// <summary>
        /// Returns a displacement object with multiplied components by an integer.
        /// </summary>
        public static Displacement operator *(Displacement displacement, int multiplier) => displacement * (double)multiplier;

        /// <summary>
        /// Returns a displacement object with multiplied components by an integer.
        /// </summary>
        public static Displacement operator *(int multiplier, Displacement displacement) => displacement * (double)multiplier;

        /// <summary>
        /// Returns a displacement object with components divided by a double.
        /// </summary>
        public static Displacement operator /(Displacement displacement, double divider) => new Displacement(displacement._displacementX / divider, displacement._displacementY / divider, displacement.Unit);

        /// <summary>
        /// Returns a displacement object with components divided by an integer.
        /// </summary>
        public static Displacement operator /(Displacement displacement, int divider) => displacement / (double)divider;

    }
}
