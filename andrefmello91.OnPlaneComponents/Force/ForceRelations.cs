using andrefmello91.Extensions;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents;

/// <summary>
///     Static class for force relations.
/// </summary>
public static class ForceRelations
{

	#region Methods

	/// <summary>
	///     Calculate components of a resultant force.
	/// </summary>
	/// <param name="resultant">Absolute value of force resultant.</param>
	/// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
	public static (double X, double Y) CalculateComponents(double resultant, double angle)
	{
		if (angle.ApproxZero())
			return (resultant, 0);

		if (angle.Approx(Constants.PiOver2, 1E-3))
			return (0, resultant);

		if (angle.Approx(Constants.Pi, 1E-3))
			return (-resultant, 0);

		if (angle.Approx(Constants.Pi3Over2, 1E-3))
			return (0, -resultant);

		return
			(resultant * angle.Acos(), resultant * angle.Asin());
	}

	/// <summary>
	///     Calculate Force components of a resultant force.
	/// </summary>
	/// <param name="resultantForce">Absolute force resultant.</param>
	/// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
	public static (Force X, Force Y) CalculateComponents(Force resultantForce, double angle)
	{
		var (x, y) = CalculateComponents(resultantForce.Value, angle);

		return
			(Force.From(x, resultantForce.Unit), Force.From(y, resultantForce.Unit));
	}

	/// <summary>
	///     Calculate the absolute value of resultant force.
	/// </summary>
	/// <param name="componentX">Value of force component in X direction (positive to right).</param>
	/// <param name="componentY">Value of force component in Y direction (positive upwards).</param>
	public static double CalculateResultant(double componentX, double componentY)
	{
		if (componentX.ApproxZero() && componentY.ApproxZero())
			return 0;

		return
			(componentX * componentX + componentY * componentY).Sqrt();
	}

	/// <summary>
	///     Calculate the absolute value of resultant force.
	/// </summary>
	/// <param name="forceX">Value of force component in X direction (positive to right).</param>
	/// <param name="forceY">Value of force component in Y direction (positive upwards).</param>
	/// <param name="unit">The unit of force to return (default: N).</param>
	public static Force CalculateResultant(Force forceX, Force forceY, ForceUnit unit = ForceUnit.Newton)
	{
		if (forceX == Force.Zero && forceY == Force.Zero)
			return Force.Zero;

		return
			Force.From(CalculateResultant(forceX.ToUnit(unit).Value, forceY.ToUnit(unit).Value), unit);
	}

	/// <summary>
	///     Calculate the angle of the resultant force.
	/// </summary>
	/// <param name="componentX">Value of force component in X direction.</param>
	/// <param name="componentY">Value of force component in Y direction.</param>
	public static double CalculateResultantAngle(double componentX, double componentY)
	{
		if (componentX > 0 && componentY.ApproxZero())
			return 0;

		if (componentX < 0 && componentY.ApproxZero())
			return Constants.Pi;

		if (componentX.ApproxZero() && componentY > 0)
			return Constants.PiOver2;

		if (componentX.ApproxZero() && componentY < 0)
			return Constants.Pi3Over2;

		return
			(componentY / componentX).Atan();
	}

	/// <summary>
	///     Calculate the angle of the resultant force.
	/// </summary>
	/// <param name="forceX">Value of force component in X direction (positive to right).</param>
	/// <param name="forceY">Value of force component in Y direction (positive upwards).</param>
	public static double CalculateResultantAngle(Force forceX, Force forceY) => CalculateResultantAngle(forceX.Value, forceY.ToUnit(forceX.Unit).Value);

	#endregion

}