using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;

namespace andrefmello91.OnPlaneComponents
{
	public partial class QuantityMatrix<TQuantity, TUnit>
	{

		#region Operators

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

		/// <inheritdoc cref="RightMultiply" />
		public static QuantityMatrix<TQuantity, TUnit> operator *(QuantityMatrix<TQuantity, TUnit> matrix, Matrix<double> multiplier) => matrix.RightMultiply(multiplier);

		/// <inheritdoc cref="LeftMultiply" />
		public static QuantityMatrix<TQuantity, TUnit> operator *(Matrix<double> multiplier, QuantityMatrix<TQuantity, TUnit> matrix) => matrix.LeftMultiply(multiplier);

		/// <inheritdoc cref="Subtract(TQuantity)" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(QuantityMatrix<TQuantity, TUnit> left, TQuantity right) => left.Subtract(right);

		/// <inheritdoc cref="Subtract(QuantityMatrix{TQuantity, TUnit})" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(QuantityMatrix<TQuantity, TUnit> left, QuantityMatrix<TQuantity, TUnit> right) => left.Subtract(right);

		/// <inheritdoc cref="SubtractFrom(TQuantity)" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(TQuantity left, QuantityMatrix<TQuantity, TUnit> right) => right.SubtractFrom(left);

		/// <inheritdoc cref="Negate" />
		public static QuantityMatrix<TQuantity, TUnit> operator -(QuantityMatrix<TQuantity, TUnit> right) => right.Negate();

		#endregion

	}

	public partial class MaterialMatrix
	{

		#region Operators

		/// <inheritdoc cref="Solve(StressState)" />
		public static StrainState operator /(StressState stresses, MaterialMatrix matrix) => matrix.Solve(stresses);

		/// <inheritdoc cref="Solve(StrainState)" />
		public static StressState operator *(MaterialMatrix matrix, StrainState strains) => matrix.Solve(strains);

		#endregion

	}

	public partial class StiffnessMatrix
	{

		#region Operators

		/// <summary>
		///     Solve the displacement vector by multiplying the inverse of stiffness and the force vector.
		/// </summary>
		/// <remarks>
		///     This uses the simplified stiffness matrix and forces.
		/// </remarks>
		/// <returns>
		///     The resultant <see cref="DisplacementVector" />.
		/// </returns>
		public static DisplacementVector operator /(ForceVector forceVector, StiffnessMatrix stiffnessMatrix) => stiffnessMatrix.Solve(forceVector);


		/// <summary>
		///     Solve the force vector by multiplying the stiffness and the displacement vector.
		/// </summary>
		/// <remarks>
		///     This uses the simplified stiffness matrix and displacements.
		/// </remarks>
		/// <returns>
		///     The resultant <see cref="ForceVector" />.
		/// </returns>
		public static ForceVector operator *(StiffnessMatrix stiffnessMatrix, DisplacementVector displacementVector) => stiffnessMatrix.Solve(displacementVector);

		#endregion

	}
}