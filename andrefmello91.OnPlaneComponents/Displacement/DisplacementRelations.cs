using Extensions;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Static class for displacement relations.
	/// </summary>
	public static class DisplacementRelations
	{

		#region Methods

		/// <summary>
		///     Calculate components of a resultant displacement.
		/// </summary>
		/// <param name="resultant">Absolute value of displacement resultant.</param>
		/// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
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
		///     Calculate Displacement components of a resultant displacement.
		/// </summary>
		/// <param name="resultantDisplacement">Absolute displacement resultant.</param>
		/// <param name="angle">Angle that displacement resultant is pointing at, in radians.</param>
		public static (Length X, Length Y) CalculateComponents(Length resultantDisplacement, double angle)
		{
			var (x, y) = CalculateComponents(resultantDisplacement.Value, angle);

			return
				(Length.From(x, resultantDisplacement.Unit), Length.From(y, resultantDisplacement.Unit));
		}

		/// <summary>
		///     Calculate the resultant displacement.
		/// </summary>
		/// <param name="componentX">Value of displacement component in X direction (positive to right).</param>
		/// <param name="componentY">Value of displacement component in Y direction (positive upwards).</param>
		public static double CalculateResultant(double componentX, double componentY)
		{
			if (componentX.ApproxZero() && componentY.ApproxZero())
				return 0;

			return
				(componentX * componentX + componentY * componentY).Sqrt();
		}

		/// <summary>
		///     Calculate the resultant displacement.
		/// </summary>
		/// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length" />).</param>
		/// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length" />).</param>
		/// <param name="unit">The unit of displacement (default: mm).</param>
		public static Length CalculateResultant(Length displacementX, Length displacementY, LengthUnit unit = LengthUnit.Millimeter)
		{
			if (displacementX == Length.Zero && displacementY == Length.Zero)
				return Length.Zero;

			return
				Length.From(CalculateResultant(displacementX.ToUnit(unit).Value, displacementY.ToUnit(unit).Value), unit);
		}

		/// <summary>
		///     Calculate the angle of the resultant displacement.
		/// </summary>
		/// <param name="componentX">Value of displacement component in X direction (positive to right).</param>
		/// <param name="componentY">Value of displacement component in Y direction (positive upwards).</param>
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
		///     Calculate the angle of the resultant displacement.
		/// </summary>
		/// <param name="displacementX">Displacement component in X direction (positive to right) (<see cref="Length" />).</param>
		/// <param name="displacementY">Displacement component in Y direction (positive upwards) (<see cref="Length" />).</param>
		public static double CalculateResultantAngle(Length displacementX, Length displacementY) => CalculateResultantAngle(displacementX.Value, displacementY.ToUnit(displacementX.Unit).Value);

		#endregion

	}
}