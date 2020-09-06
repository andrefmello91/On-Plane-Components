using System;
using UnitsNet.Units;

namespace OnPlaneComponents
{
	/// <summary>
	/// Force struct.
	/// </summary>
	public partial struct Force : IEquatable<Force>
	{
		// Auxiliary fields
		private UnitsNet.Force _forceX, _forceY;

		/// <summary>
        /// Get/set the force unit (<see cref="ForceUnit"/>).
        /// </summary>
		public ForceUnit Unit => _forceX.Unit;

		/// <summary>
		/// Get the force component value in X direction, in the unit constructed (<see cref="Unit"/>).
		/// </summary>
		public double ComponentX => _forceX.Value;

        /// <summary>
        /// Get the force component value in Y direction, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double ComponentY => _forceY.Value;

        /// <summary>
        /// Get the resultant force value, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public double Resultant => ForceRelations.CalculateResultant(ComponentX, ComponentY);

        /// <summary>
        /// Get the resultant force, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public UnitsNet.Force ResultantForce => ForceRelations.CalculateResultant(_forceX, _forceY, Unit);

		/// <summary>
		/// Get the resultant force angle, in radians.
		/// </summary>
		public double ResultantAngle => ForceRelations.CalculateResultantAngle(ComponentX, ComponentY);

		/// <summary>
		/// Verify if X component is zero.
		/// </summary>
		public bool IsComponentXZero => ComponentX == 0;

		/// <summary>
		/// Verify if Y component is zero.
		/// </summary>
		public bool IsComponentYZero => ComponentY == 0;

		/// <summary>
		/// Verify if force components are zero.
		/// </summary>
		public bool AreComponentsZero => IsComponentXZero && IsComponentYZero;

		/// <summary>
		/// Verify if force resultant is zero.
		/// </summary>
		public bool IsResultantZero => Resultant == 0;

        /// <summary>
        /// Force object.
        /// </summary>
        /// <param name="componentX">Value of force component in X direction (positive to right).</param>
        /// <param name="componentY">Value of force component in Y direction (positive upwards).</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public Force(double componentX, double componentY, ForceUnit unit = ForceUnit.Newton)
		{
			_forceX = UnitsNet.Force.From(DoubleToZero(componentX), unit);
			_forceY = UnitsNet.Force.From(DoubleToZero(componentY), unit);
		}

        /// <summary>
        /// Force object.
        /// </summary>
        /// <param name="forceX"><see cref="UnitsNet.Force"/> component in X direction (positive to right).</param>
        /// <param name="forceY"><see cref="UnitsNet.Force"/> component in Y direction (positive upwards).</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public Force(UnitsNet.Force forceX, UnitsNet.Force forceY, ForceUnit unit = ForceUnit.Newton)
		{
			_forceX = forceX.ToUnit(unit);
			_forceY = forceY.ToUnit(unit);
		}

        /// <summary>
        /// Change the force unit.
        /// </summary>
        /// <param name="toUnit">The <see cref="ForceUnit"/> to convert.</param>
        public void ChangeUnit(ForceUnit toUnit)
		{
			if (Unit == toUnit)
				return;

            _forceX = _forceX.ToUnit(toUnit);
			_forceY = _forceY.ToUnit(toUnit);
		}

        /// <summary>
        /// Get a <see cref="Force"/> with zero value.
        /// </summary>
        public static Force Zero => new Force(0, 0);

        /// <summary>
        /// Get a <see cref="Force"/> in X direction.
        /// </summary>
        /// <param name="value">Value of force component in X direction (positive to right).</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public static Force InX(double value, ForceUnit unit = ForceUnit.Newton) => new Force(value, 0, unit);

        /// <summary>
        /// Get a <see cref="Force"/> in X direction.
        /// </summary>
        /// <param name="force"><see cref="UnitsNet.Force"/> component in X direction (positive to right).</param>
        public static Force InX(UnitsNet.Force force) => new Force(force, UnitsNet.Force.Zero, force.Unit);

        /// <summary>
        /// Get a <see cref="Force"/> in X direction.
        /// </summary>
        /// <param name="value">Value of force component in Y direction (positive upwards).</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public static Force InY(double value, ForceUnit unit = ForceUnit.Newton) => new Force(0, value, unit);

        /// <summary>
        /// Get a <see cref="Force"/> in Y direction.
        /// </summary>
        /// <param name="force"><see cref="UnitsNet.Force"/> component in Y direction (positive upwards).</param>
        public static Force InY(UnitsNet.Force force) => new Force(UnitsNet.Force.Zero, force, force.Unit);

        /// <summary>
        /// Get a <see cref="Force"/> from a resultant.
        /// </summary>
        /// <param name="resultant">Absolute value of force resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public static Force FromResultant(double resultant, double angle, ForceUnit unit = ForceUnit.Newton)
        {
	        var (x, y) = ForceRelations.CalculateComponents(resultant, angle);

			return new Force(x, y, unit);
        }

        /// <summary>
        /// Get a <see cref="Force"/> from a resultant.
        /// </summary>
        /// <param name="resultantForce">Absolute <see cref="UnitsNet.Force"/> resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        public static Force FromResultant(UnitsNet.Force resultantForce, double angle)
        {
	        var (x, y) = ForceRelations.CalculateComponents(resultantForce, angle);

			return
				new Force(x, y, resultantForce.Unit);
        }

        /// <summary>
        /// Return zero if <paramref name="number"/> is <see cref="double.NaN"/> or <see cref="double.PositiveInfinity"/> or <see cref="double.NegativeInfinity"/>.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static double DoubleToZero(double number) => !double.IsNaN(number) && !double.IsInfinity(number) ? number : 0;

        /// <summary>
        /// Compare two <see cref="Force"/> objects.
        /// </summary>
        /// <param name="other">The <see cref="Force"/> to compare.</param>
        public bool Equals(Force other) => _forceX == other._forceX && _forceY == other._forceY;

		public override bool Equals(object obj)
		{
			if (obj is Force other)
				return Equals(other);

			return false;
		}

		public override string ToString()
		{
			return
				"Fx = " + _forceX + "\n" + 
				"Fy = " + _forceY;
		}

        public override int GetHashCode() => (int) (ComponentX * ComponentY);
	}
}