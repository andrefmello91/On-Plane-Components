using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Displacement vector class.
	/// </summary>
	/// <remarks>
	///     Unit is <see cref="LengthUnit" />.
	///     <para>
	///         Quantity is <see cref="Length" />.
	///     </para>
	/// </remarks>
	public class DisplacementVector : QuantityVector<Length, LengthUnit>
	{

		#region Constructors

		/// <inheritdoc />
		public DisplacementVector(IEnumerable<double> values, LengthUnit unit = LengthUnit.Millimeter)
			: base(values, unit)
		{
		}

		/// <inheritdoc />
		public DisplacementVector(IEnumerable<Length> values)
			: base(values)
		{
		}

		/// <inheritdoc />
		public DisplacementVector(IEnumerable<PlaneDisplacement> components)
			: base(components.Cast<IPlaneComponent<Length>>())
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Create a displacement vector with zero elements.
		/// </summary>
		/// <param name="size">The size of the vector.</param>
		public new static DisplacementVector Zero(int size) => new(new double[size]);

		/// <inheritdoc cref="ICloneable.Clone" />
		public override QuantityVector<Length, LengthUnit> Clone() => new DisplacementVector(Values, Unit);

		/// <inheritdoc cref="QuantityVector{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		/// <remarks>
		///     This uses the default tolerance.
		/// </remarks>
		public DisplacementVector Simplified(IEnumerable<int>? indexes = null) => (DisplacementVector) Simplified(indexes, PlaneDisplacement.Tolerance);

		#endregion

		/*
		#region Object override

		/// <inheritdoc cref="ForceVector.op_Addition" />
		public static DisplacementVector operator +(DisplacementVector left, DisplacementVector right)
		{
			right = right.Unit == left.Unit
				? right
				: (DisplacementVector) right.Convert(left.Unit);

			var vec = (Vector<double>) left + right;

			return
				new DisplacementVector(vec, left.Unit);
		}

		/// <inheritdoc cref="Vector{T}.op_Division(Vector{T}, T)" />
		public static DisplacementVector operator /(DisplacementVector vector, double divisor) => new(vector.Values.Select(v => v / divisor), vector.Unit);

		/// <inheritdoc cref="ForceVector.op_Multiply(double,ForceVector) " />
		public static DisplacementVector operator *(double multiplier, DisplacementVector vector) =>
			new(vector.Values.Select(v => v * multiplier), vector.Unit);

		/// <inheritdoc cref="ForceVector.op_Multiply(double,ForceVector) " />
		public static DisplacementVector operator *(DisplacementVector vector, double multiplier) => multiplier * vector;

		/// <inheritdoc cref="Matrix{T}.op_Multiply(Matrix{T}, Vector{T})" />
		public static DisplacementVector operator *(Matrix<double> left, DisplacementVector right) => new(left * (Vector<double>) right, right.Unit);

		/// <inheritdoc cref="ForceVector.op_Subtraction" />
		public static DisplacementVector operator -(DisplacementVector left, DisplacementVector right)
		{
			right = right.Unit == left.Unit
				? right
				: (DisplacementVector) right.Convert(left.Unit);

			var vec = (Vector<double>) left - right;

			return
				new DisplacementVector(vec, left.Unit);
		}

		/// <inheritdoc cref="Vector{T}.op_UnaryNegation" />
		public static DisplacementVector operator -(DisplacementVector vector) => new(vector.Values.Select(v => -v), vector.Unit);

		#endregion
		*/
	}
}