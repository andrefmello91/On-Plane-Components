using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents
{

	/// <summary>
	///     Default stiffness matrix class.
	/// </summary>
	/// <remarks>
	///     Matrix that relates forces to displacements:
	///     <para>
	///         <c>{F} = [K] {u}</c>
	///     </para>
	/// </remarks>
	public class StiffnessMatrix : QuantityMatrix<ForcePerLength, ForcePerLengthUnit>
	{

		#region Properties

		/// <summary>
		///     Default tolerance for stiffness matrix.
		/// </summary>
		private static ForcePerLength Tolerance { get; } = ForcePerLength.FromNewtonsPerMillimeter(1E-6);

		/// <summary>
		///     The index of constrained DoFs.
		/// </summary>
		public List<int>? ConstraintIndex { get; set; }

		#endregion

		#region Constructors

		/// <inheritdoc cref="StiffnessMatrix(Matrix{double}, ForcePerLengthUnit)" />
		public StiffnessMatrix(double[,] values, ForcePerLengthUnit unit = ForcePerLengthUnit.NewtonPerMillimeter)
			: base(values, unit)
		{
		}

		/// <inheritdoc cref="StiffnessMatrix(Matrix{double}, ForcePerLengthUnit)" />
		public StiffnessMatrix(ForcePerLength[,] values)
			: base(values)
		{
		}

		/// <summary>
		///     Create a stiffness matrix.
		/// </summary>
		/// <param name="value">The <see cref="Matrix{T}" /> or array value.</param>
		/// <param name="unit">The unit of <paramref name="value" />'s components</param>
		public StiffnessMatrix(Matrix<double> value, ForcePerLengthUnit unit = ForcePerLengthUnit.NewtonPerMillimeter)
			: base(value, unit)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Create a stiffness matrix with zero elements.
		/// </summary>
		/// <param name="size">The size of the matrix.</param>
		public new static StiffnessMatrix Zero(int size) => new(new double[size, size]);

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}.Clone" />
		public override QuantityMatrix<ForcePerLength, ForcePerLengthUnit> Clone() => new StiffnessMatrix(ToArray(), Unit)
		{
			ConstraintIndex = ConstraintIndex
		};

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		public StiffnessMatrix Simplified() => (StiffnessMatrix) Simplified(ConstraintIndex, Tolerance);

		/// <inheritdoc cref="Matrix{T}.Solve(Vector{T})" />
		public new Vector<double> Solve(Vector<double> input) => input switch
		{
			ForceVector forceVector               => Solve(forceVector),
			DisplacementVector displacementVector => Solve(displacementVector),
			_                                     => base.Solve(input)
		};

		/// <summary>
		///     Solve the displacement vector from a force vector.
		/// </summary>
		/// <param name="forceVector">The force vector.</param>
		/// <param name="useSimplified">Use simplified matrix and vector?</param>
		/// <param name="unit">The unit to return.</param>
		/// <remarks>
		///     <c>{F} = [K] {u}</c>
		/// </remarks>
		/// <returns>
		///     The resulting displacement vector.
		/// </returns>
		public DisplacementVector Solve(ForceVector forceVector, bool useSimplified = true, LengthUnit unit = LengthUnit.Millimeter)
		{
			// Convert
			var stiffnessMatrix = Unit is ForcePerLengthUnit.NewtonPerMillimeter
				? this
				: (StiffnessMatrix) Convert(ForcePerLengthUnit.NewtonPerMillimeter);

			var forces = forceVector.Unit is ForceUnit.Newton
				? forceVector
				: (ForceVector) forceVector.Convert(ForceUnit.Newton);

			var k = useSimplified
				? stiffnessMatrix.Simplified()
				: stiffnessMatrix;

			var f = useSimplified
				? forces.Simplified(ConstraintIndex)
				: forces;

			// Solve
			var d  = k.Solve(f);
			var dv = new DisplacementVector(d);

			return
				unit is LengthUnit.Millimeter
					? dv
					: (DisplacementVector) dv.Convert(unit);
		}

		/// <summary>
		///     Solve the force vector from a displacement vector.
		/// </summary>
		/// <param name="displacementVector">The displacement vector.</param>
		/// <param name="useSimplified">Use simplified matrix and vector?</param>
		/// <remarks>
		///     <c>{F} = [K] {u}</c>
		/// </remarks>
		/// <returns>
		///     The resulting force vector.
		/// </returns>
		public ForceVector Solve(DisplacementVector displacementVector, bool useSimplified = true, ForceUnit unit = ForceUnit.Newton)
		{
			// Convert
			var stiffnessMatrix = Unit is ForcePerLengthUnit.NewtonPerMillimeter
				? this
				: (StiffnessMatrix) Convert(ForcePerLengthUnit.NewtonPerMillimeter);

			var displacements = displacementVector.Unit is LengthUnit.Millimeter
				? displacementVector
				: (DisplacementVector) displacementVector.Convert(LengthUnit.Millimeter);

			Matrix<double> k = useSimplified
				? stiffnessMatrix.Simplified()
				: stiffnessMatrix;

			Vector<double> d = useSimplified
				? displacements.Simplified(ConstraintIndex)
				: displacements;

			// Solve
			var f  = k * d;
			var fv = new ForceVector(f);

			return
				unit is ForceUnit.Newton
					? fv
					: (ForceVector) fv.Convert(unit);
		}

		/// <inheritdoc />
		public override QuantityMatrix<ForcePerLength, ForcePerLengthUnit> Transform(Matrix<double> transformationMatrix)
		{
			var value = (StiffnessMatrix) base.Transform(transformationMatrix);

			value.ConstraintIndex = ConstraintIndex;

			return value;
		}

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

		#endregion

	}
}