using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace OnPlaneComponents
{
    /// <summary>
    /// UnitConvertible interface.
    /// </summary>
    /// <typeparam name="T1">
    ///     The struct that represents the value of the object.
    ///     <para>
    ///         <see cref="double"/>, <see cref="Length"/>, <see cref="Force"/>, <see cref="Pressure"/>, etc.
    ///     </para>
    /// </typeparam>
    /// <typeparam name="T2">
    ///     The enum of unit that represents the object.
    ///     <para>
    ///         <see cref="LengthUnit"/>, <see cref="ForceUnit"/>, <see cref="PressureUnit"/>, etc.
    ///     </para>
    /// </typeparam>
    public interface IUnitConvertible<out T1, T2>
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
    }

    /// <summary>
    /// Approachable interface
    /// </summary>
    /// <typeparam name="T1">The type that represents the object.</typeparam>
    /// <typeparam name="T2">The type that represents the tolerance to compare objects.</typeparam>
    public interface IApproachable<in T1, in T2>
    {
        /// <summary>
        /// Returns true if this object is approximately equivalent to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <param name="tolerance">The tolerance to consider this object approximately equivalent to <paramref name="other"/>.</param>
	    bool Approaches(T1 other, T2 tolerance);
    }

    /// <summary>
    /// Cloneable interface with generic type.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public interface ICloneable<out T>
    {
	    /// <inheritdoc cref="ICloneable.Clone"/>
	    T Clone();
    }

    /// <summary>
    /// Plane component interface.
    /// </summary>
    /// <typeparam name="T">The struct that represents the values of the object's components.</typeparam>
    public interface IPlaneComponent<out T>
	    where T : struct
    {
        /// <summary>
        ///     Get the component in X (horizontal) direction.
        /// </summary>
        T X { get; }

        /// <summary>
        ///     Get the component in Y (vertical) direction.
        /// </summary>
        T Y { get; }

        /// <summary>
        ///     Returns true if X component is nearly zero.
        /// </summary>
        bool IsXZero { get; }

        /// <summary>
        ///     Returns true if Y component is nearly zero.
        /// </summary>
        bool IsYZero { get; }

        /// <summary>
        ///     Returns true if this is nearly zero.
        /// </summary>
        bool IsZero  { get; }
    }

    /// <summary>
    /// Interface to strain/stress states.
    /// </summary>
    /// <typeparam name="T">The struct that represents the values of the object's components.</typeparam>
    public interface IState<T>
	    where T : struct
    {
	    /// <summary>
	    ///     Get the normal component in X direction.
	    /// </summary>
	    T X  { get; }

	    /// <summary>
	    ///     Get the normal component in Y direction.
	    /// </summary>
	    T Y  { get; }

	    /// <summary>
	    ///     Get the shear component.
	    /// </summary>
	    T XY { get; }

        /// <summary>
        ///     Returns true if normal component in X is nearly zero.
        /// </summary>
        bool IsXZero { get; }

        /// <summary>
        ///     Returns true if normal component in Y is nearly zero.
        /// </summary>
        bool IsYZero { get; }

        /// <summary>
        ///     Returns true if shear component is nearly zero.
        /// </summary>
        bool IsXYZero { get; }

        /// <summary>
        ///     Returns true if this is nearly zero.
        /// </summary>
        bool IsZero { get; }

        /// <summary>
        ///     Returns true if X direction coincides to horizontal axis.
        /// </summary>
        bool IsHorizontal { get; }

        /// <summary>
        ///     Returns true if X direction coincides to vertical axis.
        /// </summary>
        bool IsVertical { get; }

        /// <summary>
        ///     Returns true if this is principal state.
        /// </summary>
        bool IsPrincipal { get; }

        /// <summary>
        ///     Returns true if this is a pure shear state.
        /// </summary>
        bool IsPureShear { get; }

        /// <summary>
        ///     Get the angle of X direction, related to horizontal axis.
        /// </summary>
        double ThetaX { get; }

        /// <summary>
        ///     Get the angle of Y direction, related to horizontal axis.
        /// </summary>
        double ThetaY { get; }

        /// <summary>
        ///     Get transformation <see cref="Matrix"/> to transform this state from horizontal plane to XY plane.
        /// </summary>
        /// <remarks>
        ///     See: <seealso cref="StrainRelations.TransformationMatrix"/>.
        /// </remarks>
        Matrix<double> TransformationMatrix { get; }
    }

    /// <summary>
    /// Interface to principal strain/stress states.
    /// </summary>
    /// <typeparam name="T">The struct that represents the values of the object's components.</typeparam>
    public interface IPrincipalState<T>
	    where T : struct
    {
	    /// <summary>
	    ///     Get the <see cref="PrincipalCase" /> of this state.
	    /// </summary>
	    PrincipalCase Case { get; }

        /// <summary>
        ///     Get the maximum component.
        /// </summary>
        T T1  { get; }

        /// <summary>
        ///     Get the minimum component.
	    /// </summary>
        T T2  { get; }

        /// <summary>
        ///     Returns true if maximum component is nearly zero.
        /// </summary>
        bool Is1Zero { get; }

        /// <summary>
        ///     Returns true if minimum component is nearly zero.
        /// </summary>
        bool Is2Zero { get; }

        /// <summary>
        ///     Returns true if this is nearly zero.
        /// </summary>
        bool IsZero { get; }

        /// <summary>
        ///     Returns true if maximum component direction coincides to horizontal axis.
        /// </summary>
        bool IsHorizontal { get; }

        /// <summary>
        ///     Returns true if maximum component direction coincides to vertical axis.
        /// </summary>
        bool IsVertical { get; }

        /// <summary>
        ///     Returns true if maximum component direction is at an angle of 45 degrees, related to horizontal axis.
        /// </summary>
        bool IsAt45Degrees { get; }

        /// <summary>
        ///     Get the angle of maximum component direction, related to horizontal axis.
        /// </summary>
        double Theta1 { get; }

        /// <summary>
        ///     Get the angle of minimum component direction, related to horizontal axis.
        /// </summary>
        double Theta2 { get; }

        /// <summary>
        ///     Get transformation <see cref="Matrix"/> to transform this state from horizontal plane to principal plane.
        /// </summary>
        /// <remarks>
        ///     See: <seealso cref="StrainRelations.TransformationMatrix"/>.
        /// </remarks>
        Matrix<double> TransformationMatrix { get; }
    }
}
