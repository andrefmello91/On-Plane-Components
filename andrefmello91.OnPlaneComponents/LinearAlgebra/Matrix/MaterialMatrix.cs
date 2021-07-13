﻿using System;
using System.Collections.Generic;
using andrefmello91.Extensions;
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

		/// <summary>
		///		The rotation angle, related to horizontal (X) axis, of this matrix.
		/// </summary>
		public double Angle { get; private set; }

		/// <summary>
		///     Returns true if <c>Angle = 0</c>.
		/// </summary>
		public bool IsHorizontal => Angle.ApproxZero(1E-6);

		#region Constructors

		/// <inheritdoc />
		/// <param name="angle">The rotation angle, related to horizontal (X) axis, of this matrix.</param>
		public MaterialMatrix(double[,] values, double angle = 0, PressureUnit unit = PressureUnit.Megapascal)
			: base(values, unit)
		{
			Angle = angle;
		}

		/// <inheritdoc />
		/// <param name="angle">The rotation angle, related to horizontal (X) axis, of this matrix.</param>
		public MaterialMatrix(Matrix<double> value, double angle = 0, PressureUnit unit = PressureUnit.Megapascal)
			: base(value, unit)
		{
			Angle = angle;
		}

		/// <inheritdoc />
		/// <param name="angle">The rotation angle, related to horizontal (X) axis, of this matrix.</param>
		public MaterialMatrix(Pressure[,] value, double angle = 0)
			: base(value)
		{
			Angle = angle;
		}

		#endregion

		#region Methods

		/// <summary>
		///     Create a material stiffness matrix with zero elements.
		/// </summary>
		/// <param name="unit">The required unit.</param>
		public new static MaterialMatrix Zero(double angle = 0, PressureUnit unit = PressureUnit.Megapascal) => new(new double[3, 3], angle, unit);
		
		/// <inheritdoc />
		public override QuantityMatrix<Pressure, PressureUnit> Clone() => new MaterialMatrix(ToArray(), Angle, Unit);

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		public MaterialMatrix Simplified() => (MaterialMatrix) Simplified(null, StressState.Tolerance);

		/// <inheritdoc cref="QuantityMatrix{TQuantity,TUnit}.Transform"/>
		/// <param name="angle">The rotation angle. Positive to counterclockwise.</param>
		public MaterialMatrix Transform(double angle)
		{
			if (angle.ApproxZero(1E-6))
				return (MaterialMatrix) Clone();
			
			var matrix = (MaterialMatrix) Transform(StrainRelations.TransformationMatrix(angle));
			matrix.Angle = Angle + angle;

			return matrix;
		}

		/// <summary>
		///		Transform this matrix to horizontal plane.
		/// </summary>
		/// <returns>
		///		A <see cref="MaterialMatrix"/> with <c>Angle = 0</c>.
		/// </returns>
		public MaterialMatrix ToHorizontal() => Transform(-Angle);
		
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

			var sVec = stresses.IsHorizontal
				? stresses.AsVector(Unit)
				: stresses.ToHorizontal().AsVector(Unit);

			var matrix = IsHorizontal
				? this
				: ToHorizontal();
			
			var strain = StrainState.FromVector(matrix.Solve(sVec));

			return stresses.IsHorizontal
				? strain
				: strain.Transform(stresses.ThetaX);
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

			var sVec = strains.IsHorizontal
				? strains.AsVector()
				: strains.ToHorizontal().AsVector();

			var matrix = IsHorizontal
				? this
				: ToHorizontal();

			var stress = StressState.FromVector(matrix * sVec);

			return strains.IsHorizontal
				? stress
				: stress.Transform(strains.ThetaX);
		}

		/// <inheritdoc />
		public override QuantityMatrix<Pressure, PressureUnit> Add(QuantityMatrix<Pressure, PressureUnit> other)
		{
			if (other is not MaterialMatrix matrix)
				throw new ArgumentException("Other matrix is not MaterialMatrix");

			return base.Add(Angle.Approx(matrix.Angle, 1E-6)
				? matrix
				: matrix.Transform(Angle - matrix.Angle));
		}

		/// <inheritdoc />
		public override QuantityMatrix<Pressure, PressureUnit> Subtract(QuantityMatrix<Pressure, PressureUnit> other)
		{
			if (other is not MaterialMatrix matrix)
				throw new ArgumentException("Other matrix is not MaterialMatrix");

			return base.Subtract(Angle.Approx(matrix.Angle, 1E-6)
				? matrix
				: matrix.Transform(Angle - matrix.Angle));
		}

		#region Object override

		/// <inheritdoc cref="Solve(StressState)" />
		public static StrainState operator /(StressState stresses, MaterialMatrix matrix) => matrix.Solve(stresses);

		/// <inheritdoc cref="Solve(StrainState)" />
		public static StressState operator *(MaterialMatrix matrix, StrainState strains) => matrix.Solve(strains);

		#endregion

		#endregion

	}
}