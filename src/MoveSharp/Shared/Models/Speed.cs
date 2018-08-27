//
// SpeedUnit.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Runtime.Serialization;

namespace MoveSharp.Models
{
    /// <summary>
    /// Unit of speed
    /// </summary>
    public enum SpeedUnit
    {
        [EnumMember(Value = "kph")]
        KilometerPerHour,
        [EnumMember(Value = "mph")]
        MilePerHour,
        [EnumMember(Value = "mps")]
        MeterPerSecond,
        [EnumMember(Value = "min/km")]
        MinutePerKilometer,
        [EnumMember(Value = "min/mile")]
        MinutePerMile,
    }

    /// <summary>
    /// Extension methods for <see cref="SpeedUnit" />
    /// </summary>
    public static class SpeedUnitExtensions
    {
        public static bool IsPace(this SpeedUnit speed)
        {
            return speed == SpeedUnit.MinutePerKilometer || speed == SpeedUnit.MinutePerMile;
        }
    }

    /// <summary>
    /// Speed
    /// </summary>
    public struct Speed
    {
        /// <summary>
        /// Empty value
        /// </summary>
        public readonly static Speed Empty = new Speed();

        private static readonly float[] Conversion = { 1.0f, 1.609f, 3.6f, 60, 96.56f };

        public float Value { get; set; }

        public SpeedUnit Unit { get; set; }

        public bool HasValue
        {
            get
            {
                return Value != 0.0f;
            }
        }

        public Speed(float value, SpeedUnit unit)
            : this()
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Returns the value in the desired unit
        /// </summary>
        /// <param name="unit">the desired unit</param>
        /// <returns>value in <c>unit</c></returns>
        public float GetValueAs(SpeedUnit unit)
        {
            return GetValueAs(Value, Unit, unit);
        }

        public static float GetValueAs(float value, SpeedUnit from, SpeedUnit unit)
        {
            if (System.Math.Abs(value) < 1e-3)
                return 0;

            if (unit.IsPace() && !from.IsPace() ||
                !unit.IsPace() && from.IsPace())
            {
                // convert between pace and speed
                return Conversion[(int)unit] / (value * Conversion[(int)from]);
            }
            else
            {
                // spare conversion to the same unit
                return from == unit ? value : value * Conversion[(int)from] / Conversion[(int)unit];
            }

        }

        public override bool Equals(object obj)
        {
            if (obj is Speed)
                return Equals((Speed)obj);

            return false;
        }

        public bool Equals(Speed compareTo)
        {
            return Value == compareTo.GetValueAs(Unit);
        }

        public static bool operator ==(Speed a, Speed b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Speed a, Speed b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
