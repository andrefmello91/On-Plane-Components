using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Material stiffness [D] matrix class.
	/// </summary>
	/// <remarks>
	///     Matrix that relates stress state to strain state:
	///     <para>
	///         <c>{stress} = [D] {strain}</c>
	///     </para>
	///     Matrix must be 3x3.
	/// </remarks>
	public class MaterialMatrix : QuantityMatrix<Pressure, PressureUnit>
	{

		#region Constructors

		/// <inheritdoc />
		public MaterialMatrix(double[,] values, PressureUnit unit = PressureUnit.Megapascal)
			: base(values, unit)
		{
		}

		/// <inheritdoc />
		public MaterialMatrix(Matrix<double> value, PressureUnit unit = PressureUnit.Megapascal)
			: base(value, unit)
		{
		}

		/// <inheritdoc />
		public MaterialMatrix(Pressure[,] value)
			: base(value)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Create a stiffness matrix with zero elements.
		/// </summary>
		/// <param name="size">The size of the matrix.</param>
		public new static MaterialMatrix Zero(int size) => new(new double[size, size]);

		/// <inheritdoc />
		public override QuantityMatrix<Pressure, PressureUnit> Clone() => new MaterialMatrix(ToArray(), Unit);

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		public MaterialMatrix Simplified() => (MaterialMatrix) Simplified(null, StressState.Tolerance);

		/// <summary>
		///     Solve the strain state related to a stress state.
		/// </summary>
		/// <param name="stresses">The stress state.</param>
		/// <returns>
		///     <see cref="StrainState" />.
		/// </returns>
		public StrainState Solve(StressState stresses)
		{
			if (stresses.IsZero)
				return StrainState.Zero;

			var strain = Solve(stresses.ToHorizontal().AsVector(Unit));

			return
				StrainState.FromVector(strain)
					.Transform(stresses.ThetaX);
		}

		/// <summary>
		///     Solve the stress state related to a strain state.
		/// </summary>
		/// <param name="strains">The strain state.</param>
		/// <returns>
		///     <see cref="StressState" />, with the same unit of this matrix.
		/// </returns>
		public StressState Solve(StrainState strains)
		{
			if (strains.IsZero)
				return StressState.Zero;

			var stress = this * strains.ToHorizontal().AsVector();

			return
				StressState.FromVector(stress, unit: Unit)
					.Transform(strains.ThetaX);
		}

		#region Object override

		/// <inheritdoc cref="Solve(StressState)" />
		public static StrainState operator /(MaterialMatrix matrix, StressState stresses) => matrix.Solve(stresses);

		/// <inheritdoc cref="Solve(StrainState)" />
		public static StressState operator *(MaterialMatrix matrix, StrainState strains) => matrix.Solve(strains);

		#endregion

		#endregion

	}
}