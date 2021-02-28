using Extensions;
using UnitsNet;
using UnitsNet.Units;
using static OnPlaneComponents.ForceRelations;

namespace OnPlaneComponents
{
	/// <summary>
	///     Force struct.
	/// </summary>
	public partial struct PlaneForce : IPlaneComponent<PlaneForce, Force>, IUnitConvertible<PlaneForce, ForceUnit>
	{
		#region Fields

		/// <summary>
		///     Get a <see cref="PlaneForce" /> with zero value.
		/// </summary>
		public static readonly PlaneForce Zero = new PlaneForce(0, 0);

		/// <summary>
		///     The tolerance to consider forces equal.
		/// </summary>
		public static readonly UnitsNet.Force Tolerance = UnitsNet.Force.FromNewtons(1E-6);

		#endregion

		#region Properties

		/// <summary>
		///     Get/set the force unit (<see cref="ForceUnit" />).
		/// </summary>
		public ForceUnit Unit
		{
			get => X.Unit;
			set => ChangeUnit(value);
		}

		/// <summary>
		///     Get the applied force directions.
		/// </summary>
		public ComponentDirection Direction => IsZero
			? ComponentDirection.None
			: IsXZero switch
			{
				false when IsYZero => ComponentDirection.X,
				true when !IsYZero => ComponentDirection.Y,
				_                  => ComponentDirection.Both
			};


		public bool IsZero => IsXZero && IsYZero;

		public bool IsXZero => X.ApproxZero(Tolerance);

		public bool IsYZero => Y.ApproxZero(Tolerance);

		/// <summary>
		///     Get the force component in X direction.
		/// </summary>
		public Force X { get; private set; }

		/// <summary>
		///     Get the force component in Y direction.
		/// </summary>
		public Force Y { get; private set; }

		/// <summary>
		///     Verify if force resultant is approximately zero.
		/// </summary>
		public bool IsResultantZero => Resultant.Abs() <= Tolerance;

		/// <summary>
		///     Get the resultant force value.
		/// </summary>
		public Force Resultant { get; private set; }

		/// <summary>
		///     Get the resultant force angle, in radians.
		/// </summary>
		public double ResultantAngle => CalculateResultantAngle(X, Y);

		#endregion

		#region Constructors

		/// <summary>
		///     Force object.
		/// </summary>
		/// <param name="componentX">Value of force component in X direction, in <paramref name="unit" />. (positive to right).</param>
		/// <param name="componentY">Value of force component in Y direction, in <paramref name="unit" />. (positive upwards).</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public PlaneForce(double componentX, double componentY, ForceUnit unit = ForceUnit.Newton)
			: this (UnitsNet.Force.From(componentX.ToZero(), unit), UnitsNet.Force.From(componentY.ToZero(), unit))
		{
		}

		/// <summary>
		///     Force object.
		/// </summary>
		/// <param name="forceX"><see cref="UnitsNet.Force" /> component in X direction (positive to right).</param>
		/// <param name="forceY"><see cref="UnitsNet.Force" /> component in Y direction (positive upwards).</param>
		public PlaneForce(UnitsNet.Force forceX, UnitsNet.Force forceY)
		{
			X         = forceX.ToZero();
			Y         = forceY.ToZero().ToUnit(forceX.Unit);
			Resultant = CalculateResultant(X, Y, X.Unit);
		}

		#endregion

		#region

		/// <summary>
		///     Get a <see cref="PlaneForce" /> in X direction.
		/// </summary>
		/// <param name="value">Value of force component in X direction (positive to right).</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public static PlaneForce InX(double value, ForceUnit unit = ForceUnit.Newton) => new PlaneForce(value, 0, unit);

		/// <summary>
		///     Get a <see cref="PlaneForce" /> in X direction.
		/// </summary>
		/// <param name="force"><see cref="UnitsNet.Force" /> component in X direction (positive to right).</param>
		public static PlaneForce InX(UnitsNet.Force force) => new PlaneForce(force, UnitsNet.Force.Zero);

		/// <summary>
		///     Get a <see cref="PlaneForce" /> in X direction.
		/// </summary>
		/// <param name="value">Value of force component in Y direction (positive upwards).</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public static PlaneForce InY(double value, ForceUnit unit = ForceUnit.Newton) => new PlaneForce(0, value, unit);

		/// <summary>
		///     Get a <see cref="PlaneForce" /> in Y direction.
		/// </summary>
		/// <param name="force"><see cref="UnitsNet.Force" /> component in Y direction (positive upwards).</param>
		public static PlaneForce InY(UnitsNet.Force force) => new PlaneForce(UnitsNet.Force.Zero, force);

		/// <summary>
		///     Get a <see cref="PlaneForce" /> from a resultant.
		/// </summary>
		/// <param name="resultant">Absolute value of force resultant.</param>
		/// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
		/// <param name="unit">The <see cref="ForceUnit" /> (default: <see cref="ForceUnit.Newton" />).</param>
		public static PlaneForce FromResultant(double resultant, double angle, ForceUnit unit = ForceUnit.Newton)
		{
			var (x, y) = CalculateComponents(resultant, angle);

			return new PlaneForce(x, y, unit);
		}

		/// <summary>
		///     Get a <see cref="PlaneForce" /> from a resultant.
		/// </summary>
		/// <param name="resultantForce">Absolute <see cref="UnitsNet.Force" /> resultant.</param>
		/// <param name="angle">Angle that force resultant is pointing at, in radians.</param>
		public static PlaneForce FromResultant(UnitsNet.Force resultantForce, double angle)
		{
			var (x, y) = CalculateComponents(resultantForce, angle);

			return
				new PlaneForce(x, y);
		}

		/// <summary>
		///     Change the <see cref="ForceUnit" /> of this object.
		/// </summary>
		/// <param name="unit">The <see cref="ForceUnit" /> to convert.</param>
		public void ChangeUnit(ForceUnit unit)
		{
			if (unit == Unit)
				return;

			// Update values
			X         = X.ToUnit(unit);
			Y         = Y.ToUnit(unit);
			Resultant = CalculateResultant(X, Y).ToUnit(unit);
		}

		/// <summary>
		///     Convert this <see cref="PlaneForce" /> to another <see cref="ForceUnit" />.
		/// </summary>
		/// <inheritdoc cref="ChangeUnit" />
		public PlaneForce Convert(ForceUnit unit) => unit == Unit
			? this
			: new PlaneForce(X.ToUnit(unit), Y.ToUnit(unit));

		public PlaneForce Clone() => new PlaneForce(X, Y);

		public bool Approaches(PlaneForce other, UnitsNet.Force tolerance) => X.Approx(other.X, tolerance) && Y.Approx(other.X, tolerance);

		/// <inheritdoc cref="Approaches" />
		/// <remarks>
		///     Default <see cref="Tolerance" /> is considered.
		/// </remarks>
		public bool Equals(PlaneForce other) => Approaches(other, Tolerance);

		public override bool Equals(object obj) => obj is PlaneForce other && Equals(other);

		public override string ToString() =>
			$"Fx = {X}\n" +
			$"Fy = {Y}";

		public override int GetHashCode() => (int) (X.Value * Y.Value);

		#endregion
	}
}