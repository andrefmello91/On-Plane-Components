using System;
using Extensions.Number;
using UnitsNet;
using UnitsNet.Units;
using static OnPlaneComponents.DisplacementRelations;

namespace OnPlaneComponents
{
	/// <summary>
	///     Displacement struct.
	/// </summary>
	public partial struct Displacement : IUnitConvertible<Displacement, LengthUnit>, IEquatable<Displacement>
	{
		#region Fields

		/// <summary>
		///     Get a <see cref="Displacement" /> with zero value.
		/// </summary>
		public static readonly Displacement Zero = new Displacement(0, 0);

		// Auxiliar fields
		private Length _displacementX, _displacementY, _resultant;

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the displacement unit (<see cref="LengthUnit" />).
		/// </summary>
		public LengthUnit Unit
		{
			get => _displacementX.Unit;
			set => ChangeUnit(value);
		}

		/// <summary>
		///     Verify if displacement components are zero.
		/// </summary>
		public bool AreComponentsZero => IsComponentXZero && IsComponentYZero;

		/// <summary>
		///     Get the displacement component in X direction, in the unit constructed (<see cref="Unit" />).
		/// </summary>
		public double ComponentX => _displacementX.Value;

		/// <summary>
		///     Get the displacement component in Y direction, in the unit constructed (<see cref="Unit" />).
		/// </summary>
		public double ComponentY => _displacementY.Value;

		/// <summary>
		///     Verify if X component is zero.
		/// </summary>
		public bool IsComponentXZero => ComponentX.ApproxZero();

		/// <summary>
		///     Verify if Y component is zero.
		/// </summary>
		public bool IsComponentYZero => ComponentY.ApproxZero();

		/// <summary>
		///     Verify if displacement resultant is zero.
		/// </summary>
		public bool IsResultantZero => Resultant.ApproxZero();

		/// <summary>
		///     Get the resultant displacement value, in the unit constructed (<see cref="Unit" />).
		/// </summary>
		public double Resultant => _resultant.Value;

		/// <summary>
		///     Get the resultant displacement angle, in radians.
		/// </summary>
		public double ResultantAngle => CalculateResultantAngle(ComponentX, ComponentY);

		#endregion

		#region Constructors

		/// <summary>
		///     Displacement object.
		/// </summary>
		/// <param name="componentX">
		///     Value of displacement component in X direction, in <paramref name="unit" />. (positive to
		///     right).
		/// </param>
		/// <param name="componentY">
		///     Value of displacement component in Y direction, in <paramref name="unit" />. (positive
		///     upwards).
		/// </param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public Displacement(double componentX, double componentY, LengthUnit unit = LengthUnit.Millimeter)
			: this (Length.From(componentX.ToZero(), unit), Length.From(componentY.ToZero(), unit))
		{
		}

		/// <summary>
		///     Displacement object.
		/// </summary>
		/// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length" />).</param>
		/// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length" />).</param>
		public Displacement(Length displacementX, Length displacementY)
		{
			_displacementX = displacementX;
			_displacementY = displacementY.ToUnit(displacementX.Unit);
			_resultant     = CalculateResultant(_displacementX, _displacementY, _displacementX.Unit);
		}

		#endregion

		#region  Methods

		/// <summary>
		///     Get a <see cref="Displacement" /> in X direction.
		/// </summary>
		/// <param name="value">Value of displacement component in X direction (positive to right).</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public static Displacement InX(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(value, 0, unit);

		/// <summary>
		///     Get a <see cref="Displacement" /> in X direction.
		/// </summary>
		/// <param name="displacement">Displacement component in X direction (positive to right).</param>
		public static Displacement InX(Length displacement) => new Displacement(displacement, Length.Zero);

		/// <summary>
		///     Get a <see cref="Displacement" /> in X direction.
		/// </summary>
		/// <param name="value">Value of displacement component in Y direction (positive upwards).</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public static Displacement InY(double value, LengthUnit unit = LengthUnit.Millimeter) => new Displacement(0, value, unit);

		/// <summary>
		///     Get a <see cref="Displacement" /> in Y direction.
		/// </summary>
		/// <param name="displacement">Displacement component in Y direction (positive to right).</param>
		public static Displacement InY(Length displacement) => new Displacement(Length.Zero, displacement);

		/// <summary>
		///     Get a <see cref="Displacement" /> in from a resultant.
		/// </summary>
		/// <param name="resultant">Absolute value of displacement resultant.</param>
		/// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public static Displacement FromResultant(double resultant, double angle, LengthUnit unit = LengthUnit.Millimeter) => FromResultant(Length.From(resultant, unit), angle);

		/// <summary>
		///     Get a <see cref="Displacement" /> in from a resultant.
		/// </summary>
		/// <param name="resultantDisplacement">Absolute value of displacement resultant.</param>
		/// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
		public static Displacement FromResultant(Length resultantDisplacement, double angle)
		{
			var (x, y) = CalculateComponents(resultantDisplacement, angle);

			return new Displacement(x, y);
		}

		/// <summary>
		///     Return a copy of this <see cref="Displacement" />.
		/// </summary>
		public Displacement Copy() => new Displacement(_displacementX, _displacementY);

		/// <summary>
		///     Change the <see cref="LengthUnit" /> of this object.
		/// </summary>
		/// <param name="unit">The <see cref="LengthUnit" /> to convert.</param>
		public void ChangeUnit(LengthUnit unit)
		{
			if (unit == Unit)
				return;

			// Update values
			_displacementX = _displacementX.ToUnit(unit);
			_displacementY = _displacementY.ToUnit(unit);
			_resultant     = CalculateResultant(_displacementX, _displacementY, unit);
		}

		/// <summary>
		///     Convert this <see cref="Displacement" /> to another <see cref="LengthUnit" />.
		/// </summary>
		/// <param name="unit">The <see cref="LengthUnit" /> to convert.</param>
		public Displacement Convert(LengthUnit unit) => unit == Unit
			? this
			: new Displacement(_displacementX.ToUnit(unit), _displacementY.ToUnit(unit));

		/// <summary>
		///     Compare two <see cref="Displacement" /> objects.
		/// </summary>
		/// <param name="other">The <see cref="Displacement" /> to compare.</param>
		public bool Equals(Displacement other) => _displacementX == other._displacementX && _displacementY == other._displacementY;

		public override bool Equals(object obj)
		{
			if (obj is Displacement other)
				return Equals(other);

			return false;
		}

		public override string ToString() =>
			$"ux = {_displacementX}\n" +
			$"uy = {_displacementY}";

		public override int GetHashCode() => (int) (ComponentX * ComponentY);

		#endregion
	}
}