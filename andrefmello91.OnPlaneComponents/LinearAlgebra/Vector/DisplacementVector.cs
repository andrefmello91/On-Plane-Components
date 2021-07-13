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
		/// <param name="unit">The unit.</param>
		public new static DisplacementVector Zero(int size, LengthUnit unit = LengthUnit.Millimeter) => new(new double[size], unit);

		/// <inheritdoc cref="ICloneable.Clone" />
		public override QuantityVector<Length, LengthUnit> Clone() => new DisplacementVector(Values, Unit);

		/// <inheritdoc cref="QuantityVector{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		/// <remarks>
		///     This uses the default tolerance.
		/// </remarks>
		public DisplacementVector Simplified(IEnumerable<int>? indexes = null) => (DisplacementVector) Simplified(indexes, PlaneDisplacement.Tolerance);

		/// <inheritdoc />
		public override bool Equals(QuantityVector<Length, LengthUnit>? other) => Approaches(other, PlaneDisplacement.Tolerance);

		#endregion
	}
}