//
// ILapSummary.cs
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
    /// Summary of a lap
    /// TODO: add moving time!
    /// </summary>
    public interface ILapSummary
    {
        /// <summary>
        /// Distance
        /// </summary>
        Distance Distance { get; }
        /// <summary>
        /// Total elevation gain [meters]
        /// </summary>
        float Ascent { get; }
        /// <summary>
        /// Total elevation loss [meters]
        /// </summary>
        float Descent { get; }
        /// <summary>
        /// Average heart rate [bpm]
        /// </summary>
        int AvgHeartRate { get; }
        /// <summary>
        /// Maxiumum heart rate [bpm]
        /// </summary>
        int MaxHeartRate { get; }
        /// <summary>
        /// Average power [watt]
        /// </summary>
        int AvgPower { get; }
        /// <summary>
        /// Maximum power [watt]
        /// </summary>
        int MaxPower { get; }
        /// <summary>
        /// Average speed
        /// </summary>
        Speed AvgSpeed { get; }
        /// <summary>
        /// Maximum speed
        /// </summary>
        Speed MaxSpeed { get; }
        /// <summary>
        /// Average cadence [rpm]
        /// </summary>
        int AvgCadence { get; }
        /// <summary>
        /// Maximum cadence [rpm]
        /// </summary>
        int MaxCadence { get; }
        /// <summary>
        /// Starting time of the lap
        /// </summary>
        DateTime StartTime { get; }
        /// <summary>
        /// Time elapsed [seconds]
        /// </summary>
        int ElapsedTime { get; }
        /// <summary>
        /// Moving time [seconds]
        /// </summary>
        int MovingTime { get; }
    }
}
