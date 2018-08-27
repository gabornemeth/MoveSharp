//
// BikeSpeedSensorDescription.cs
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
    /// Description of a bike speed sensor
    /// </summary>
    public class BikeSpeedSensorDescription : SensorDescription, IBikeSpeedSensor
    {
        /// <summary>
        /// Wheel size in millimeters
        /// </summary>
        public int WheelSize { get; set; }

        private Speed _speed = new Speed();
        public Speed Speed
        {
            get { return _speed; }
        }

        public BikeSpeedSensorDescription()
        {
            
        }

        public BikeSpeedSensorDescription(IBikeSpeedSensor sensor) : base(sensor)
        {
            WheelSize = sensor.WheelSize;
        }
    }
}
