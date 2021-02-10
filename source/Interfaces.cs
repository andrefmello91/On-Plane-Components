using System;
using Extensions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace OnPlaneComponents
{
    /// <summary>
    /// Plane component interface.
    /// </summary>
    /// <typeparam name="T1">Any type that implements <see cref="IPlaneComponent{T1,T2}."/></typeparam>
    /// <typeparam name="T2">The struct that represents the values of the object's components.</typeparam>
    public interface IPlaneComponent<T1, T2> : IApproachable<T1, T2>, ICloneable<T1>, IEquatable<T1>
        where T2 : struct
    {
        /// <summary>
        ///     Get the component in X (horizontal) direction.
        /// </summary>
        T2 X { get; }

        /// <summary>
        ///     Get the component in Y (vertical) direction.
        /// </summary>
        T2 Y { get; }

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
    /// <typeparam name="T1">Any type that implements <see cref="IState{T1,T2,T3}"/>.</typeparam>
    /// <typeparam name="T2">Any type that implements <see cref="IPrincipalState{T1,T2,T3}"/>.</typeparam>
    /// <typeparam name="T3">The struct that represents the values of the object's components.</typeparam>
    public interface IState<T1, T2, T3> : IApproachable<T1, T2, T3>, IEquatable<T1, T2>
		where T1 : IState<T1, T2, T3>
		where T2 : IPrincipalState<T2, T1, T3>
		where T3 : struct
    {
	    /// <summary>
	    ///     Get the normal component in X direction.
	    /// </summary>
	    T3 X  { get; }

	    /// <summary>
	    ///     Get the normal component in Y direction.
	    /// </summary>
	    T3 Y  { get; }

	    /// <summary>
	    ///     Get the shear component.
	    /// </summary>
	    T3 XY { get; }

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

        /// <summary>
        ///     Get this state as an array.
        /// </summary>
        T3[] AsArray();

	    /// <summary>
        ///     Get this state transformed to horizontal direction (<see cref="ThetaX" /> = 0).
        /// </summary>
        T1 ToHorizontal();

        /// <summary>
        ///     Rotate this state by a <paramref name="rotationAngle"/>.
        /// </summary>
        /// <remarks>
        ///     <paramref name="rotationAngle"/> is positive if counterclockwise.
        /// </remarks>
        /// <param name="rotationAngle">The rotation angle in radians.</param>
        T1 Transform(double rotationAngle);

        /// <summary>
        ///     Transform this state into a principal state.
        /// </summary>
        T2 ToPrincipal();
    }

    /// <summary>
    /// Interface to principal strain/stress states.
    /// </summary>
    /// <typeparam name="T1">Any type that implements <see cref="IPrincipalState{T1,T2,T3}"/>.</typeparam>
    /// <typeparam name="T2">Any type that implements <see cref="IState{T3,T2,T1}"/>.</typeparam>
    /// <typeparam name="T3">The struct that represents the values of the object's components.</typeparam>
    public interface IPrincipalState<T1, T2, T3> : IState<T2, T1, T3>
        where T1 : IPrincipalState<T1, T2, T3>
	    where T3 : struct
	    where T2 : IState<T2, T1, T3>
    {
        /// <summary>
        ///     Get the <see cref="PrincipalCase" /> of this state.
        /// </summary>
        PrincipalCase Case { get; }

        /// <summary>
        ///     Get the maximum component.
        /// </summary>
        T3 S1  { get; }

        /// <summary>
        ///     Get the minimum component.
	    /// </summary>
        T3 S2  { get; }

        /// <summary>
        ///     Returns true if maximum component is nearly zero.
        /// </summary>
        bool Is1Zero { get; }

        /// <summary>
        ///     Returns true if minimum component is nearly zero.
        /// </summary>
        bool Is2Zero { get; }

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
        ///     Get this principal state as a state.
        /// </summary>
        T2 AsState();
    }
}
