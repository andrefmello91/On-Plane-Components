namespace OnPlaneComponents
{
	public partial struct Force
	{
		/// <summary>
        /// Returns true if components are equal.
        /// </summary>
		public static bool operator == (Force left, Force right) => left.Equals(right);

		/// <summary>
		/// Returns true if components are different.
		/// </summary>
		public static bool operator != (Force left, Force right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="Force"/> object with summed components, in left argument's unit.
        /// </summary>
        public static Force operator + (Force left, Force right) => new Force(left.X + right.X.ToUnit(left.Unit), left.Y + right.Y.ToUnit(left.Unit));

        /// <summary>
        /// Returns a <see cref="Force"/> object with subtracted components, in left argument's unit.
        /// </summary>
        public static Force operator - (Force left, Force right) => new Force(left.X - right.X.ToUnit(left.Unit), left.Y - right.Y.ToUnit(left.Unit));

        /// <summary>
        /// Returns a <see cref="Force"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static Force operator * (Force force, double multiplier) => new Force(multiplier * force.X, multiplier * force.Y);

        /// <summary>
        /// Returns a <see cref="Force"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static Force operator * (double multiplier, Force force) => force * multiplier;

        /// <summary>
        /// Returns a <see cref="Force"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static Force operator * (Force force, int multiplier) => force * (double) multiplier;

        /// <summary>
        /// Returns a <see cref="Force"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static Force operator * (int multiplier, Force force) => force * (double) multiplier;

        /// <summary>
        /// Returns a <see cref="Force"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static Force operator / (Force force, double divider) => new Force(force.X / divider, force.Y / divider);

        /// <summary>
        /// Returns a <see cref="Force"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static Force operator / (Force force, int divider) => force / (double) divider;
	}
}