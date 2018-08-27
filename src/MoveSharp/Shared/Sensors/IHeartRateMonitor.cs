//
// IHeartRateMonitor.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;

namespace MoveSharp.Sensors
{
    public class HeartRateMeasurement
    {
        /// <summary>
        /// Heart rate value in bpm (beats per minute)
        /// </summary>
        public ushort HeartRateValue { get; set; }
        public bool HasExpendedEnergy { get; set; }
        public ushort ExpendedEnergy { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int[] RRValues { get; set; }

        public bool HasRR
        {
            get
            {
                return RRValues != null && RRValues.Length > 0;
            }
        }

        public override string ToString()
        {
            return HeartRateValue + " bpm @ " + Timestamp;
        }
    }

    /// <summary>
    /// Heart rate monitor
    /// Different heart rate monitor implementations should be derived from this class in order to RTTI be working
    /// </summary>
    public interface IHeartRateMonitor : ISensor
    {
        HeartRateMeasurement HeartRateData { get; }
    }
}
