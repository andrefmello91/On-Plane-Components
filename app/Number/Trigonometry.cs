using System;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace Extensions.Number
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns the cosine of an <paramref name="angle"/> in radians.
        /// </summary>
        /// <param name="absoluteValue">Return absolute values? (default: false).</param>
        public static double Cos(this double angle, bool absoluteValue = false) =>
            absoluteValue
                ? Trig.Cos(angle).CoerceZero(1E-6).Abs()
                : Trig.Cos(angle).CoerceZero(1E-6);

        /// <summary>
        /// Returns the sine of an <paramref name="angle"/> in radians.
        /// </summary>
        /// <param name="absoluteValue">Return absolute values? (default: false).</param>
        public static double Sin(this double angle, bool absoluteValue = false) =>
            absoluteValue
                ? Trig.Sin(angle).CoerceZero(1E-6).Abs()
                : Trig.Sin(angle).CoerceZero(1E-6);

        /// <summary>
        /// Returns the tangent of an <paramref name="angle"/> in radians.
        /// </summary>
        /// <param name="absoluteValue">Return absolute values? (default: false).</param>
        public static double Tan(this double angle, bool absoluteValue = false) =>
            absoluteValue
                ? Trig.Tan(angle).CoerceZero(1E-6).Abs()
                : Trig.Tan(angle).CoerceZero(1E-6);

        /// <summary>
        /// Returns the cotangent of an <paramref name="angle"/> in radians.
        /// </summary>
        /// <param name="absoluteValue">Return absolute values? (default: false).</param>
        public static double Cotan(this double angle, bool absoluteValue = false) => 1.0 / angle.Tan(absoluteValue);

        /// <summary>
        /// Returns the arc-cosine of a <paramref name="cosine"/>, in radians.
        /// </summary>
        public static double Acos(this double cosine) => Trig.Acos(cosine);

        /// <summary>
        /// Returns the arc-sine of a <paramref name="sine"/>, in radians.
        /// </summary>
        public static double Asin(this double sine) => Trig.Asin(sine);

        /// <summary>
        /// Returns the arc-tangent of a <paramref name="tangent"/>, in radians.
        /// </summary>
        public static double Atan(this double tangent) => Trig.Atan(tangent);

        /// <summary>
        /// Returns the direction cosines (cos, sin) of an <paramref name="angle"/> in radians.
        /// </summary>
        /// <param name="absoluteValue">Return absolute values? (default: false).</param>
        public static (double cos, double sin) DirectionCosines(this double angle, bool absoluteValue = false) => (angle.Cos(absoluteValue), angle.Sin(absoluteValue));

        /// <summary>
        /// Convert an <paramref name="angle"/>, in radians, to degrees.
        /// </summary>
        public static double ToDegree(this double angle) => Trig.RadianToDegree(angle);

        /// <summary>
        /// Convert an <paramref name="angle"/>, in radians, to degrees.
        /// </summary>
        public static double ToDegree(this int angle) => Trig.RadianToDegree(angle);

        /// <summary>
        /// Convert an <paramref name="angle"/>, in degrees, to radians.
        /// </summary>
        public static double ToRadian(this double angle) => Trig.DegreeToRadian(angle);

        /// <summary>
        /// Convert an <paramref name="angle"/>, in degrees, to radians.
        /// </summary>
        public static double ToRadian(this int angle) => Trig.DegreeToRadian(angle);
    }
}
