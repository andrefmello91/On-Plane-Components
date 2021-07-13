using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet.Units;

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

	public partial class MaterialMatrix
	{
		#region Object override

		/// <inheritdoc cref="Solve(StressState)" />
		public static StrainState operator /(StressState stresses, MaterialMatrix matrix) => matrix.Solve(stresses);

		/// <inheritdoc cref="Solve(StrainState)" />
		public static StressState operator *(MaterialMatrix matrix, StrainState strains) => matrix.Solve(strains);

		#endregion
	}

	public partial class StiffnessMatrix
	{
		#region Object override

		/// <summary>
		///     Solve the displacement vector by multiplying the inverse of stiffness and the force vector.
		/// </summary>
		/// <remarks>
		///     This uses the simplified stiffness matrix and forces.
		/// </remarks>
		/// <returns>
		///     The <see cref="DisplacementVector" /> with components in <see cref="LengthUnit.Millimeter" />.
		/// </returns>
		public static DisplacementVector operator /(StiffnessMatrix stiffnessMatrix, ForceVector forceVector) => stiffnessMatrix.Solve(forceVector);


		/// <summary>
		///     Solve the force vector by multiplying the stiffness and the displacement vector.
		/// </summary>
		/// <remarks>
		///     This uses the simplified stiffness matrix and displacements.
		/// </remarks>
		/// <returns>
		///     The <see cref="ForceVector" /> with components in <see cref="ForceUnit.Newton" />.
		/// </returns>
		public static ForceVector operator *(StiffnessMatrix stiffnessMatrix, DisplacementVector displacementVector) => stiffnessMatrix.Solve(displacementVector);

		#endregion
	}
}