namespace OnPlaneComponents
{
    public partial struct Displacement
    {
        /// <summary>
        /// Returns true if components are equal.
        /// </summary>
        public static bool operator == (Displacement left, Displacement right) => left.Equals(right);

        /// <summary>
        /// Returns true if components are different.
        /// </summary>
        public static bool operator != (Displacement left, Displacement right) => !left.Equals(right);

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with summed components, in left argument's unit.
        /// </summary>
        public static Displacement operator + (Displacement left, Displacement right) => new Displacement(left.X + right.X.ToUnit(left.Unit), left.Y + right.Y.ToUnit(left.Unit));

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with subtracted components, in left argument's unit.
        /// </summary>
        public static Displacement operator - (Displacement left, Displacement right) => new Displacement(left.X - right.X.ToUnit(left.Unit), left.Y - right.Y.ToUnit(left.Unit));

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static Displacement operator *(Displacement displacement, double multiplier) => new Displacement(multiplier * displacement.X, multiplier * displacement.Y);

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with multiplied components by a <see cref="double"/>.
        /// </summary>
        public static Displacement operator *(double multiplier, Displacement displacement) => displacement * multiplier;

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static Displacement operator *(Displacement displacement, int multiplier) => displacement * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with multiplied components by an <see cref="int"/>.
        /// </summary>
        public static Displacement operator *(int multiplier, Displacement displacement) => displacement * (double)multiplier;

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with components divided by a <see cref="double"/>.
        /// </summary>
        public static Displacement operator /(Displacement displacement, double divider) => new Displacement(displacement.X / divider, displacement.Y / divider);

        /// <summary>
        /// Returns a <see cref="Displacement"/> object with components divided by an <see cref="int"/>.
        /// </summary>
        public static Displacement operator /(Displacement displacement, int divider) => displacement / (double)divider;

    }
}
