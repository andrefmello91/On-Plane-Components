using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace andrefmello91.OnPlaneComponents
{
	public partial class QuantityMatrix<TQuantity, TUnit>
	{
		/// <inheritdoc cref="Add(TQuantity)" />
		public static QuantityMatrix<TQuantity, TUnit> operator +(QuantityMatrix<TQuantity, TUnit> left, TQuantity right) => left.Add(right);

		/// <inheritdoc cref="Add(TQuantity)" />
		public static QuantityMatrix<TQuantity, TUnit> operator +(TQuantity left, QuantityMatrix<TQuantity, TUnit> right) => right.Add(left);

		/// <inheritdoc cref="Add(QuantityMatrix{TQuantity, TUnit})" />
		public static QuantityMatrix<TQuantity, TUnit> operator +(QuantityMatrix<TQuantity, TUnit> left, QuantityMatrix<TQuantity, TUnit> right) => left.Add(right);

		/// <inheritdoc cref="Divide(double)" />
		public static QuantityMatrix<TQuantity, TUnit> operator /(QuantityMatrix<TQuantity, TUnit> matrix, double divisor) => matrix.Divide(divisor);

		/// <inheritdoc cref="Divide(TQuantity)" />
		public static Matrix<double> operator /(QuantityMatrix<TQuantity, TUnit> matrix, TQuantity divisor) => matrix.Divide(divisor);

		/// <inheritdoc cref="DivideByThis(TQuantity)" />
		public static Matrix<double> operator /(TQuantity quantity, QuantityMatrix<TQuantity, TUnit> matrix) => matrix.DivideByThis(quantity);

		/// <returns>
		///     True if objects are equal.
		/// </returns>
		public static bool operator ==(QuantityMatrix<TQuantity, TUnit>? left, QuantityMatrix<TQuantity, TUnit>? right) => left.IsEqualTo(right);

		/// <returns>
		///     True if objects are not equal.
		/// </returns>
		public static bool operator !=(QuantityMatrix<TQuantity, TUnit>? left, QuantityMatrix<TQuantity, TUnit>? right) => left.IsNotEqualTo(right);

		/// <inheritdoc cref="Multiply(double)" />
		public static QuantityMatrix<TQuantity, TUnit> operator *(QuantityMatrix<TQuantity, TUnit> matrix, double multiplier) => matrix.Multiply(multiplier);

		/// <inheritdoc cref="Multiply(double)" />
		public static QuantityMatrix<TQuantity, TUnit> operator *(double multiplier, QuantityMatrix<TQuantity, TUnit> matrix) => matrix.Multiply(multiplier);

		/// <inheritdoc cref="Multiply(Matrix{double})" />
		public static QuantityMatrix<TQuantity, TUnit> operator *(QuantityMatrix<TQuantity, TUnit> matrix, Matrix<double> multiplier) => matrix.Multiply(multiplier);

		/// <inheritdoc cref="MultiplyBy(Matrix{double})" />
		public static QuantityMatrix<TQuantity, TUnit> operator *(Matrix<double> multiplier, QuantityMatrix<TQuantity, TUnit> matrix) => matrix.MultiplyBy(multiplier);

		/// <inheritdoc cref="Subtract(TQuantity)" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(QuantityMatrix<TQuantity, TUnit> left, TQuantity right) => left.Subtract(right);

		/// <inheritdoc cref="Subtract(QuantityMatrix{TQuantity, TUnit})" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(QuantityMatrix<TQuantity, TUnit> left, QuantityMatrix<TQuantity, TUnit> right) => left.Subtract(right);

		/// <inheritdoc cref="SubtractFrom(TQuantity)" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(TQuantity left, QuantityMatrix<TQuantity, TUnit> right) => right.SubtractFrom(left);

		/// <inheritdoc cref="Negate" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(QuantityMatrix<TQuantity, TUnit> right) => right.Negate();

	}
}