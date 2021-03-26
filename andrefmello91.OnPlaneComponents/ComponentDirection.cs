using System.Xml.Serialization;

namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Component attributes class.
	/// </summary>
	public class ComponentAttribute : XmlEnumAttribute
	{

		#region Properties

		/// <summary>
		///     Get/set the X direction.
		/// </summary>
		/// <remarks>
		///     True if there is a component in this direction.
		/// </remarks>
		public bool X { get; set; }

		/// <summary>
		///     Get/set the X direction.
		/// </summary>
		/// <inheritdoc cref="X" />
		public bool Y { get; set; }

		#endregion

	}

	/// <summary>
	///     Condition direction enumeration.
	/// </summary>
	public enum ComponentDirection
	{
		/// <summary>
		///     No components in X and Y directions.
		/// </summary>
		[Component(Name = "None", X = false, Y = false)]
		None,

		/// <summary>
		///     Component exists only in X (horizontal) direction.
		/// </summary>
		[Component(Name = "X", X = true, Y = false)]
		X,

		/// <summary>
		///     Component exists only in Y (vertical) direction.
		/// </summary>
		[Component(Name = "Y", X = false, Y = true)]
		Y,

		/// <summary>
		///     Components exist in both X (horizontal) and Y (vertical) directions.
		/// </summary>
		[Component(Name = "Both", X = true, Y = true)]
		Both
	}
}