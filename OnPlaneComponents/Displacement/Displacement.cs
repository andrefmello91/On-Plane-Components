using System;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// Displacement struct.
    /// </summary>
    public partial struct Displacement : IEquatable<Displacement>
    {
        // Auxiliar fields
        private Length _displacementX, _displacementY;
		
        /// <summary>
        /// Get/set the displacement unit (<see cref="LengthUnit"/>).
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
        /// <param name="unit">The <see cref="LengthUnit"/> of displacement (default: <see cref="LengthUnit.Millimeter"/>).</param>
        public Displacement(double componentX, double componentY, LengthUnit unit = LengthUnit.Millimeter)
        {
            _displacementX = Length.From(!double.IsNaN(componentX) ? componentX : 0, unit);
            _displacementY = Length.From(!double.IsNaN(componentY) ? componentY : 0, unit);
        }

        /// <summary>
        /// Displacement object.
        /// </summary>
        /// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length"/>).</param>
        /// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length"/>).</param>
        /// <param name="unit">The <see cref="LengthUnit"/> of displacement (default: <see cref="LengthUnit.Millimeter"/>).</param>
        public Displacement(Length displacementX, Length displacementY, LengthUnit unit = LengthUnit.Millimeter)
        {
            _displacementX = displacementX.ToUnit(unit);
            _displacementY = displacementY.ToUnit(unit);
        }

        /// <summary>
        /// Change the displacement unit (<see cref="LengthUnit"/).
        /// </summary>
        /// <param name="toUnit">The unit to convert.</param>
        public void ChangeUnit(LengthUnit toUnit)
        {
	        Unit = toUnit;
        }

        /// <summary>
        /// Get a <see cref="Displacement"/> with zero value.
        /// </summary>
        public static Displacement Zero => new Displacement(0, 0);

        /// <summary>
        /// Get a <see cref="Displacement"/> in X direction.
        /// </summary>
        /// <param name="value">Value of displacement component in X direction (positive to right).</param>
        /// <param name="unit">The <see cref="LengthUnit"/> of displacement (default: <see cref="LengthUnit.Millimeter"/>).</param>
        public static Displacement InX(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(value, 0, unit);

        /// <summary>
        /// Get a <see cref="Displacement"/> in X direction.
        /// </summary>
        /// <param name="displacement">Displacement component in X direction (positive to right).</param>
        public static Displacement InX(Length displacement) => new Displacement(displacement, Length.Zero, displacement.Unit);

        /// <summary>
        /// Get a <see cref="Displacement"/> in X direction.
        /// </summary>
        /// <param name="value">Value of displacement component in Y direction (positive upwards).</param>
        /// <param name="unit">The <see cref="LengthUnit"/> of displacement (default: <see cref="LengthUnit.Millimeter"/>).</param>
        public static Displacement InY(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(0, value, unit);

        /// <summary>
        /// Get a <see cref="Displacement"/> in Y direction.
        /// </summary>
        /// <param name="displacement">Displacement component in Y direction (positive to right).</param>
        public static Displacement InY(Length displacement) => new Displacement(Length.Zero, displacement, displacement.Unit);

        /// <summary>
        /// Get a <see cref="Displacement"/> in from a resultant.
        /// </summary>
        /// <param name="resultant">Absolute value of displacement resultant.</param>
        /// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
        /// <param name="unit">The <see cref="LengthUnit"/> of displacement (default: <see cref="LengthUnit.Millimeter"/>).</param>
        public static Displacement FromResultant(double resultant, double angle, LengthUnit unit = LengthUnit.Millimeter)
        {
            var (x, y) = DisplacementRelations.CalculateComponents(resultant, angle);

            return
	            new Displacement(x, y, unit);
        }

        /// <summary>
        /// Get a <see cref="Displacement"/> in from a resultant.
        /// </summary>
        /// <param name="resultantDisplacement">Absolute value of displacement resultant.</param>
        /// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
        public static Displacement FromResultant(Length resultantDisplacement, double angle)
        {
            var (x, y) = DisplacementRelations.CalculateComponents(resultantDisplacement, angle);

            return new Displacement(x, y, resultantDisplacement.Unit);
        }


        /// <summary>
        /// Compare two <see cref="Displacement"/> objects.
        /// </summary>
        /// <param name="other">The <see cref="Displacement"/> to compare.</param>
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
    }
}
