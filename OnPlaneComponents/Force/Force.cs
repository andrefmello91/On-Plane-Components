using System;
using Extensions.Number;
using UnitsNet.Units;
using static OnPlaneComponents.ForceRelations;

namespace OnPlaneComponents
{
	/// <summary>
	/// Force struct.
	/// </summary>
	public partial struct Force : IEquatable<Force>
	{
		// Auxiliary fields
		private UnitsNet.Force _forceX, _forceY;
		private UnitsNet.Force? _resultant;

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
        public double Resultant => _resultant?.Value ?? CalculateResultant().Value;

		/// <summary>
		/// Get the resultant force angle, in radians.
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
        /// <param name="componentX">Value of force component in X direction, in <paramref name="unit"/>. (positive to right).</param>
        /// <param name="componentY">Value of force component in Y direction, in <paramref name="unit"/>. (positive upwards).</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public Force(double componentX, double componentY, ForceUnit unit = ForceUnit.Newton)
			: this (UnitsNet.Force.From(componentX.ToZero(), unit), UnitsNet.Force.From(componentY.ToZero(), unit))
		{
		}

        /// <summary>
        /// Force object.
        /// </summary>
        /// <param name="forceX"><see cref="UnitsNet.Force"/> component in X direction (positive to right).</param>
        /// <param name="forceY"><see cref="UnitsNet.Force"/> component in Y direction (positive upwards).</param>
        public Force(UnitsNet.Force forceX, UnitsNet.Force forceY)
		{
			_forceX    = forceX;
			_forceY    = forceY.Unit == forceX.Unit ? forceY : forceY.ToUnit(forceX.Unit);
			_resultant = null;
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
        /// Return a copy of this <see cref="Force"/>.
        /// </summary>
        public Force Copy() => new Force(ComponentX, ComponentY, Unit);

        /// <summary>
        /// Calculate <see cref="Resultant"/> force.
        /// </summary>
        private UnitsNet.Force CalculateResultant()
        {
	        if (!_resultant.HasValue)
		        _resultant = ForceRelations.CalculateResultant(_forceX, _forceY, Unit);

	        return _resultant.Value;
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
        public static Force InX(UnitsNet.Force force) => new Force(force, UnitsNet.Force.Zero);

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
        public static Force InY(UnitsNet.Force force) => new Force(UnitsNet.Force.Zero, force);

        /// <summary>
        /// Get a <see cref="Force"/> from a resultant.
        /// </summary>
        /// <param name="resultant">Absolute value of force resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        /// <param name="unit">The <see cref="ForceUnit"/> (default: <see cref="ForceUnit.Newton"/>).</param>
        public static Force FromResultant(double resultant, double angle, ForceUnit unit = ForceUnit.Newton)
        {
	        var (x, y) = CalculateComponents(resultant, angle);

			return new Force(x, y, unit);
        }

        /// <summary>
        /// Get a <see cref="Force"/> from a resultant.
        /// </summary>
        /// <param name="resultantForce">Absolute <see cref="UnitsNet.Force"/> resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        public static Force FromResultant(UnitsNet.Force resultantForce, double angle)
        {
	        var (x, y) = CalculateComponents(resultantForce, angle);

			return
				new Force(x, y);
        }

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
				$"Fx = {_forceX}\n" +
				$"Fy = {_forceY}";
		}

        public override int GetHashCode() => (int) (ComponentX * ComponentY);
	}
}