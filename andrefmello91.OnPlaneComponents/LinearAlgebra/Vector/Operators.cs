using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace andrefmello91.OnPlaneComponents
{
	public partial class QuantityVector<TQuantity, TUnit>
	{

		#region Operators

		/// <inheritdoc cref="Add(TQuantity)" />
		public static QuantityVector<TQuantity, TUnit> operator +(QuantityVector<TQuantity, TUnit> left, TQuantity right) => left.Add(right);

		/// <inheritdoc cref="Add(QuantityVector{TQuantity, TUnit})" />
		public static QuantityVector<TQuantity, TUnit> operator +(QuantityVector<TQuantity, TUnit> left, QuantityVector<TQuantity, TUnit> right) => left.Add(right);

		/// <inheritdoc cref="Divide(double)" />
		public static QuantityVector<TQuantity, TUnit> operator /(QuantityVector<TQuantity, TUnit> left, double divisor) => left.Divide(divisor);

		/// <inheritdoc cref="Divide(TQuantity)" />
		public static Vector<double> operator /(QuantityVector<TQuantity, TUnit> left, TQuantity divisor) => left.Divide(divisor);

		/// <inheritdoc cref="DivideByThis(TQuantity)" />
		public static Vector<double> operator /(TQuantity left, QuantityVector<TQuantity, TUnit> divisor) => divisor.DivideByThis(left);

		/// <inheritdoc cref="StiffnessMatrix.op_Equality" />
		public static bool operator ==(QuantityVector<TQuantity, TUnit>? left, QuantityVector<TQuantity, TUnit>? right) => left.IsEqualTo(right);

		/// <inheritdoc cref="StiffnessMatrix.op_Inequality" />
		public static bool operator !=(QuantityVector<TQuantity, TUnit>? left, QuantityVector<TQuantity, TUnit>? right) => left.IsNotEqualTo(right);

		/// <inheritdoc cref="Multiply(double)" />
		public static QuantityVector<TQuantity, TUnit> operator *(QuantityVector<TQuantity, TUnit> left, double multiplier) => left.Multiply(multiplier);

		/// <inheritdoc cref="Multiply(double)" />
		public static QuantityVector<TQuantity, TUnit> operator *(double multiplier, QuantityVector<TQuantity, TUnit> right) => right.Multiply(multiplier);

		/// <inheritdoc cref="RightMultiply" />
		public static QuantityVector<TQuantity, TUnit> operator *(Matrix<double> multiplier, QuantityVector<TQuantity, TUnit> right) => right.RightMultiply(multiplier);

		/// <inheritdoc cref="LeftMultiply" />
		public static QuantityVector<TQuantity, TUnit> operator *(QuantityVector<TQuantity, TUnit> left, Matrix<double> multiplier) => left.LeftMultiply(multiplier);

		/// <inheritdoc cref="DotProduct" />
		public static double operator *(QuantityVector<TQuantity, TUnit> left, QuantityVector<TQuantity, TUnit> right) => left.DotProduct(right);

		/// <inheritdoc cref="Subtract(TQuantity)" />
		public static QuantityVector<TQuantity, TUnit> operator -(QuantityVector<TQuantity, TUnit> left, TQuantity right) => left.Subtract(right);

		/// <inheritdoc cref="Subtract(QuantityVector{TQuantity, TUnit})" />
		public static QuantityVector<TQuantity, TUnit> operator -(QuantityVector<TQuantity, TUnit> left, QuantityVector<TQuantity, TUnit> right) => left.Subtract(right);

		/// <inheritdoc cref="SubtractFrom(TQuantity)" />
		public static QuantityVector<TQuantity, TUnit> operator -(TQuantity left, QuantityVector<TQuantity, TUnit> right) => right.SubtractFrom(left);

		/// <inheritdoc cref="Negate()" />
		public static QuantityVector<TQuantity, TUnit> operator -(QuantityVector<TQuantity, TUnit> vector) => vector.Negate();

		#endregion

	}

	public partial class ForceVector
	{

		#region Operators

		/// <inheritdoc cref="StiffnessMatrix.Tangent" />
		public static StiffnessMatrix operator /(ForceVector forceVector, DisplacementVector displacementVector) => StiffnessMatrix.Tangent(forceVector, displacementVector);

		#endregion

	}
}