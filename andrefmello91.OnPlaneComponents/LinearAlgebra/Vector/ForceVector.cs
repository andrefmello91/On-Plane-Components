﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Force vector class.
	/// </summary>
	/// <remarks>
	///     Unit is <see cref="ForceUnit" />.
	///     <para>
	///         Quantity is <see cref="Force" />.
	///     </para>
	/// </remarks>
	public class ForceVector : QuantityVector<Force, ForceUnit>
	{

		#region Constructors

		/// <inheritdoc />
		/// <remarks>
		///     Default unit is Newton.
		/// </remarks>
		public ForceVector(IEnumerable<double> values, ForceUnit unit = ForceUnit.Newton)
			: base(values, unit)
		{
		}

		/// <inheritdoc />
		public ForceVector(IEnumerable<Force> values)
			: base(values)
		{
		}

		/// <inheritdoc />
		public ForceVector(IEnumerable<PlaneForce> components)
			: base(components.Cast<IPlaneComponent<Force>>())
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Create a force vector with zero elements.
		/// </summary>
		/// <param name="size">The size of the vector.</param>
		public new static ForceVector Zero(int size) => new(new double[size]);

		/// <inheritdoc cref="ICloneable.Clone" />
		public override QuantityVector<Force, ForceUnit> Clone() => new ForceVector(Values, Unit);

		/// <inheritdoc cref="QuantityVector{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		/// <remarks>
		///     This uses the default tolerance.
		/// </remarks>
		public ForceVector Simplified(IEnumerable<int>? indexes = null) => (ForceVector) Simplified(indexes, PlaneForce.Tolerance);

		#endregion

		/*
		#region Operators

		/// <returns>
		///     A new vector with summed components in <paramref name="left" />'s unit.
		/// </returns>
		/// <exception cref="ArgumentException">If left and right don't have the same dimensions.</exception>
		public static ForceVector operator +(ForceVector left, ForceVector right)
		{
			right = right.Unit == left.Unit
				? right
				: (ForceVector) right.Convert(left.Unit);

			var vec = (Vector<double>) left + right;

			return
				new ForceVector(vec, left.Unit);
		}

		/// <returns>
		///     A new vector with subtracted components in <paramref name="left" />'s unit.
		/// </returns>
		/// <exception cref="ArgumentException">If left and right don't have the same dimensions.</exception>
		public static ForceVector operator -(ForceVector left, ForceVector right) 
		{
			right = right.Unit == left.Unit
				? right
				: (ForceVector) right.Convert(left.Unit);

			var vec = (Vector<double>) left - right;

			return
				new ForceVector(vec, left.Unit);
		}

		/// <returns>
		///     A vector with components multiplied by a value
		/// </returns>
		public static ForceVector operator *(double multiplier, ForceVector vector) => new(vector.Values.Select(v => v * multiplier), vector.Unit);

		/// <inheritdoc cref="Matrix{T}.op_Multiply(Matrix{T}, Vector{T})"/>
		public static ForceVector operator *(Matrix<double> left, ForceVector right) => new(left * (Vector<double>) right, right.Unit);
		
		/// <inheritdoc cref="op_Multiply(double, ForceVector) " />
		public static ForceVector operator *(ForceVector vector, double multiplier) => multiplier * vector;

		/// <inheritdoc cref="Vector{T}.op_UnaryNegation" />
		public static ForceVector operator -(ForceVector vector) => new(vector.Values.Select(v => -v), vector.Unit);


		/// <inheritdoc cref="Vector{T}.op_Division(Vector{T}, T)" />
		public static ForceVector operator /(ForceVector vector, double divisor) => new(vector.Values.Select(v => v / divisor), vector.Unit);

		#endregion
		*/
	}
}