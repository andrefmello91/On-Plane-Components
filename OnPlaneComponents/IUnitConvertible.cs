using System;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// UnitConvertible interface.
    /// </summary>
    /// <typeparam name="T1">The struct that represents the value of the object.</typeparam>
    /// <typeparam name="T2">
    /// The enum of unit that represents the object.
    /// <para><see cref="LengthUnit"/>, <see cref="ForceUnit"/>, <see cref="PressureUnit"/>, etc.</para>
    /// </typeparam>
    public interface IUnitConvertible<out T1,T2>
		where T1 : struct
		where T2 : Enum
    {
        /// <summary>
        /// Get/set the unit of this object.
        /// </summary>
        T2 Unit { get; set; }

        /// <summary>
        /// Change the unit of this object.
        /// </summary>
        /// <param name="unit">The unit to convert.</param>
        void ChangeUnit(T2 unit);

        /// <summary>
        /// Convert this object to another unit.
        /// </summary>
        /// <inheritdoc cref="ChangeUnit"/>
        T1 Convert(T2 unit);

        /// <summary>
        /// Create a copy of this object.
        /// </summary>
        T1 Copy();
    }
}
