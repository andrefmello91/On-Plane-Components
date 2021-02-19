namespace OnPlaneComponents
{
	public partial struct PlaneForce
	{
		/// <summary>
        /// Returns true if components are equal.
        /// </summary>
		public static bool operator == (PlaneForce left, PlaneForce right) => left.Equals(right);

		/// <summary>
		/// Returns true if components are different.
		/// </summary>
		public static bool operator != (PlaneForce left, PlaneForce right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with summed components, in left argument's unit.
        /// </summary>
        public static PlaneForce operator + (PlaneForce left, PlaneForce right) => new PlaneForce(left.X + right.X.ToUnit(left.Unit), left.Y + right.Y.ToUnit(left.Unit));

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with subtracted components, in left argument's unit.
        /// </summary>
        public static PlaneForce operator - (PlaneForce left, PlaneForce right) => new PlaneForce(left.X - right.X.ToUnit(left.Unit), left.Y - right.Y.ToUnit(left.Unit));

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PlaneForce operator * (PlaneForce planeForce, double multiplier) => new PlaneForce(multiplier * planeForce.X, multiplier * planeForce.Y);

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static PlaneForce operator * (double multiplier, PlaneForce planeForce) => planeForce * multiplier;

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PlaneForce operator * (PlaneForce planeForce, int multiplier) => planeForce * (double) multiplier;

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static PlaneForce operator * (int multiplier, PlaneForce planeForce) => planeForce * (double) multiplier;

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static PlaneForce operator / (PlaneForce planeForce, double divider) => new PlaneForce(planeForce.X / divider, planeForce.Y / divider);

        /// <summary>
        /// Returns a <see cref="PlaneForce"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static PlaneForce operator / (PlaneForce planeForce, int divider) => planeForce / (double) divider;
	}
}