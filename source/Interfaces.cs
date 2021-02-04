using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace OnPlaneComponents
{
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

        /// <summary>
        ///     Get this state as an array.
        /// </summary>
        T[] AsArray();

	    /// <summary>
        ///     Get this state transformed to horizontal direction (<see cref="ThetaX" /> = 0).
        /// </summary>
        IState<T> ToHorizontal();

        /// <summary>
        ///     Rotate this state by a <paramref name="rotationAngle"/>.
        /// </summary>
        /// <remarks>
        ///     <paramref name="rotationAngle"/> is positive if counterclockwise.
        /// </remarks>
        /// <param name="rotationAngle">The rotation angle in radians.</param>
        IState<T> Transform(double rotationAngle);

        /// <summary>
        ///     Transform this state into a principal state.
        /// </summary>
        IPrincipalState<T> ToPrincipal();
    }

    /// <summary>
    /// Interface to principal strain/stress states.
    /// </summary>
    /// <typeparam name="T">The struct that represents the values of the object's components.</typeparam>
    public interface IPrincipalState<T> : IState<T>
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
        IState<T> AsState();
    }
}
