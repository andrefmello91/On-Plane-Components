﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using UnitsNet;

namespace andrefmello91.OnPlaneComponents;

/// <summary>
///     Generic quantity vector class.
/// </summary>
/// <typeparam name="TQuantity">The quantity that represents the value of components of the vector.</typeparam>
/// <typeparam name="TUnit">The unit enumeration that represents the quantity of the components of this vector.</typeparam>
public abstract partial class QuantityVector<TQuantity, TUnit> : DenseVector, IUnitConvertible<TUnit>, ICloneable<QuantityVector<TQuantity, TUnit>>, IApproachable<QuantityVector<TQuantity, TUnit>, TQuantity>, IEquatable<QuantityVector<TQuantity, TUnit>>, IEnumerable<TQuantity>
	where TQuantity : struct, IQuantity<TUnit>
	where TUnit : Enum
{

	#region Fields

	private TUnit _unit;

	#endregion

	#region Properties

	/// <summary>
	///     Get/set the quantity at this index.
	/// </summary>
	/// <param name="index">The index of the component.</param>
	public new TQuantity this[int index]
	{
		get => (TQuantity) Values[index].As(_unit);
		set => Values[index] = value.As(_unit);
	}

	/// <inheritdoc />
	public TUnit Unit
	{
		get => _unit;
		set => ChangeUnit(value);
	}

	#endregion

	#region Constructors

	/// <inheritdoc />
	protected QuantityVector(IEnumerable<double> values, TUnit unit)
		: this(DenseVectorStorage<double>.OfEnumerable(values.ToArray()), unit)
	{
	}

	/// <inheritdoc />
	/// <param name="quantities">The component values of the vector.</param>
	protected QuantityVector(IEnumerable<TQuantity> quantities)
		: this(quantities.Select(v => v.As(quantities.First().Unit)), quantities.First().Unit)
	{
	}

	/// <inheritdoc />
	/// <param name="components">The component values of the vector.</param>
	protected QuantityVector(IEnumerable<IPlaneComponent<TQuantity>> components)
		: this(components.SelectMany(p => new[] { p.X, p.Y }))
	{
	}

	/// <inheritdoc />
	private QuantityVector(DenseVectorStorage<double> storage, TUnit unit)
		: base(storage) =>
		_unit = unit;

	#endregion

	#region Methods

	/// <inheritdoc cref="Vector{T}.AbsoluteMaximum" />
	public new TQuantity AbsoluteMaximum() => (TQuantity) base.AbsoluteMaximum().As(Unit);

	/// <inheritdoc cref="Vector{T}.AbsoluteMinimum" />
	public new TQuantity AbsoluteMinimum() => (TQuantity) base.AbsoluteMinimum().As(Unit);

	/// <inheritdoc cref="Vector{T}.Add(T)" />
	public QuantityVector<TQuantity, TUnit> Add(TQuantity scalar)
	{
		if (scalar.Value.Equals(0))
			return Clone();

		var result = CloneAndClear();

		DoAdd(scalar.As(Unit), result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Add(Vector{T})" />
	public QuantityVector<TQuantity, TUnit> Add(QuantityVector<TQuantity, TUnit> other)
	{
		var result = Count == other.Count
			? CloneAndClear()
			: throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));

		DoAdd(Unit.Equals(other.Unit)
				? other
				: other.Convert(Unit),
			result);

		return result;
	}

	/// <inheritdoc cref="CloneAndClear()" />
	/// <param name="count">The number of elements of the required vector.</param>
	/// <returns>
	///     A new <see cref="QuantityVector{TQuantity,TUnit}" />, with zero elements.
	/// </returns>
	public abstract QuantityVector<TQuantity, TUnit> BuildSame(int count);

	/// <summary>
	///     Create a clone of this vector and clear its content.
	/// </summary>
	/// <returns>
	///     A cleared <see cref="QuantityVector{TQuantity,TUnit}" /> clone of this vector.
	/// </returns>
	public QuantityVector<TQuantity, TUnit> CloneAndClear()
	{
		var result = Clone();
		result.Clear();
		return result;
	}

	/// <inheritdoc cref="IUnitConvertible{TUnit}.Convert" />
	public QuantityVector<TQuantity, TUnit> Convert(TUnit unit)
	{
		var result = Clone();

		if (!Unit.Equals(unit))
			result.ChangeUnit(unit);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Divide(T)" />
	public new QuantityVector<TQuantity, TUnit> Divide(double scalar)
	{
		if (scalar.Equals(1))
			return Clone();

		var result = CloneAndClear();

		DoDivide(scalar, result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Divide(T)" />
	public Vector<double> Divide(TQuantity scalar) => base.Divide(scalar.As(Unit));

	/// <inheritdoc cref="Vector{T}.DivideByThis(T)" />
	public Vector<double> DivideByThis(TQuantity scalar)
	{
		var result = Build.Dense(Count);

		DoDivideByThis(scalar.As(Unit), result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.DotProduct" />
	public double DotProduct(QuantityVector<TQuantity, TUnit> other) => Count == other.Count
		? DoDotProduct(other.Unit.Equals(Unit)
			? other
			: other.Convert(Unit))
		: throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));

	/// <inheritdoc cref="object.Equals(object)" />
	public new bool Equals(object? obj) =>
		obj is QuantityVector<TQuantity, TUnit> other && Equals(other);

	/// <summary>
	///     Multiply this vector by a matrix on the left side.
	/// </summary>
	/// <remarks>
	///     <c>Result = this * matrix</c>
	/// </remarks>
	/// <param name="matrix">The double matrix.</param>
	public QuantityVector<TQuantity, TUnit> LeftMultiply(Matrix<double> matrix)
	{
		if (Count != matrix.RowCount)
			throw new ArgumentException("Matrix and vector sizes don's match.");

		var result = BuildSame(matrix.ColumnCount);

		matrix.LeftMultiply(this, result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Maximum" />
	public TQuantity Maximum() => (TQuantity) base.Maximum().As(Unit);

	/// <inheritdoc cref="Vector{T}.Minimum" />
	public TQuantity Minimum() => (TQuantity) base.Minimum().As(Unit);

	/// <inheritdoc cref="Vector{T}.Multiply(T)" />
	public new QuantityVector<TQuantity, TUnit> Multiply(double scalar)
	{
		if (scalar.Equals(1))
			return Clone();

		var result = CloneAndClear();

		if (scalar.Equals(0))
			return result;

		DoMultiply(scalar, result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Negate()" />
	public new QuantityVector<TQuantity, TUnit> Negate()
	{
		var result = CloneAndClear();

		DoNegate(result);

		return result;
	}

	/// <summary>
	///     Multiply this vector by a matrix on the right side.
	/// </summary>
	/// <remarks>
	///     <c>Result = matrix * this</c>
	/// </remarks>
	/// <param name="matrix">The double matrix.</param>
	public QuantityVector<TQuantity, TUnit> RightMultiply(Matrix<double> matrix)
	{
		if (Count != matrix.ColumnCount)
			throw new ArgumentException("Matrix and vector sizes don's match.");

		var result = BuildSame(matrix.RowCount);

		matrix.Multiply(this, result);

		return result;
	}

	/// <summary>
	///     Get the vector simplified by constrained DoFs.
	/// </summary>
	/// <param name="threshold">
	///     A value for setting all values whose absolute value is smaller than to zero. If null, this is
	///     not applied.
	/// </param>
	/// <param name="indexes">The collection of indexes to simplify.</param>
	/// <returns>
	///     The simplified <see cref="Vector{T}" />.
	/// </returns>
	public QuantityVector<TQuantity, TUnit> Simplified(IEnumerable<int>? indexes, double? threshold)
	{
		var result = Clone();

		if (indexes is not null)
			foreach (var index in indexes)
				result[index] = (TQuantity) 0.As(Unit);

		if (threshold.HasValue)
			result.CoerceZero(threshold.Value);

		return result;
	}

	/// <inheritdoc cref="Simplified(IEnumerable{int}?, double?)" />
	public QuantityVector<TQuantity, TUnit> Simplified(IEnumerable<int>? indexes, TQuantity? threshold) => Simplified(indexes, threshold?.As(Unit));

	/// <inheritdoc cref="Vector{T}.Subtract(T)" />
	public QuantityVector<TQuantity, TUnit> Subtract(TQuantity scalar)
	{
		if (scalar.Value.Equals(0))
			return Clone();

		var result = CloneAndClear();

		DoSubtract(scalar.As(Unit), result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Subtract(Vector{T})" />
	public QuantityVector<TQuantity, TUnit> Subtract(QuantityVector<TQuantity, TUnit> other)
	{
		var result = Count == other.Count
			? CloneAndClear()
			: throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));

		DoSubtract(Unit.Equals(other.Unit)
				? other
				: other.Convert(Unit),
			result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.SubtractFrom(T)" />
	public QuantityVector<TQuantity, TUnit> SubtractFrom(TQuantity scalar)
	{
		if (scalar.Value.Equals(0))
			return -Clone();

		var result = CloneAndClear();

		DoSubtractFrom(scalar.As(Unit), result);

		return result;
	}

	/// <inheritdoc cref="Vector{T}.Sum" />
	public new TQuantity Sum() => (TQuantity) base.Sum().As(Unit);

	/// <inheritdoc cref="object.ToString" />
	public new string ToString() =>
		$"Unit: {Unit} \n" +
		$"Value: {base.ToString()}";

	/// <inheritdoc />
	public bool Approaches(QuantityVector<TQuantity, TUnit>? other, TQuantity tolerance) => other is not null &&
	                                                                                        (this - other).AbsoluteMaximum().As(Unit) <= tolerance.As(Unit);

	/// <inheritdoc cref="ICloneable{T}.Clone" />
	public abstract QuantityVector<TQuantity, TUnit> Clone();

	/// <inheritdoc />
	public IEnumerator<TQuantity> GetEnumerator() => Values
		.GetQuantities<TQuantity, TUnit>(Unit)
		.GetEnumerator();

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc />
	public virtual bool Equals(QuantityVector<TQuantity, TUnit>? other) =>
		other is not null &&
		base.Equals(Unit.Equals(other.Unit)
			? other
			: other.Convert(Unit));

	/// <inheritdoc />
	public void ChangeUnit(TUnit unit)
	{
		if (_unit.Equals(unit))
			return;

		// Multiply matrix
		var multiplier = 1.As(Unit).As(unit);
		MapInplace(x => x * multiplier);

		_unit = unit;
	}

	IUnitConvertible<TUnit> IUnitConvertible<TUnit>.Convert(TUnit unit) => Convert(unit);

	#endregion

}