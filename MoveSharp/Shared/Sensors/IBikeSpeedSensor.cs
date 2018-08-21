//
// IBikeSpeedSensor.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
namespace MoveSharp.Sensors
{
    /// <summary>
    /// Bike speed sensor interface
    /// </summary>
    public interface IBikeSpeedSensor : ISpeedSensor
    {
        /// <summary>
        /// Wheel size in millimeters
        /// </summary>
        int WheelSize { get; set; }
    }

    /// <summary>
    /// Combined bike speed and cadence sensor
    /// </summary>
    public interface IBikeSpeedAndCadenceSensor : IBikeSpeedSensor, ICadenceSensor
    {
        SpeedAndCadence SpeedAndCadence { get; }
    }
}
