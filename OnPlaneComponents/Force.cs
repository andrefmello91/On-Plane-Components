using System;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
	/// <summary>
	/// Directions
	/// </summary>
	public enum Direction
	{
		X = 1,
		Y = 2
	}

	/// <summary>
	/// Force struct.
	/// </summary>
	public struct Force : IEquatable<Force>
	{
		// Auxiliary fields
		private UnitsNet.Force _forceX, _forceY;

		/// <summary>
        /// Get/set the force unit.
        /// </summary>
		public ForceUnit Unit
		{
			get => _forceX.Unit;
			set
			{
				_forceX.ToUnit(value);
				_forceY.ToUnit(value);
			}
		}

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
        public double Resultant => CalculateResultant(ComponentX, ComponentY);

        /// <summary>
        /// Get the resultant force, in the unit constructed (<see cref="Unit"/>).
        /// </summary>
        public UnitsNet.Force ResultantForce => CalculateResultant(_forceX, _forceY, Unit);

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
        /// <param name="componentX">Value of force component in X direction (positive to right).</param>
        /// <param name="componentY">Value of force component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public Force(double componentX, double componentY, ForceUnit unit = ForceUnit.Newton)
		{
			_forceX = UnitsNet.Force.From(componentX, unit);
			_forceY = UnitsNet.Force.From(componentY, unit);
		}

        /// <summary>
        /// Force object.
        /// </summary>
        /// <param name="forceX">Force component in X direction (positive to right).</param>
        /// <param name="forceY">Force component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public Force(UnitsNet.Force forceX, UnitsNet.Force forceY, ForceUnit unit = ForceUnit.Newton)
		{
			_forceX = forceX.ToUnit(unit);
			_forceY = forceY.ToUnit(unit);
		}

		/// <summary>
        /// Change the force unit.
        /// </summary>
        /// <param name="toUnit">The unit to convert.</param>
        public void ChangeUnit(ForceUnit toUnit)
		{
			Unit = toUnit;
		}

        /// <summary>
        /// Add values to current forces.
        /// </summary>
        /// <param name="incrementX">The increment for X component, in current unit (<see cref="Unit"/>).</param>
        /// <param name="incrementY">The increment for Y component, in current unit (<see cref="Unit"/>).</param>
        public void Add(double incrementX, double incrementY) =>
			Add(UnitsNet.Force.From(incrementX, Unit), UnitsNet.Force.From(incrementY, Unit));

        /// <summary>
        /// Add values to current forces.
        /// </summary>
        /// <param name="incrementX">The force increment for X component.</param>
        /// <param name="incrementY">The force increment for Y component.</param>
        public void Add(UnitsNet.Force incrementX, UnitsNet.Force incrementY)
		{
			_forceX += incrementX;
			_forceY += incrementY;
		}

        /// <summary>
        /// Subtract values from current forces.
        /// </summary>
        /// <param name="decrementX">The decrement for X component (positive value), in current unit (<see cref="Unit"/>).</param>
        /// <param name="decrementY">The decrement for Y component (positive value), in current unit (<see cref="Unit"/>).</param>
        public void Subtract(double decrementX, double decrementY) =>
			Subtract(UnitsNet.Force.From(decrementX, Unit), UnitsNet.Force.From(decrementY, Unit));

        /// <summary>
        /// Subtract values from current forces.
        /// </summary>
        /// <param name="decrementX">The force decrement for X component (positive value).</param>
        /// <param name="decrementY">The force decrement for Y component (positive value).</param>
        public void Subtract(UnitsNet.Force decrementX, UnitsNet.Force decrementY)
		{
			_forceX -= decrementX;
			_forceY -= decrementY;
		}

        /// <summary>
        /// Multiply current forces by a value.
        /// </summary>
        /// <param name="multiplier">The multiplier for X and Y components.</param>
        public void Multiply(double multiplier) => Multiply(multiplier, multiplier);

        /// <summary>
        /// Multiply current forces by values.
        /// </summary>
        /// <param name="multiplierX">The multiplier for X component.</param>
        /// <param name="multiplierY">The multiplier for Y component.</param>
        public void Multiply(double multiplierX, double multiplierY)
        {
	        _forceX *= multiplierX;
	        _forceY *= multiplierY;
        }

        /// <summary>
        /// Divide current forces by a value.
        /// </summary>
        /// <param name="divider">The divider for X and Y components.</param>
        public void Divide(double divider) => Divide(divider, divider);

        /// <summary>
        /// Divide current forces by values.
        /// </summary>
        /// <param name="dividerX">The divider for X component.</param>
        /// <param name="dividerY">The divider for Y component.</param>
        public void Divide(double dividerX, double dividerY)
        {
	        _forceX /= dividerX;
	        _forceY /= dividerY;
        }

        /// <summary>
        /// Get a Force with zero value.
        /// </summary>
        public static Force Zero => new Force(0, 0);

        /// <summary>
        /// Get a Force in X direction.
        /// </summary>
        /// <param name="value">Value of force component in X direction (positive to right).</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public static Force InX(double value, ForceUnit unit = ForceUnit.Newton) => new Force(value, 0, unit);

        /// <summary>
        /// Get a Force in X direction.
        /// </summary>
        /// <param name="force">Force component in X direction (positive to right).</param>
        public static Force InX(UnitsNet.Force force) => new Force(force, UnitsNet.Force.Zero, force.Unit);

        /// <summary>
        /// Get a Force in X direction.
        /// </summary>
        /// <param name="value">Value of force component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public static Force InY(double value, ForceUnit unit = ForceUnit.Newton) => new Force(0, value, unit);

        /// <summary>
        /// Get a Force in Y direction.
        /// </summary>
        /// <param name="force">Force component in Y direction (positive upwards).</param>
        public static Force InY(UnitsNet.Force force) => new Force(UnitsNet.Force.Zero, force, force.Unit);

        /// <summary>
        /// Get a Force from a resultant.
        /// </summary>
        /// <param name="resultant">Absolute value of force resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public static Force FromResultant(double resultant, double angle, ForceUnit unit = ForceUnit.Newton)
        {
	        var (x, y) = CalculateComponents(resultant, angle);

			return new Force(x, y, unit);
        }

        /// <summary>
        /// Get a Force from a resultant.
        /// </summary>
        /// <param name="resultantForce">Absolute force resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        public static Force FromResultant(UnitsNet.Force resultantForce, double angle)
        {
	        var (x, y) = CalculateComponents(resultantForce, angle);

			return
				new Force(x, y, resultantForce.Unit);
        }

        /// <summary>
        /// Calculate the absolute value of resultant force.
        /// </summary>
        /// <param name="componentX">Value of force component in X direction (positive to right).</param>
        /// <param name="componentY">Value of force component in Y direction (positive upwards).</param>
        public static double CalculateResultant(double componentX, double componentY)
        {
	        if (componentX == 0 && componentY == 0)
		        return 0;

			return
				Math.Sqrt(componentX * componentX + componentY * componentY);
		}

        /// <summary>
        /// Calculate the absolute value of resultant force.
        /// </summary>
        /// <param name="forceX">Value of force component in X direction (positive to right).</param>
        /// <param name="forceY">Value of force component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of force to return (default: N).</param>
        public static UnitsNet.Force CalculateResultant(UnitsNet.Force forceX, UnitsNet.Force forceY, ForceUnit unit = ForceUnit.Newton)
        {
	        if (forceX == UnitsNet.Force.Zero && forceY == UnitsNet.Force.Zero)
		        return UnitsNet.Force.Zero;

			return
				UnitsNet.Force.From(CalculateResultant(forceX.Value, forceY.ToUnit(forceX.Unit).Value), unit);
		}

        /// <summary>
        /// Calculate the angle of the resultant force.
        /// </summary>
        /// <param name="componentX">Value of force component in X direction.</param>
        /// <param name="componentY">Value of force component in Y direction.</param>
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
        /// Calculate the angle of the resultant force.
        /// </summary>
        /// <param name="forceX">Value of force component in X direction (positive to right).</param>
        /// <param name="forceY">Value of force component in Y direction (positive upwards).</param>
        public static double CalculateResultantAngle(UnitsNet.Force forceX, UnitsNet.Force forceY)
		{
			return
				CalculateResultantAngle(forceX.Value, forceY.ToUnit(forceX.Unit).Value);
		}

        /// <summary>
        /// Calculate components of a resultant force.
        /// </summary>
        /// <param name="resultant">Absolute value of force resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        public static (double X, double Y) CalculateComponents(double resultant, double angle)
        {
	        if (angle == 0)
		        return (resultant, 0);

	        if (angle == Constants.PiOver2)
		        return  (0, resultant);

	        if (angle == Constants.Pi)
		        return (-resultant, 0);

	        if (angle == Constants.Pi3Over2)
		        return (0, -resultant);

            return
                (resultant * Math.Acos(angle), resultant * Math.Asin(angle));
        }

        /// <summary>
        /// Calculate Force components of a resultant force.
        /// </summary>
        /// <param name="resultantForce">Absolute force resultant.</param>
        /// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
        public static (UnitsNet.Force X, UnitsNet.Force Y) CalculateComponents(UnitsNet.Force resultantForce, double angle)
        {
	        var (x, y) = CalculateComponents(resultantForce.Value, angle);

            return
                (UnitsNet.Force.From(x, resultantForce.Unit), UnitsNet.Force.From(y, resultantForce.Unit));
        }

		/// <summary>
		/// Compare two force objects.
		/// </summary>
		/// <param name="other">The force to compare.</param>
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

		/// <summary>
        /// Returns true if components are equal.
        /// </summary>
		public static bool operator == (Force left, Force right) => left.Equals(right);

		/// <summary>
		/// Returns true if components are different.
		/// </summary>
		public static bool operator != (Force left, Force right) => !left.Equals(right);

		/// <summary>
        /// Returns a force object with summed components, in left argument's unit.
        /// </summary>
		public static Force operator + (Force left, Force right) => new Force(left._forceX + right._forceX, left._forceY + right._forceY, left.Unit);

		/// <summary>
        /// Returns a force object with subtracted components, in left argument's unit.
        /// </summary>
		public static Force operator - (Force left, Force right) => new Force(left._forceX - right._forceX, left._forceY - right._forceY, left.Unit);

		/// <summary>
        /// Returns a force object with multiplied components by a double.
        /// </summary>
		public static Force operator * (Force force, double multiplier) => new Force(multiplier * force._forceX, multiplier * force._forceY, force.Unit);

		/// <summary>
        /// Returns a force object with multiplied components by a double.
        /// </summary>
		public static Force operator * (double multiplier, Force force) => force * multiplier;

		/// <summary>
        /// Returns a force object with multiplied components by an integer.
        /// </summary>
		public static Force operator * (Force force, int multiplier) => force * (double) multiplier;

		/// <summary>
        /// Returns a force object with multiplied components by an integer.
        /// </summary>
		public static Force operator * (int multiplier, Force force) => force * (double) multiplier;

		/// <summary>
		/// Returns a force object with components divided by a double.
		/// </summary>
		public static Force operator / (Force force, double divider) => new Force(force._forceX / divider, force._forceY / divider, force.Unit);

		/// <summary>
		/// Returns a force object with components divided by an integer.
		/// </summary>
		public static Force operator / (Force force, int divider) => force / (double) divider;
	}
}