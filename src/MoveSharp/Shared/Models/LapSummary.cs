//
// LapSummary.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;

namespace MoveSharp.Models
{
    /// <summary>
    /// Summary information about a lap
    /// Can be easily serialized
    /// </summary>
    public class LapSummary : ILapSummary
    {
        public virtual Distance Distance
        {
            get;
            set;
        }

        public int AvgHeartRate
        {
            get;
            set;
        }

        public int MaxHeartRate
        {
            get;
            set;
        }

        public int AvgPower
        {
            get;
            set;
        }

        public int MaxPower
        {
            get;
            set;
        }

        public Speed AvgSpeed
        {
            get;
            set;
        }

        public Speed MaxSpeed
        {
            get;
            set;
        }

        public int AvgCadence
        {
            get;
            set;
        }

        public int MaxCadence
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public int ElapsedTime
        {
            get;
            set;
        }

        public int MovingTime
        {
            get;
            set;
        }

        public float Ascent
        {
            get;
            set;
        }

        public float Descent
        {
            get;
            set;
        }

        public static LapSummary FromLap(ILapSummary lap)
        {
            var summary = new LapSummary();
            summary.CopyFrom(lap);
            return summary;
        }

        public void CopyFrom(ILapSummary source)
        {
            Distance = source.Distance;
            AvgSpeed = source.AvgSpeed;
            MaxSpeed = source.MaxSpeed;
            AvgHeartRate = source.AvgHeartRate;
            MaxHeartRate = source.MaxHeartRate;
            AvgPower = source.AvgPower;
            MaxPower = source.MaxPower;
            AvgCadence = source.AvgCadence;
            MaxCadence = source.MaxCadence;
            ElapsedTime = source.ElapsedTime;
            MovingTime = source.MovingTime;
        }
    }
}
