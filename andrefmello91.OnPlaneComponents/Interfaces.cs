using andrefmello91.Extensions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace andrefmello91.OnPlaneComponents;

/// <summary>
///     Plane component interface.
/// </summary>
/// <typeparam name="TStruct">The struct that represents the values of the object's components.</typeparam>
public interface IPlaneComponent<TStruct> : IVectorTransformable
	where TStruct : struct
{

	#region Properties

	/// <summary>
	///     Get the direction of this component;
	/// </summary>
	ComponentDirection Direction { get; }

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
	bool IsZero { get; }

	/// <summary>
	///     Get the component in X (horizontal) direction.
	/// </summary>
	TStruct X { get; }

	/// <summary>
	///     Get the component in Y (vertical) direction.
	/// </summary>
	TStruct Y { get; }

	#endregion

}

/// <summary>
///     Interface to strain/stress states.
/// </summary>
/// <typeparam name="TStruct">The struct that represents the values of the object's components.</typeparam>
public interface IState<TStruct> : IVectorTransformable
	where TStruct : struct
{

	#region Properties

	/// <summary>
	///     Returns true if X direction coincides to horizontal axis.
	/// </summary>
	bool IsHorizontal { get; }

	/// <summary>
	///     Returns true if this is principal state.
	/// </summary>
	bool IsPrincipal { get; }

	/// <summary>
	///     Returns true if this is a pure shear state.
	/// </summary>
	bool IsPureShear { get; }

	/// <summary>
	///     Returns true if X direction coincides to vertical axis.
	/// </summary>
	bool IsVertical { get; }

	/// <summary>
	///     Returns true if shear component is nearly zero.
	/// </summary>
	bool IsXYZero { get; }

	/// <summary>
	///     Returns true if normal component in X is nearly zero.
	/// </summary>
	bool IsXZero { get; }

	/// <summary>
	///     Returns true if normal component in Y is nearly zero.
	/// </summary>
	bool IsYZero { get; }

	/// <summary>
	///     Returns true if this is nearly zero.
	/// </summary>
	bool IsZero { get; }

	/// <summary>
	///     Get the angle of X direction, related to horizontal axis.
	/// </summary>
	double ThetaX { get; }

	/// <summary>
	///     Get the angle of Y direction, related to horizontal axis.
	/// </summary>
	double ThetaY { get; }

	/// <summary>
	///     Get transformation <see cref="Matrix" /> to transform this state from horizontal plane to XY plane.
	/// </summary>
	/// <remarks>
	///     See: <seealso cref="StrainRelations.TransformationMatrix" />.
	/// </remarks>
	Matrix<double> TransformationMatrix { get; }

	/// <summary>
	///     Get the normal component in X direction.
	/// </summary>
	TStruct X { get; }

	/// <summary>
	///     Get the shear component.
	/// </summary>
	TStruct XY { get; }

	/// <summary>
	///     Get the normal component in Y direction.
	/// </summary>
	TStruct Y { get; }

	#endregion

	#region Methods

	/// <summary>
	///     Get this state as an array.
	/// </summary>
	TStruct[] AsArray();

	/// <summary>
	///     Get this state transformed to horizontal direction (<see cref="ThetaX" /> = 0).
	/// </summary>
	IState<TStruct> ToHorizontal();

	/// <summary>
	///     Transform this state into a principal state.
	/// </summary>
	IPrincipalState<TStruct> ToPrincipal();

	/// <summary>
	///     Rotate this state by a <paramref name="rotationAngle" />.
	/// </summary>
	/// <remarks>
	///     <paramref name="rotationAngle" /> is positive if counterclockwise.
	/// </remarks>
	/// <param name="rotationAngle">The rotation angle in radians.</param>
	IState<TStruct> Transform(double rotationAngle);

	#endregion

}

/// <summary>
///     Interface to principal strain/stress states.
/// </summary>
/// <typeparam name="TStruct">The struct that represents the values of the object's components.</typeparam>
public interface IPrincipalState<TStruct> : IState<TStruct>
	where TStruct : struct
{

	#region Properties

	/// <summary>
	///     Get the <see cref="PrincipalCase" /> of this state.
	/// </summary>
	PrincipalCase Case { get; }

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
	///     Get the maximum component.
	/// </summary>
	TStruct S1 { get; }

	/// <summary>
	///     Get the minimum component.
	/// </summary>
	TStruct S2 { get; }

	/// <summary>
	///     Get the angle of maximum component direction, related to horizontal axis.
	/// </summary>
	double Theta1 { get; }

	/// <summary>
	///     Get the angle of minimum component direction, related to horizontal axis.
	/// </summary>
	double Theta2 { get; }

	#endregion

	#region Methods

	/// <summary>
	///     Get this principal state as a state.
	/// </summary>
	IState<TStruct> AsState();

	#endregion

}