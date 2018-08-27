//
// IStrideSensor.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
namespace MoveSharp.Sensors
{
    /// <summary>
    /// Stride sensor
    /// </summary>
    public interface IStrideSensor : ISpeedSensor, ICadenceSensor
    {
        SpeedAndCadence SpeedAndCadence { get; }
    }
}
