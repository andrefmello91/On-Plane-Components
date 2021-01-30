using System;
using Extensions;
using Extensions.Number;
using UnitsNet;
using UnitsNet.Units;
using static OnPlaneComponents.ForceRelations;

namespace OnPlaneComponents
{
	/// <summary>
	///     Force struct.
	/// </summary>
	public partial struct Force : IPlaneComponent<UnitsNet.Force>, IUnitConvertible<Force, ForceUnit>, IApproachable<Force, UnitsNet.Force>, ICloneable<Force>, IEquatable<Force>
	{
		#region Fields

		/// <summary>
		///     Get a <see cref="Force" /> with zero value.
		/// </summary>
		public static readonly Force Zero = new Force(0, 0);

		/// <summary>
		///     The tolerance to consider forces equal.
		/// </summary>
		public static readonly UnitsNet.Force Tolerance = UnitsNet.Force.FromNewtons(1E-6);

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the force unit (<see cref="ForceUnit" />).
		/// </summary>
		public ForceUnit Unit
		{
			get => X.Unit;
			set => ChangeUnit(value);
		}

		public bool IsZero => IsXZero && IsYZero;

		public bool IsXZero => X.ApproxZero(Tolerance);

		public bool IsYZero => Y.ApproxZero(Tolerance);

		/// <summary>
		///     Get the force component in X direction.
		/// </summary>
		public UnitsNet.Force X { get; private set; }

		/// <summary>
		///     Get the force component in Y direction.
		/// </summary>
		public UnitsNet.Force Y { get; private set; }

		/// <summary>
		///     Verify if force resultant is approximately zero.
		/// </summary>
		public bool IsResultantZero => Resultant.Abs() <= Tolerance;

		/// <summary>
		///     Get the resultant force value.
		/// </summary>
		public UnitsNet.Force Resultant { get; private set; }

		/// <summary>
		///     Get the resultant force angle, in radians.
		/// </summary>
		public double ResultantAngle => CalculateResultantAngle(X, Y);

		#endregion

		#region Constructors

		/// <summary>
		///     Force object.
		/// </summary>
		/// <param name="componentX">Value of force component in X direction, in <paramref name="unit" />. (positive to right).</param>
		/// <param name="componentY">Value of force component in Y direction, in <paramref name="unit" />. (positive upwards).</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public Force(double componentX, double componentY, ForceUnit unit = ForceUnit.Newton)
			: this (UnitsNet.Force.From(componentX.ToZero(), unit), UnitsNet.Force.From(componentY.ToZero(), unit))
		{
		}

		/// <summary>
		///     Force object.
		/// </summary>
		/// <param name="forceX"><see cref="UnitsNet.Force" /> component in X direction (positive to right).</param>
		/// <param name="forceY"><see cref="UnitsNet.Force" /> component in Y direction (positive upwards).</param>
		public Force(UnitsNet.Force forceX, UnitsNet.Force forceY)
		{
			X         = forceX;
			Y         = forceY.ToUnit(forceX.Unit);
			Resultant = CalculateResultant(X, Y).ToUnit(forceX.Unit);
		}

		#endregion

		#region

		/// <summary>
		///     Get a <see cref="Force" /> in X direction.
		/// </summary>
		/// <param name="value">Value of force component in X direction (positive to right).</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public static Force InX(double value, ForceUnit unit = ForceUnit.Newton) => new Force(value, 0, unit);

		/// <summary>
		///     Get a <see cref="Force" /> in X direction.
		/// </summary>
		/// <param name="force"><see cref="UnitsNet.Force" /> component in X direction (positive to right).</param>
		public static Force InX(UnitsNet.Force force) => new Force(force, UnitsNet.Force.Zero);

		/// <summary>
		///     Get a <see cref="Force" /> in X direction.
		/// </summary>
		/// <param name="value">Value of force component in Y direction (positive upwards).</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public static Force InY(double value, ForceUnit unit = ForceUnit.Newton) => new Force(0, value, unit);

		/// <summary>
		///     Get a <see cref="Force" /> in Y direction.
		/// </summary>
		/// <param name="force"><see cref="UnitsNet.Force" /> component in Y direction (positive upwards).</param>
		public static Force InY(UnitsNet.Force force) => new Force(UnitsNet.Force.Zero, force);

		/// <summary>
		///     Get a <see cref="Force" /> from a resultant.
		/// </summary>
		/// <param name="resultant">Absolute value of force resultant.</param>
		/// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public static Force FromResultant(double resultant, double angle, ForceUnit unit = ForceUnit.Newton)
		{
			var (x, y) = CalculateComponents(resultant, angle);

			return new Force(x, y, unit);
		}

		/// <summary>
		///     Get a <see cref="Force" /> from a resultant.
		/// </summary>
		/// <param name="resultantForce">Absolute <see cref="UnitsNet.Force" /> resultant.</param>
		/// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
		public static Force FromResultant(UnitsNet.Force resultantForce, double angle)
		{
			var (x, y) = CalculateComponents(resultantForce, angle);

			return
				new Force(x, y);
		}

		/// <summary>
		///     Change the <see cref="ForceUnit" /> of this object.
		/// </summary>
		/// <param name="unit">The <see cref="ForceUnit" /> to convert.</param>
		public void ChangeUnit(ForceUnit unit)
		{
			if (unit == Unit)
				return;

			// Update values
			X         = X.ToUnit(unit);
			Y         = Y.ToUnit(unit);
			Resultant = CalculateResultant(X, Y).ToUnit(unit);
		}

		/// <summary>
		///     Convert this <see cref="Force" /> to another <see cref="ForceUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public Force Convert(ForceUnit unit) => unit == Unit
			? this
			: new Force(X.ToUnit(unit), Y.ToUnit(unit));

		public Force Clone() => new Force(X, Y);

		public bool Approaches(Force other, UnitsNet.Force tolerance) => X.Approx(other.X, tolerance) && Y.Approx(other.X, tolerance);

		/// <inheritdoc cref="Approaches" />
		/// <remarks>
		///     Default <see cref="Tolerance" /> is considered.
		/// </remarks>
		public bool Equals(Force other) => Approaches(other, Tolerance);

		public override bool Equals(object obj) => obj is Force other && Equals(other);

		public override string ToString() =>
			$"Fx = {X}\n" +
			$"Fy = {Y}";

		public override int GetHashCode() => (int) (X.Value * Y.Value);

		#endregion
	}
}