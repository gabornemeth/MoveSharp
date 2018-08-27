//
// ConversionHelper.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using MoveSharp.Models;
using System.Globalization;

namespace MoveSharp.Helpers
{
    /// <summary>
    /// Helper class for converting various properties into human readable string representation
    /// </summary>
    public class ConversionHelper
    {
        public static DateTime GetDateTimeFromUtcString(string utc)
        {
            DateTime time;
            if (DateTime.TryParseExact(utc, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.None, out time))
                return time;
            else if (DateTime.TryParseExact(utc, "yyyy-MM-ddTHH:mm:ss.fff", null, DateTimeStyles.None, out time))
                return time;
            if (DateTime.TryParseExact(utc, "yyyy-MM-ddTHH:mm:ssZ", null, DateTimeStyles.None, out time))
                return time;
            else if (DateTime.TryParseExact(utc, "yyyy-MM-ddTHH:mm:ss.fffZ", null, DateTimeStyles.None, out time))
                return time;

            throw new ArgumentException(nameof(utc));
        }

        public static string FormatTime(int seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
        }

        public const string None = "-";
        public const string HeartRateUnit = "bpm";
        public const string CadenceUnit = "rpm";
        public const string PowerUnit = "W";

        public static string FormatHeartRate(int hr, bool displayUnits = false)
        {
            if (hr == 0)
                return None;

            return displayUnits ? hr.ToString() + " " + HeartRateUnit : hr.ToString();
        }

        public static string FormatPower(int power, bool displayUnits = false)
        {
            if (power == 0)
                return None;
            return displayUnits ? power.ToString() + " " + PowerUnit : power.ToString();
        }

        public static string FormatCadence(int cadence, bool displayUnits = false)
        {
            if (cadence == 0)
                return None;
            return displayUnits ? string.Format("{0} rpm", cadence) : cadence.ToString();
        }

        #region Ascent

        public static string FormatAscent(UnitSystem unitSystem, Distance distance, bool displayUnits = false)
        {
            string ascentAsString = null;

            if (unitSystem == UnitSystem.Metric)
                ascentAsString = Convert.ToInt32(distance.GetValueAs(DistanceUnit.Meter)).ToString();
            else
                ascentAsString = Convert.ToInt32(distance.GetValueAs(DistanceUnit.Foot)).ToString();

            return displayUnits ? ascentAsString + " " + GetAscentUnits(unitSystem, distance) : ascentAsString;
        }

        public static string GetAscentUnits(UnitSystem unitSystem, Distance distance)
        {
            if (unitSystem == UnitSystem.Metric)
                return "m";
            else
                return "ft";
        }

        #endregion

        #region Distance

        public static string FormatDistance(UnitSystem unitSystem, Distance distance, bool displayUnits = false)
        {
            string distanceAsString = null;

            if (unitSystem == UnitSystem.Metric)
            {
                if (distance.GetValueAs(DistanceUnit.Kilometer) < 1)
                    distanceAsString = Convert.ToInt32(distance.GetValueAs(DistanceUnit.Meter)).ToString();
                else
                    distanceAsString = distance.GetValueAs(DistanceUnit.Kilometer).ToString("F1");
            }
            else
            {
                if (distance.GetValueAs(DistanceUnit.Mile) < 1)
                    distanceAsString = Convert.ToInt32(distance.GetValueAs(DistanceUnit.Foot)).ToString();
                else
                    distanceAsString = distance.GetValueAs(DistanceUnit.Mile).ToString("F1");
            }

            return displayUnits ? distanceAsString + " " + GetDistanceUnits(unitSystem, distance) : distanceAsString;
        }

        public static string GetDistanceUnits(UnitSystem unitSystem, Distance distance)
        {
            if (unitSystem == UnitSystem.Metric)
                return distance.GetValueAs(DistanceUnit.Kilometer) < 1 ? "m" : "km";
            else
                return distance.GetValueAs(DistanceUnit.Mile) < 1 ? "ft" : "mi";
        }

        #endregion

        #region Speed

        public static string FormatSpeed(UnitSystem unitSystem, Speed speed, bool usePace = false, bool displayUnits = false)
        {
            string speedAsString = null;
            if (usePace)
            {
                if (unitSystem == UnitSystem.Metric)
                    speedAsString = speed.Value == 0 ? None : PaceHelper.GetPaceAsString(speed.GetValueAs(SpeedUnit.MinutePerKilometer));
                else
                    speedAsString = speed.Value == 0 ? None : PaceHelper.GetPaceAsString(speed.GetValueAs(SpeedUnit.MinutePerMile));
            }
            else
            {
                if (unitSystem == UnitSystem.Metric)
                    speedAsString = speed.Value == 0 ? None : speed.GetValueAs(SpeedUnit.KilometerPerHour).ToString("F1");
                else
                    speedAsString = speed.Value == 0 ? None : speed.GetValueAs(SpeedUnit.MilePerHour).ToString("F1");
            }

            return displayUnits ? speedAsString + " " + GetSpeedUnits(unitSystem, usePace) : speedAsString;
        }

        /// <summary>
        /// Speed unit as string
        /// </summary>
        public static string GetSpeedUnits(UnitSystem unitSystem, bool usePace)
        {
            if (usePace)
                return unitSystem == UnitSystem.Metric ? "min/km" : "min/mi";
            else
                return unitSystem == UnitSystem.Metric ? "kph" : "mph";
        }

        #endregion
    }
}
