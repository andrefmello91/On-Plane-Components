﻿using System;
using Extensions;
using UnitsNet;
using UnitsNet.Units;
using static OnPlaneComponents.DisplacementRelations;

namespace OnPlaneComponents
{
	/// <summary>
	///     Displacement struct.
	/// </summary>
	public partial struct PlaneDisplacement : IPlaneComponent<PlaneDisplacement, Length>, IUnitConvertible<PlaneDisplacement, LengthUnit>
	{
		#region Fields

		/// <summary>
		///     The tolerance to consider displacements equal.
		/// </summary>
		public static readonly Length Tolerance = Length.FromMillimeters(1E-6);

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> with zero value.
		/// </summary>
		public static readonly PlaneDisplacement Zero = new PlaneDisplacement(0, 0);

		#endregion

		#region Properties

		/// <summary>
		///     Verify if displacement components are zero.
		/// </summary>
		public bool IsZero => IsXZero && IsYZero;

		/// <summary>
		///     Verify if X component is approximately zero.
		/// </summary>
		public bool IsXZero => X.ApproxZero(Tolerance);

		/// <summary>
		///     Verify if Y component is approximately zero.
		/// </summary>
		public bool IsYZero => Y.ApproxZero(Tolerance);

		/// <summary>
		///     Get the existent displacement directions.
		/// </summary>
		public ComponentDirection Direction => IsZero
			? ComponentDirection.None
			: IsXZero switch
			{
				false when IsYZero => ComponentDirection.X,
				true when !IsYZero => ComponentDirection.Y,
				_ => ComponentDirection.Both
			};

		/// <summary>
		///     Get/set the displacement unit (<see cref="LengthUnit" />).
		/// </summary>
		public LengthUnit Unit
		{
			get => X.Unit;
			set => ChangeUnit(value);
		}

		/// <summary>
		///     Get the displacement component in X direction.
		/// </summary>
		public Length X { get; private set; }

		/// <summary>
		///     Get the displacement component in Y direction.
		/// </summary>
		public Length Y { get; private set; }

		/// <summary>
		///     Verify if displacement resultant is approximately zero.
		/// </summary>
		public bool IsResultantZero => Resultant.ApproxZero(Tolerance);

		/// <summary>
		///     Get the resultant displacement value.
		/// </summary>
		public Length Resultant { get; private set; }

		/// <summary>
		///     Get the resultant displacement angle, in radians.
		/// </summary>
		public double ResultantAngle => CalculateResultantAngle(X, Y);

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
		public PlaneDisplacement(double componentX, double componentY, LengthUnit unit = LengthUnit.Millimeter)
			: this (Length.From(componentX.ToZero(), unit), Length.From(componentY.ToZero(), unit))
		{
		}

		/// <summary>
		///     Displacement object.
		/// </summary>
		/// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length" />).</param>
		/// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length" />).</param>
		public PlaneDisplacement(Length displacementX, Length displacementY)
		{
			X          = displacementX.ToZero();
			Y          = displacementY.ToZero().ToUnit(displacementX.Unit);
			Resultant  = CalculateResultant(X, Y, X.Unit);
		}

		#endregion

		#region

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> in X direction.
		/// </summary>
		/// <param name="value">Value of displacement component in X direction (positive to right).</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public static PlaneDisplacement InX(double value, LengthUnit unit = LengthUnit.Millimeter) => new PlaneDisplacement(value, 0, unit);

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> in X direction.
		/// </summary>
		/// <param name="displacement">Displacement component in X direction (positive to right).</param>
		public static PlaneDisplacement InX(Length displacement) => new PlaneDisplacement(displacement, Length.Zero);

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> in X direction.
		/// </summary>
		/// <param name="value">Value of displacement component in Y direction (positive upwards).</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public static PlaneDisplacement InY(double value, LengthUnit unit = LengthUnit.Millimeter) => new PlaneDisplacement(0, value, unit);

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> in Y direction.
		/// </summary>
		/// <param name="displacement">Displacement component in Y direction (positive to right).</param>
		public static PlaneDisplacement InY(Length displacement) => new PlaneDisplacement(Length.Zero, displacement);

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> in from a resultant.
		/// </summary>
		/// <param name="resultant">Absolute value of displacement resultant.</param>
		/// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
		/// <param name="unit">The <see cref="LengthUnit" /> of displacement (default: <see cref="LengthUnit.Millimeter" />).</param>
		public static PlaneDisplacement FromResultant(double resultant, double angle, LengthUnit unit = LengthUnit.Millimeter) => FromResultant(Length.From(resultant, unit), angle);

		/// <summary>
		///     Get a <see cref="PlaneDisplacement" /> in from a resultant.
		/// </summary>
		/// <param name="resultantDisplacement">Absolute value of displacement resultant.</param>
		/// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
		public static PlaneDisplacement FromResultant(Length resultantDisplacement, double angle)
		{
			var (x, y) = CalculateComponents(resultantDisplacement, angle);

			return new PlaneDisplacement(x, y);
		}

		/// <summary>
		///     Change the <see cref="LengthUnit" /> of this object.
		/// </summary>
		/// <param name="unit">The <see cref="LengthUnit" /> to convert.</param>
		public void ChangeUnit(LengthUnit unit)
		{
			if (unit == Unit)
				return;

			// Update values
			X = X.ToUnit(unit);
			Y = Y.ToUnit(unit);
			Resultant  = CalculateResultant(X, Y).ToUnit(unit);
		}

		/// <summary>
		///     Convert this <see cref="PlaneDisplacement" /> to another <see cref="LengthUnit" />.
		/// </summary>
		/// <param name="unit">The <see cref="LengthUnit" /> to convert.</param>
		public PlaneDisplacement Convert(LengthUnit unit) => unit == Unit
			? this
			: new PlaneDisplacement(X.ToUnit(unit), Y.ToUnit(unit));

		/// <inheritdoc />
		public bool Approaches(PlaneDisplacement other, Length tolerance) => X.Approx(other.X, tolerance) && Y.Approx(other.Y, tolerance);

		public PlaneDisplacement Clone() => new PlaneDisplacement(X, Y);

		/// <inheritdoc cref="Approaches" />
		/// <remarks>
		///     Default <see cref="Tolerance" /> is considered.
		/// </remarks>
		public bool Equals(PlaneDisplacement other) => Approaches(other, Tolerance);

		public override bool Equals(object obj) => obj is PlaneDisplacement other && Equals(other);

		public override string ToString() =>
			$"ux = {X}\n" +
			$"uy = {Y}";

		public override int GetHashCode() => (int) (X.Value * Y.Value);

		#endregion
	}
}