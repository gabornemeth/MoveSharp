//
// ISpeedSensor.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using MoveSharp.Models;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Speed sensor interface
    /// </summary>
    public interface ISpeedSensor : ISensor
    {
        Speed Speed { get; }
    }
}
