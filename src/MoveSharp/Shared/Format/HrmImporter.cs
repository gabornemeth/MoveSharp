//
// HrmImporter.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
// HRM File format is specified at: http://www.polar.com/files/Polar_HRM_file%20format.pdf
// 

using MoveSharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Format
{
    /// <summary>
    /// Importing Polar HRM files
    /// </summary>
    public class HrmImporter : ActivityImporter
    {
        private enum DataType
        {
            HeartRate,
            Speed,
            Cadence,
            Altitude,
            Power
        };

        private readonly Version v102 = new Version("1.0.2");
        private readonly Version v105 = new Version("1.0.5");
        private readonly Version v106 = new Version("1.0.6");
        private readonly Version v107 = new Version("1.0.7");

        private UnitSystem _unitSystem;
        private int _dataRecordingInterval;
        private DateTime _startTime;
        private Version _version;
        private ActivitySummary _summary = new ActivitySummary();
        private bool _hasSpeed;
        private bool _hasCadence;
        private bool _hasAltitude;
        private bool _hasPower;

        public HrmImporter(MemoryActivity activity)
            : base(activity)
        {
        }

        private bool SectionBegins(string text, string sectionName)
        {
            return text.StartsWith("[" + sectionName + "]");
        }

        private Speed GetSpeed(string speedAsString)
        {
            return new Speed
            {
                Value = Convert.ToInt32(speedAsString) / 10.0f,
                Unit = _unitSystem == UnitSystem.Metric ? SpeedUnit.KilometerPerHour : SpeedUnit.MilePerHour
            };
        }

        private Distance GetDistance(string distanceAsString)
        {
            return new Distance
            {
                Value = Convert.ToInt32(distanceAsString) / 10.0f,
                Unit = _unitSystem == UnitSystem.Metric ? DistanceUnit.Kilometer : DistanceUnit.Mile
            };
        }

        private Distance GetAltitude(string altitudeAsString)
        {
            return new Distance
            {
                Value = _version == v102 ?
                     Convert.ToInt32(altitudeAsString) * 10.0f : Convert.ToInt32(altitudeAsString),
                Unit = _unitSystem == UnitSystem.Metric ? DistanceUnit.Meter : DistanceUnit.Foot
            };
        }


        /// <summary>
        /// Parsing a Property=value line from the HRM file, and returns the value part if the property is the one we check
        /// </summary>
        /// <param name="text">one line</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="result"></param>
        /// <returns>true if the line contains the searched property, false otherwise</returns>
        private bool GetValueOf(string text, string propertyName, out string result)
        {
            result = null;
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(propertyName))
                return false;

            if (!text.StartsWith(propertyName + "="))
                return false; // it is not the searched property

            result = text.Substring(propertyName.Length + 1); // value is after propertyName=
            return true;
        }

        public override void Load(System.IO.Stream source)
        {
            using (var reader = new StreamReader(source))
            {
                string value;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (GetValueOf(line, "Version", out value))
                    {
                        _version = new Version(value.Insert(2, ".").Insert(1, "."));
                    }
                    else if (GetValueOf(line, "Mode", out value))
                    {
                        if (value[0] == '0')
                            _hasCadence = true;
                        else if (value[0] == '1')
                            _hasAltitude = true;

                        if (value[1] == '1')
                            _summary.Sport = Dynastream.Fit.Sport.Cycling;
                        else
                            _summary.Sport = Dynastream.Fit.Sport.Running;

                        if (value[2] == '0')
                            _unitSystem = UnitSystem.Metric;
                        else if (value[2] == '1')
                            _unitSystem = UnitSystem.American;
                    }
                    else if (GetValueOf(line, "SMode", out value))
                    {
                        if (value[0] == '1')
                            _hasSpeed = true;
                        if (value[1] == '1')
                            _hasCadence = true;
                        if (value[2] == '1')
                            _hasAltitude = true;
                        if (value[3] == '1')
                            _hasPower = true;
                        if (value[6] == '1')
                            _summary.Sport = Dynastream.Fit.Sport.Cycling;
                        else
                            _summary.Sport = Dynastream.Fit.Sport.Running;

                        if (value[7] == '0')
                            _unitSystem = UnitSystem.Metric;
                        else if (value[7] == '1')
                            _unitSystem = UnitSystem.American;

                    }
                    else if (GetValueOf(line, "Date", out value))
                    {
                        DateTime date;
                        if (DateTime.TryParseExact(value, "yyyyMMdd", null, DateTimeStyles.None, out date))
                            _startTime = date;

                    }
                    else if (GetValueOf(line, "StartTime", out value))
                    {
                        value = value.Remove(value.Length - 2); // remove the tenth second
                        DateTime time;
                        if (DateTime.TryParseExact(value, "HH:mm:ss", null, DateTimeStyles.None, out time))
                        {
                            _startTime = _startTime.Add(time.TimeOfDay);
                        }
                    }
                    else if (GetValueOf(line, "Interval", out value))
                    {
                        _dataRecordingInterval = Convert.ToInt32(value);
                    }
                    else if (SectionBegins(line, "Trip"))
                    {
                        // Cycling trip data - summary
                        for (int i = 0; i < 8; i++)
                        {
                            var tripData = reader.ReadLine();
                            if (i == 0)
                                _summary.Distance = GetDistance(tripData);
                            else if (i == 2)
                                _summary.ElapsedTime = Convert.ToInt32(tripData);
                            else if (i == 5)
                                _summary.AvgSpeed = GetSpeed(tripData);
                            else if (i == 6)
                                _summary.MaxSpeed = GetSpeed(tripData);
                        }
                    }
                    else if (SectionBegins(line, "HRData"))
                    {
                        var timestamp = _startTime;
                        DataType[] columns = null;
                        while (!reader.EndOfStream)
                        {
                            var data = reader.ReadLine();
                            var values = data.Split('\t');
                            if (columns == null)
                            {
                                var defaultColumns = new[] { DataType.HeartRate, DataType.Speed, DataType.Cadence, DataType.Altitude, DataType.Power };

                                // Parse the first line to determine the columns we have
                                columns = new DataType[values.Length];
                                var idx = 0;
                                for (int i = 0; i < defaultColumns.Length && idx < columns.Length; i++)
                                {
                                    switch (defaultColumns[i])
                                    {
                                        case DataType.HeartRate:
                                            columns[idx++] = DataType.HeartRate;
                                            break;
                                        case DataType.Speed:
                                            if (_hasSpeed)
                                                columns[idx++] = DataType.Speed;
                                            break;
                                        case DataType.Cadence:
                                            if (_hasCadence)
                                                columns[idx++] = DataType.Cadence;
                                            break;
                                        case DataType.Altitude:
                                            if (_hasAltitude)
                                                columns[idx++] = DataType.Altitude;
                                            break;
                                        case DataType.Power:
                                            if (_hasPower)
                                                columns[idx++] = DataType.Power;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            var frame = new ActivityTimeFrame
                            {
                                Timestamp = timestamp,
                                HeartRate = Convert.ToByte(values[0])
                            };

                            for (int i = 1; i < columns.Length; i++)
                            {
                                switch (columns[i])
                                {
                                    case DataType.Speed:
                                        frame.Speed = GetSpeed(values[i]);
                                        break;
                                    case DataType.Cadence:
                                        frame.Cadence = Convert.ToByte(values[i]);
                                        break;
                                    case DataType.Power:
                                        frame.Power = Convert.ToUInt16(values[i]);
                                        break;
                                    case DataType.Altitude:
                                        frame.Altitude = GetAltitude(values[i]);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            Activity.AddTimeFrame(frame);
                            timestamp = timestamp.AddSeconds(_dataRecordingInterval);
                        }
                    }
                }

                Activity.SetSummary(_summary);
            }
        }
    }
}
