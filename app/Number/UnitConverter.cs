using System;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace Extensions.Number
{
    public static partial class Extensions
    {
		/// <summary>
        /// Convert this <paramref name="length"/> to another <see cref="LengthUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="LengthUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="LengthUnit"/>.</param>
        public static double Convert(this double length, LengthUnit fromUnit, LengthUnit toUnit = LengthUnit.Millimeter) => fromUnit == toUnit ? length : UnitConverter.Convert(length, fromUnit, toUnit);

		/// <summary>
        /// Convert this <paramref name="length"/> to another <see cref="LengthUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="LengthUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="LengthUnit"/>.</param>
        public static double Convert(this int length, LengthUnit fromUnit, LengthUnit toUnit = LengthUnit.Millimeter) => ((double) length).Convert(fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="length"/> in <see cref="LengthUnit.Millimeter"/> to another <see cref="LengthUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="LengthUnit"/>.</param>
        public static double ConvertFromMillimeter(this double length, LengthUnit toUnit) => length.Convert(LengthUnit.Millimeter, toUnit);

        /// <summary>
        /// Convert this <paramref name="length"/> in <see cref="LengthUnit.Millimeter"/> to another <see cref="LengthUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="LengthUnit"/>.</param>
        public static double ConvertFromMillimeter(this int length, LengthUnit toUnit) => length.Convert(LengthUnit.Millimeter, toUnit);

        /// <summary>
        /// Convert this <paramref name="area"/> to another <see cref="AreaUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="AreaUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="AreaUnit"/>.</param>
        public static double Convert(this double area, AreaUnit fromUnit, AreaUnit toUnit = AreaUnit.SquareMillimeter) => fromUnit == toUnit ? area : UnitConverter.Convert(area, fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="area"/> to another <see cref="AreaUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="AreaUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="AreaUnit"/>.</param>
        public static double Convert(this int area, AreaUnit fromUnit, AreaUnit toUnit = AreaUnit.SquareMillimeter) => ((double) area).Convert(fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="area"/> in <see cref="AreaUnit.SquareMillimeter"/> to another <see cref="AreaUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="AreaUnit"/>.</param>
        public static double ConvertFromSquareMillimeter(this double area, AreaUnit toUnit) => area.Convert(AreaUnit.SquareMillimeter, toUnit);

        /// <summary>
        /// Convert this <paramref name="area"/> in <see cref="AreaUnit.SquareMillimeter"/> to another <see cref="AreaUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="AreaUnit"/>.</param>
        public static double ConvertFromSquareMillimeter(this int area, AreaUnit toUnit) => area.Convert(AreaUnit.SquareMillimeter, toUnit);

        /// <summary>
        /// Convert this <paramref name="force"/> to another <see cref="ForceUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="ForceUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="ForceUnit"/>.</param>
        public static double Convert(this double force, ForceUnit fromUnit, ForceUnit toUnit = ForceUnit.Newton) => force.IsNaN() ? 0 :  fromUnit == toUnit ? force : UnitConverter.Convert(force, fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="force"/> to another <see cref="ForceUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="ForceUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="ForceUnit"/>.</param>
        public static double Convert(this int force, ForceUnit fromUnit, ForceUnit toUnit = ForceUnit.Newton) => ((double) force).Convert(fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="force"/> in <see cref="ForceUnit.Newton"/> to another <see cref="ForceUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="ForceUnit"/>.</param>
        public static double ConvertFromNewton(this double force, ForceUnit toUnit) => force.Convert(ForceUnit.Newton, toUnit);

        /// <summary>
        /// Convert this <paramref name="force"/> in <see cref="ForceUnit.Newton"/> to another <see cref="ForceUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="ForceUnit"/>.</param>
        public static double ConvertFromNewton(this int force, ForceUnit toUnit) => force.Convert(ForceUnit.Newton, toUnit);

        /// <summary>
        /// Convert this <paramref name="pressure"/> to another <see cref="PressureUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="PressureUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="PressureUnit"/>.</param>
        public static double Convert(this double pressure, PressureUnit fromUnit, PressureUnit toUnit = PressureUnit.Megapascal) => pressure.IsNaN() ? 0 : fromUnit == toUnit ? pressure : UnitConverter.Convert(pressure, fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="pressure"/> to another <see cref="PressureUnit"/>.
        /// </summary>
        /// <param name="fromUnit">The base <see cref="PressureUnit"/>.</param>
        /// <param name="toUnit">The target <see cref="PressureUnit"/>.</param>
        public static double Convert(this int pressure, PressureUnit fromUnit, PressureUnit toUnit = PressureUnit.Megapascal) => ((double) pressure).Convert(fromUnit, toUnit);

        /// <summary>
        /// Convert this <paramref name="pressure"/> in <see cref="PressureUnit.Megapascal"/> to another <see cref="PressureUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="PressureUnit"/>.</param>
        public static double ConvertFromMPa(this double pressure, PressureUnit toUnit) => pressure.Convert(PressureUnit.Megapascal, toUnit);

        /// <summary>
        /// Convert this <paramref name="pressure"/> in <see cref="PressureUnit.Megapascal"/> to another <see cref="PressureUnit"/>.
        /// </summary>
        /// <param name="toUnit">The target <see cref="PressureUnit"/>.</param>
        public static double ConvertFromMPa(this int pressure, PressureUnit toUnit) => pressure.Convert(PressureUnit.Megapascal, toUnit);
    }
}
