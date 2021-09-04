using System.Collections.Generic;
using andrefmello91.Extensions;
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
	public partial class StiffnessMatrix : QuantityMatrix<ForcePerLength, ForcePerLengthUnit>
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
		///     Calculate the tangent matrix from a force vector and a displacement vector.
		/// </summary>
		/// <param name="forceVector">The known force vector.</param>
		/// <param name="displacementVector">The known displacement vector.</param>
		/// <returns>
		///     The tangent <see cref="StiffnessMatrix" />.
		/// </returns>
		public static StiffnessMatrix Tangent(ForceVector forceVector, DisplacementVector displacementVector)
		{
			// Get unit
			var unit = forceVector.Unit.Per(displacementVector.Unit);

			// Do conversions if needed
			var fVec = unit is ForcePerLengthUnit.Undefined
				? forceVector.Convert(ForceUnit.Newton)
				: forceVector;

			var dVec = unit is ForcePerLengthUnit.Undefined
				? displacementVector.Convert(LengthUnit.Millimeter)
				: displacementVector;

			unit = unit is ForcePerLengthUnit.Undefined
				? ForcePerLengthUnit.NewtonPerMillimeter
				: unit;

			var matrix = fVec.ToColumnMatrix() * dVec.ToRowMatrix();

			return
				new StiffnessMatrix(matrix, unit);
		}

		/// <summary>
		///     Create a stiffness matrix with zero elements.
		/// </summary>
		/// <param name="size">The size of the matrix.</param>
		public new static StiffnessMatrix Zero(int size, ForcePerLengthUnit unit = ForcePerLengthUnit.NewtonPerMillimeter) => new(new double[size, size], unit);

		/// <inheritdoc />
		public override QuantityMatrix<ForcePerLength, ForcePerLengthUnit> BuildSame(int rows, int columns) =>
			new StiffnessMatrix(new double[rows, columns], Unit);

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}.Clone" />
		public override QuantityMatrix<ForcePerLength, ForcePerLengthUnit> Clone() => new StiffnessMatrix(ToArray(), Unit)
		{
			ConstraintIndex = ConstraintIndex
		};

		/// <inheritdoc />
		public override bool Equals(QuantityMatrix<ForcePerLength, ForcePerLengthUnit>? other) => Approaches(other, Tolerance);

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
		/// <remarks>
		///     <c>{F} = [K] {u}</c>
		/// </remarks>
		/// <returns>
		///     The resulting displacement vector.
		/// </returns>
		public DisplacementVector Solve(ForceVector forceVector, bool useSimplified = true)
		{
			var lenghtUnit = Unit.GetLenghtUnit();
			var forceUnit  = Unit.GetForceUnit();

			if (lenghtUnit == LengthUnit.Undefined)
				lenghtUnit = LengthUnit.Millimeter;

			if (forceUnit == ForceUnit.Undefined)
				forceUnit = ForceUnit.Newton;

			var unit = forceUnit.Per(lenghtUnit);

			var stiffness = Unit == unit
				? this
				: (StiffnessMatrix) Convert(unit);

			var forces = forceVector.Unit == forceUnit
				? forceVector
				: (ForceVector) forceVector.Convert(forceUnit);

			Matrix<double> k = useSimplified
				? stiffness.Simplified()
				: stiffness;

			Vector<double> f = useSimplified
				? forces.Simplified(ConstraintIndex)
				: forces;

			// Solve
			var d = k.Solve(f);

			return
				new DisplacementVector(d, lenghtUnit);
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
		public ForceVector Solve(DisplacementVector displacementVector, bool useSimplified = true)
		{
			var lenghtUnit = Unit.GetLenghtUnit();
			var forceUnit  = Unit.GetForceUnit();

			if (lenghtUnit == LengthUnit.Undefined)
				lenghtUnit = LengthUnit.Millimeter;

			if (forceUnit == ForceUnit.Undefined)
				forceUnit = ForceUnit.Newton;

			var unit = forceUnit.Per(lenghtUnit);

			var stiffness = Unit == unit
				? this
				: (StiffnessMatrix) Convert(unit);

			// Convert
			var displacements = displacementVector.Unit == lenghtUnit
				? displacementVector
				: (DisplacementVector) displacementVector.Convert(lenghtUnit);

			Matrix<double> k = useSimplified
				? stiffness.Simplified()
				: stiffness;

			Vector<double> d = useSimplified
				? displacements.Simplified(ConstraintIndex)
				: displacements;

			// Solve
			var f = k * d;

			return
				new ForceVector(f, forceUnit);
		}

		/// <inheritdoc />
		public override QuantityMatrix<ForcePerLength, ForcePerLengthUnit> Transform(Matrix<double> transformationMatrix)
		{
			var value = (StiffnessMatrix) base.Transform(transformationMatrix);

			value.ConstraintIndex = ConstraintIndex;

			return value;
		}

		#endregion

	}
}