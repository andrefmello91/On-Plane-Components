using System;
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
	public partial class ForceVector : QuantityVector<Force, ForceUnit>
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
		/// <param name="unit">The unit.</param>
		public new static ForceVector Zero(int size, ForceUnit unit = ForceUnit.Newton) => new(new double[size], unit);

		/// <inheritdoc />
		public override QuantityVector<Force, ForceUnit> BuildSame(int count) => Zero(count, Unit);

		/// <inheritdoc cref="ICloneable.Clone" />
		public override QuantityVector<Force, ForceUnit> Clone() => new ForceVector(Values, Unit);

		/// <inheritdoc />
		public override bool Equals(QuantityVector<Force, ForceUnit>? other) => Approaches(other, PlaneForce.Tolerance);

		/// <inheritdoc cref="QuantityVector{TQuantity,TUnit}.Simplified(IEnumerable{int}?, double?)" />
		/// <remarks>
		///     This uses the default tolerance.
		/// </remarks>
		public ForceVector Simplified(IEnumerable<int>? indexes = null) => (ForceVector) Simplified(indexes, PlaneForce.Tolerance);

		#endregion

	}
}