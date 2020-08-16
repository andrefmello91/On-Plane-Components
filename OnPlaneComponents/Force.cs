using System;
using System.Collections.Generic;
using MathNet.Numerics;
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
		// Auxiliar fields
		private readonly UnitsNet.Force
			_componentX,
			_componentY,
			_resultant;

		/// <summary>
		/// Get the force component in X direction, in the unit constructed.
		/// </summary>
		public double ComponentX => _componentX.Value;

		/// <summary>
		/// Get the force component in Y direction, in the unit constructed.
		/// </summary>
		public double ComponentY => _componentY.Value;

		/// <summary>
		/// Get the resultant force, in the unit constructed.
		/// </summary>
		public double Resultant => _resultant.Value;

		/// <summary>
		/// Get the resultant force angle, in radians.
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
			_componentX    = UnitsNet.Force.From(componentX, unit);
			_componentY    = UnitsNet.Force.From(componentY, unit);
			_resultant     = UnitsNet.Force.From(CalculateResultant(componentX, componentY), unit);
			ResultantAngle = CalculateResultantAngle(componentX, componentY);
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
        /// <param name="value">Value of force component in Y direction (positive upwards).</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public static Force InY(double value, ForceUnit unit = ForceUnit.Newton) => new Force(0, value, unit);

        /// <summary>
        /// Get a Force in from a resultant.
        /// </summary>
        /// <param name="resultant">Value of force resultant.</param>
        /// <param name="angle">Angle of force resultant, in radians (0 to Pi/2).</param>
        /// <param name="unit">The unit of force (default: N).</param>
        public static Force FromResultant(double resultant, double angle, ForceUnit unit = ForceUnit.Newton)
        {
	        var (x, y) = CalculateComponents(resultant, angle);

			return new Force(x, y, unit);
        }

        /// <summary>
        /// Calculate the resultant force.
        /// </summary>
        /// <param name="componentX">Value of force component in X direction.</param>
        /// <param name="componentY">Value of force component in Y direction.</param>
        public static double CalculateResultant(double componentX, double componentY)
        {
	        if (componentX == 0 && componentY == 0)
		        return 0;

			return
				Math.Sqrt(componentX * componentX + componentY * componentY);
		}

        /// <summary>
        /// Calculate the angle of the resultant force.
        /// </summary>
        /// <param name="componentX">Value of force component in X direction.</param>
        /// <param name="componentY">Value of force component in Y direction.</param>
        public static double CalculateResultantAngle(double componentX, double componentY)
		{
			return
				Math.Atan(componentY / componentX);
		}

        /// <summary>
        /// Calculate components of a resultant force.
        /// </summary>
        /// <param name="resultant">Value of force resultant.</param>
        /// <param name="angle">Angle of force resultant, in radians (0 to Pi/2).</param>
        public static (double X, double Y) CalculateComponents(double resultant, double angle)
        {
	        if (angle == 0)
		        return (resultant, 0);

	        if (angle == Constants.PiOver2)
		        return  (0, resultant);

            return
                (resultant * Math.Acos(angle), resultant * Math.Asin(angle));
        }

		/// <summary>
		/// Compare two force objects.
		/// </summary>
		/// <param name="other">The force to compare.</param>
		/// <returns></returns>
		public bool Equals(Force other)
		{
			return ComponentX == other.ComponentX && ComponentY == other.ComponentY;
		}

		public override bool Equals(object obj)
		{
			if (obj is Force other)
				return Equals(other);

			return false;
		}

		public override string ToString()
		{
			return
				"Fx = " + _componentX + "\n" + 
				"Fy = " + _componentY;
		}

        public override int GetHashCode()
		{
			return (int) (ComponentX * ComponentY);
		}

		public static bool operator == (Force lhs, Force rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator != (Force lhs, Force rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}