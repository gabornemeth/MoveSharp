//
// SpeedAndCadence.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using MoveSharp.Models;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Combined speed and cadence value
    /// </summary>
    public struct SpeedAndCadence
    {
        public Speed Speed { get; set; }
        /// <summary>
        /// Revolutions per minute
        /// </summary>
        public byte Cadence { get; set; }

        public SpeedAndCadence(Speed speed, byte cadence)
            : this()
        {
            Speed = speed;
            Cadence = cadence;
        }

        public SpeedAndCadence(float speedValue, SpeedUnit speedUnit, byte cadence)
            : this()
        {
            Speed = new Speed(speedValue, speedUnit);
            Cadence = cadence;
        }

        public bool IsEmpty
        {
            get { return Cadence == 0 && Speed.HasValue == false; }
        }

        public bool Equals(SpeedAndCadence other)
        {
            return Speed == other.Speed && Cadence == other.Cadence;
        }

        public static bool operator ==(SpeedAndCadence p1, SpeedAndCadence p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(SpeedAndCadence p1, SpeedAndCadence p2)
        {
            return !p1.Equals(p2);
        }
    }
}
