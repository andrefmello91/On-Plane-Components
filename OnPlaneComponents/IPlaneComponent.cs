using System;

namespace OnPlaneComponents
{
    public interface IPlaneComponent<T1,T2> : IEquatable<IPlaneComponent<T1,T2>>
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
        /// <param name="unit">The <seealso cref="T2"/> to convert.</param>
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
