using System;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// UnitConvertible interface.
    /// </summary>
    /// <typeparam name="T1">
    ///     The type that represents the object.
    /// </typeparam>
    /// <typeparam name="T2">
    ///     The struct that represents the value of the object.
    ///     <para>
    ///         <see cref="double"/>, <see cref="Length"/>, <see cref="Force"/>, <see cref="Pressure"/>, etc.
    ///     </para>
    /// </typeparam>
    /// <typeparam name="T3">
    ///     The enum of unit that represents the object.
    ///     <para>
    ///         <see cref="LengthUnit"/>, <see cref="ForceUnit"/>, <see cref="PressureUnit"/>, etc.
    ///     </para>
    /// </typeparam>
    public interface IUnitConvertible<T1, in T2, T3> : IApproximated<T1, T2>
		where T2 : struct
		where T3 : Enum
    {
        /// <summary>
        /// Get/set the unit of this object.
        /// </summary>
        T3 Unit { get; set; }

        /// <summary>
        /// Change the unit of this object.
        /// </summary>
        /// <param name="unit">The unit to convert.</param>
        void ChangeUnit(T3 unit);

        /// <summary>
        /// Convert this object to another unit.
        /// </summary>
        /// <inheritdoc cref="ChangeUnit"/>
        T1 Convert(T3 unit);
    }

    /// <summary>
    /// Approximated interface
    /// </summary>
    /// <typeparam name="T1">The type that represents the object.</typeparam>
    /// <typeparam name="T2">The type that represents the tolerance to compare objects.</typeparam>
    public interface IApproximated<T1, in T2> : IEquatable<T1>
    {
        /// <summary>
        /// Returns true if this object is approximately equivalent to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <param name="tolerance">The tolerance to consider this object approximately equivalent to <paramref name="other"/>.</param>
	    bool Approx(T1 other, T2 tolerance);
    }

    /// <summary>
    /// Copyable interface.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public interface ICopyable<out T>
    {
	    /// <summary>
	    /// Create a copy of this object.
	    /// </summary>
	    T Copy();
    }
}
