using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Statistics;
using UnitsNet;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Generic stiffness matrix class.
	/// </summary>
	/// <typeparam name="TQuantity">The quantity that represents the value of components of the matrix.</typeparam>
	/// <typeparam name="TUnit">The unit enumeration that represents the quantity of the components of the matrix.</typeparam>
	public abstract partial class QuantityMatrix<TQuantity, TUnit> : DenseMatrix, IUnitConvertible<TUnit>, ICloneable<QuantityMatrix<TQuantity, TUnit>>, IApproachable<QuantityMatrix<TQuantity, TUnit>, TQuantity>, IEquatable<QuantityMatrix<TQuantity, TUnit>>, IEnumerable<TQuantity>
		where TQuantity : IQuantity<TUnit>
		where TUnit : Enum
	{

		#region Fields

		private TUnit _unit;

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the value at these indexes.
		/// </summary>
		/// <param name="rowIndex">The row of the required element.</param>
		/// <param name="columnIndex">The column of the required element.</param>
		public new TQuantity this[int rowIndex, int columnIndex]
		{
			get => (TQuantity) base[rowIndex, columnIndex].As(_unit);
			set => base[rowIndex, columnIndex] = value.As(_unit);
		}

		#region Interface Implementations

		/// <inheritdoc />
		public TUnit Unit
		{
			get => _unit;
			set => ChangeUnit(value);
		}

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		///     Create a quantity matrix.
		/// </summary>
		/// <inheritdoc />
		/// <param name="unit">The unit of matrix components</param>
		protected QuantityMatrix(DenseColumnMajorMatrixStorage<double> storage, TUnit unit)
			: base(storage) =>
			_unit = unit;

		/// <summary>
		///     Create a quantity matrix.
		/// </summary>
		/// <inheritdoc />
		/// <param name="unit">The unit of <paramref name="values" />'s components</param>
		protected QuantityMatrix(int rows, int columns, TUnit unit)
			: base(rows, columns) =>
			_unit = unit;

		/// <summary>
		///     Create a quantity matrix.
		/// </summary>
		/// <inheritdoc />
		/// <param name="unit">The unit of <paramref name="values" />'s components</param>
		protected QuantityMatrix(int rows, int columns, double[] storage, TUnit unit)
			: base(rows, columns, storage) =>
			_unit = unit;

		/// <summary>
		///     Create a quantity matrix.
		/// </summary>
		/// <param name="values">The array of values.</param>
		/// <param name="unit">The unit of <paramref name="values" />'s components</param>
		protected QuantityMatrix(double[,] values, TUnit unit)
			: this(DenseColumnMajorMatrixStorage<double>.OfArray(values), unit)
		{
		}

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}(double[,],TUnit)" />
		protected QuantityMatrix(Matrix<double> value, TUnit unit)
			: this(value.ToArray(), unit)
		{
		}

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}(double[,],TUnit)" />
		protected QuantityMatrix(TQuantity[,] value)
			: this(value.GetValues<TQuantity, TUnit>(), value[0, 0].Unit)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Get the global stiffness simplified.
		/// </summary>
		/// <param name="stiffness">The global stiffness <see cref="Matrix{T}" /> to simplify, in place.</param>
		/// <param name="indexes">The DoF indexes to simplify matrix.</param>
		/// <param name="simplifyZeroRows">Simplify matrix at rows containing only zero elements?</param>
		protected static void Simplify(Matrix<double> stiffness, IEnumerable<int> indexes, bool simplifyZeroRows = true)
		{
			var index = indexes.ToArray();

			// Clear the rows and columns in the stiffness matrix
			stiffness.ClearRows(index);
			stiffness.ClearColumns(index);

			// Set the diagonal element to 1
			foreach (var i in index)
				stiffness[i, i] = 1;

			if (!simplifyZeroRows)
				return;

			// Verify rows
			for (var i = 0; i < stiffness.RowCount; i++)
			{
				// Verify what line of the matrix is composed of zeroes
				if (stiffness.Row(i).Exists(num => !num.Equals(0)) && stiffness.Column(i).Exists(num => !num.Equals(0)))
					continue;

				// The row is composed of only zeroes, so the displacement must be zero
				// Set the diagonal element to 1
				stiffness[i, i] = 1;
			}
		}

		/// <inheritdoc cref="Matrix{T}.Add(T)" />
		public QuantityMatrix<TQuantity, TUnit> Add(TQuantity scalar)
		{
			var result = Clone();

			if (scalar.Value.Equals(0))
				return result;

			result.Clear();

			DoAdd(scalar.As(Unit), result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Add(Matrix{T})" />
		public virtual QuantityMatrix<TQuantity, TUnit> Add(QuantityMatrix<TQuantity, TUnit> other)
		{
			var result = other.Clone();

			result.Clear();

			Add(other.Unit.Equals(Unit)
				? other
				: other.Convert(Unit), result);

			return result;
		}

		/// <inheritdoc cref="IUnitConvertible{T}.Convert" />
		public QuantityMatrix<TQuantity, TUnit> Convert(TUnit unit)
		{
			var result = Clone();

			if (!Unit.Equals(unit))
				result.ChangeUnit(unit);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Divide(T)" />
		public new QuantityMatrix<TQuantity, TUnit> Divide(double scalar)
		{
			var result = Clone();

			if (scalar.Equals(1))
				return result;

			result.Clear();

			DoDivide(scalar, result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Divide(T)" />
		public Matrix<double> Divide(TQuantity scalar) => base.Divide(scalar.As(Unit));

		/// <inheritdoc cref="Matrix{T}.DivideByThis(T)" />
		public Matrix<double> DivideByThis(TQuantity scalar)
		{
			var result = Build.Dense(RowCount, ColumnCount);

			if (scalar.Value.Equals(0))
				return result;

			DoDivideByThis(scalar.As(Unit), result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Multiply(T)" />
		public new QuantityMatrix<TQuantity, TUnit> Multiply(double scalar)
		{
			var result = Clone();

			if (scalar.Equals(1))
				return result;

			result.Clear();

			if (scalar.Equals(0))
				return result;

			DoMultiply(scalar, result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Multiply(Matrix{T})" />
		public new QuantityMatrix<TQuantity, TUnit> Multiply(Matrix<double> other)
		{
			var result = Clone();

			result.Clear();

			Multiply(other, result);

			return result;
		}

		/// <summary>
		///     Multiply the other matrix by this.
		/// </summary>
		/// <param name="other">The matrix to multiply by this</param>
		public QuantityMatrix<TQuantity, TUnit> MultiplyBy(Matrix<double> other)
		{
			var result = Clone();

			result.Clear();

			other.Multiply(this, result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Negate()" />
		public new QuantityMatrix<TQuantity, TUnit> Negate()
		{
			var result = Clone();

			result.Clear();

			DoNegate(result);

			return result;
		}

		/// <summary>
		///     Get the simplified stiffness matrix by the constrained DoFs.
		/// </summary>
		/// <param name="constraintIndexes">The indexes to simplify.</param>
		/// <param name="threshold">
		///     A value for setting all values whose absolute value is smaller than to zero. If null, this is
		///     not applied.
		/// </param>
		/// <returns>
		///     The simplified <see cref="Matrix{T}" />.
		/// </returns>
		public QuantityMatrix<TQuantity, TUnit> Simplified(IEnumerable<int>? constraintIndexes, double? threshold)
		{
			var value = Clone();

			if (threshold.HasValue)
				value.CoerceZero(threshold.Value);

			if (constraintIndexes is not null)
				Simplify(value, constraintIndexes);

			return value;
		}

		/// <inheritdoc cref="Simplified(IEnumerable{int}?, double?)" />
		public QuantityMatrix<TQuantity, TUnit> Simplified(IEnumerable<int>? constraintIndexes, TQuantity? threshold) => Simplified(constraintIndexes, threshold?.As(Unit));

		/// <inheritdoc cref="Matrix{T}.Subtract(T)" />
		public QuantityMatrix<TQuantity, TUnit> Subtract(TQuantity scalar)
		{
			var result = Clone();

			if (scalar.Value.Equals(0))
				return result;

			result.Clear();

			DoSubtract(scalar.As(Unit), result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.Subtract(Matrix{T})" />
		public virtual QuantityMatrix<TQuantity, TUnit> Subtract(QuantityMatrix<TQuantity, TUnit> other)
		{
			var result = other.Clone();

			result.Clear();

			Subtract(other.Unit.Equals(Unit)
				? other
				: other.Convert(Unit), result);

			return result;
		}

		/// <inheritdoc cref="Matrix{T}.SubtractFrom(T)" />
		public QuantityMatrix<TQuantity, TUnit> SubtractFrom(TQuantity scalar)
		{
			var result = Clone();

			if (scalar.Value.Equals(0))
				return result;

			result.Clear();

			DoSubtractFrom(scalar.As(Unit), result);

			return result;
		}

		/// <summary>
		///     Transform this stiffness to another coordinate system.
		/// </summary>
		/// <param name="transformationMatrix">The transformation matrix.</param>
		/// <returns>
		///     A new <see cref="StiffnessMatrix" /> with transformed components.
		/// </returns>
		/// <exception cref="ArgumentException">
		///     If the dimensions of <paramref name="transformationMatrix" /> don't conform with
		///     this.
		/// </exception>
		public virtual QuantityMatrix<TQuantity, TUnit> Transform(Matrix<double> transformationMatrix) => transformationMatrix.Transpose() * this * transformationMatrix;

		/// <inheritdoc cref="Matrix{T}.Transpose()" />
		public new QuantityMatrix<TQuantity, TUnit> Transpose()
		{
			var result = Clone();

			result.Clear();

			Transpose(result);

			return result;
		}

		#region Interface Implementations

		/// <inheritdoc />
		public abstract QuantityMatrix<TQuantity, TUnit> Clone();

		/// <inheritdoc />
		public virtual bool Equals(QuantityMatrix<TQuantity, TUnit>? other) =>
			other is not null &&
			base.Equals(other.Unit.Equals(Unit)
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

			// Set
			_unit = unit;
		}

		/// <inheritdoc />
		IUnitConvertible<TUnit> IUnitConvertible<TUnit>.Convert(TUnit unit) => Convert(unit);

		#endregion

		#region Object override

		/// <inheritdoc />
		public virtual bool Approaches(QuantityMatrix<TQuantity, TUnit>? other, TQuantity tolerance) =>
			other is not null &&
			(this - other).Values.MaximumAbsolute().As(Unit).Value <= tolerance.As(Unit);

		/// <inheritdoc />
		public IEnumerator<TQuantity> GetEnumerator() => Values
			.Select(v => v.As(Unit))
			.Cast<TQuantity>()
			.GetEnumerator();

		/// <inheritdoc />
		public override bool Equals(object? obj) =>
			obj is QuantityMatrix<TQuantity, TUnit> other && Equals(other);

		/// <inheritdoc />
		public override int GetHashCode() => _unit.GetHashCode() * base.GetHashCode();
		
		/// <inheritdoc cref="object.ToString" />
		public new string ToString() =>
			$"Unit: {Unit} \n" +
			$"Value: {base.ToString()}";

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#endregion

	}
}